using System;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using System.Runtime.InteropServices.WindowsRuntime;

namespace SKRecording
{
    
    class UDPRecceiver
    {
        private DatagramSocket sock;
        private int port;
        private string IP;
        private bool receivedExtrinsics = false;
        private int imgWidth;
        private int imgHeight;
        private int pixelStride;
        private int rowStride;
        private int fragmentationCount;

        UDPRecceiver(string IP, int port)
        {
            this.IP = IP;
            this.port = port;
            this.sock = new DatagramSocket();
            sock.MessageReceived += onPacketReceived;

        }

        async void onPacketReceived(DatagramSocket socket, DatagramSocketMessageReceivedEventArgs eventArguments)
        {
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

        }

        void parseExtrinsics(DataReader extrinsicsReader)
        {
            this.imgWidth = extrinsicsReader.ReadInt32();
            this.imgHeight = extrinsicsReader.ReadInt32();
            this.pixelStride = extrinsicsReader.ReadInt32();
            this.rowStride = extrinsicsReader.ReadInt32();
            this.fragmentationCount = extrinsicsReader.ReadInt32();
            // Not reading principal point, focal length, and rig to cam transform

        }

        void parsePacketHeader(DataReader headerReader)
        {

        }
    }
}
