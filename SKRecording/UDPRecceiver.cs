using System;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Collections.Generic;
using Windows.Graphics.Imaging;
using System.Threading;

namespace SKRecording
{

    struct DPacketHeader
    {
        public ulong timestamp;
        public ulong length;
        public int offset;
        public float sensorRotationX;
        public float sensorRotationY;
        public float sensorRotationZ;
        public float sensorRotationW;
        public float sensorPositionX;
        public float sensorPositionY;
        public float sensorPositionZ;
        public float rigNodeRotationX;
        public float rigNodeRotationY;
        public float rigNodeRotationZ;
        public float rigNodeRotationW;
        public float rigNodePositionX;
        public float rigNodePositionY;
        public float rigNodePositionZ;
        public float dummyZero1;
        public float dummyZero2;

        public DPacketHeader(DataReader reader)
        {
            timestamp = reader.ReadUInt64();
            length = reader.ReadUInt64();
            offset = reader.ReadInt32();
            sensorRotationX = reader.ReadSingle();
            sensorRotationY = reader.ReadSingle();
            sensorRotationZ = reader.ReadSingle();
            sensorRotationW = reader.ReadSingle();
            sensorPositionX = reader.ReadSingle();
            sensorPositionY = reader.ReadSingle();
            sensorPositionZ = reader.ReadSingle();
            rigNodeRotationX = reader.ReadSingle();
            rigNodeRotationY = reader.ReadSingle();
            rigNodeRotationZ = reader.ReadSingle();
            rigNodeRotationW = reader.ReadSingle();
            rigNodePositionX = reader.ReadSingle();
            rigNodePositionY = reader.ReadSingle();
            rigNodePositionZ = reader.ReadSingle();
            dummyZero1 = reader.ReadSingle();
            dummyZero2 = reader.ReadSingle();

            if(!(dummyZero1 == 0 && dummyZero2 == 0))
            {
                throw new ArgumentException("Two last values in D Packet header were not 0");
            }
        }
    }

    class UDPRecceiver
    {
        private const int DEPTH_PORT = 23941;
        private DatagramSocket sock;
        private int port;
        private string IP;
        private bool receivedExtrinsics = false;
        private bool initialized;
        private Stack<ulong> timestampQueue;
        // byte[][] because if fragmentationcount > 1, we have multiple byte[] per frame
        private Dictionary<ulong, (byte[][], int)> timestampToCompressedImg;
        private int imgWidth;
        private int imgHeight;
        private int pixelStride;
        private int rowStride;
        private int fragmentationCount;

        UDPRecceiver(string IP, int port)
        {
            this.initialized = false;
            this.timestampQueue = new Stack<ulong>();
            this.timestampToCompressedImg = new Dictionary<ulong, (byte[][], int)>();
            this.IP = IP;
            this.port = port;
            this.sock = new DatagramSocket();
            sock.MessageReceived += onPacketReceived;
        }

        BitmapFrame getLatestImage()
        {
            ulong latestTimestamp = 0;
            bool available = false;
            lock (timestampQueue)
            {
                if(timestampQueue.Count > 0)
                {
                    latestTimestamp = timestampQueue.Pop();
                    available = true;
                }
            }
            if (!available)
            {
                return null;
            }

            return ImageParser.decompress(timestampToCompressedImg[latestTimestamp].Item1, port != DEPTH_PORT);
        }

        private async void onPacketReceived(DatagramSocket socket, DatagramSocketMessageReceivedEventArgs eventArguments)
        {
            // Once we receive a first packet, we know our initialization request was received
            this.initialized = true;
            DataReader dataReader = eventArguments.GetDataReader();
            dataReader.UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding.Utf8;
            byte packetType = dataReader.ReadByte();

            if (!receivedExtrinsics)
            {
                if(packetType == 'E')
                {
                    parseExtrinsics(dataReader);
                    IOutputStream outputStream = await socket.GetOutputStreamAsync(
                                        eventArguments.RemoteAddress,
                                        eventArguments.RemotePort);
                    byte[] ack = new byte[1] { (byte)'A' };
                    await outputStream.WriteAsync(ack.AsBuffer());

                }
                else if (packetType == 'D')
                {
                    receivedExtrinsics = true;
                }
            }
            if (packetType == 'D')
            {
                DPacketHeader header = parsePacketHeader(dataReader);
                byte[] compressedImg = new byte[dataReader.UnconsumedBufferLength];
                dataReader.ReadBytes(compressedImg);

                if (timestampToCompressedImg.ContainsKey(header.timestamp))
                {
                    (byte[][], int) dictTuple;
                    dictTuple.Item1 = timestampToCompressedImg[header.timestamp].Item1;
                    dictTuple.Item1[header.offset] = compressedImg;
                    dictTuple.Item2 = timestampToCompressedImg[header.timestamp].Item2 + 1;
                    timestampToCompressedImg[header.timestamp] = dictTuple;
                    if(dictTuple.Item2 == this.fragmentationCount)
                    {
                        lock (timestampQueue)
                        {
                            timestampQueue.Push(header.timestamp);
                        }
                    }

                }

            }

        }

        private void parseExtrinsics(DataReader extrinsicsReader)
        {
            this.imgWidth = extrinsicsReader.ReadInt32();
            this.imgHeight = extrinsicsReader.ReadInt32();
            this.pixelStride = extrinsicsReader.ReadInt32();
            this.rowStride = extrinsicsReader.ReadInt32();
            this.fragmentationCount = extrinsicsReader.ReadInt32();
            // Not reading principal point, focal length, and rig to cam transform

        }

        public void start()
        {
            Thread initThread = new Thread(InitializationThread);
            initThread.Start();
            Thread latThread = new Thread(LatencyThread);
            latThread.Start();
        }

        async void InitializationThread()
        {
            while (!initialized)
            {
                IOutputStream outputStream = await this.sock.GetOutputStreamAsync(
                                    new Windows.Networking.HostName(this.IP),
                                    this.port.ToString());
                byte[] ack = new byte[1] { (byte)'I' };
                await outputStream.WriteAsync(ack.AsBuffer());
                // Sleep for a 50 milisecs and then send initialization request in case the previous one was not delivered since we're using UDP
                Thread.Sleep(50);
            }
        }

        async void LatencyThread()
        {
            ulong latestTimestamp = 0;
            while (true)
            {
                bool readTimestamp = false;
                lock (timestampQueue)
                {
                    if(timestampQueue.Count > 0)
                    {
                        latestTimestamp = timestampQueue.Pop();
                        readTimestamp = true;
                    }
                }
                if (!readTimestamp)
                {
                    Thread.Sleep(1000);
                    continue;
                }

                IOutputStream outputStream = await this.sock.GetOutputStreamAsync(
                    new Windows.Networking.HostName(this.IP),
                    this.port.ToString());
                byte[] opcode = new byte[1] { (byte)'L' };
                byte[] value = BitConverter.GetBytes(latestTimestamp);
                byte[] toSend = Utils.concatBytes(new byte[][] { opcode, value });
                await outputStream.WriteAsync(toSend.AsBuffer());
                Thread.Sleep(1000);
            }
        }

        DPacketHeader parsePacketHeader(DataReader headerReader)
        {
            return new DPacketHeader(headerReader);
        }
    }
}
