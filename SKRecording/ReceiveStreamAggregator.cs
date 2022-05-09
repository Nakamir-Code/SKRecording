using System;
using System.Collections.Generic;
using StereoKit;

namespace SKRecording
{
    // RecordingAggregator used for receving and displaying streamed data from a different hololens
    // Note: Cannot be used for streaming data out, see RemoteRecordingAggreagtor for that.
    class ReceiveStreamAggregator : RecordingAggregator
    {
        // Server for receiving data
        private TCPServer server;
        // Buffered received data that is yet to be parsed & displayed
        private Queue<string> buffered;
        // Coder for en- and decoding JSON
        private JsonCoder coder;
        private bool listening = false;
        // Counts for how many frames we haven't received data
        private int nothingReceivedCounter;
        // How many frames we wait until we assume the connection is gone
        private int timeout;

        public ReceiveStreamAggregator(IPoseTrackerShower[] recs, string ip, int port, int timeoutFrames) : base(recs)
        {
            // Beginning and end of JSON is { and }, seperator between frames is ;
            server = new TCPServer(ip, port, '{', '}', ';');
            server.decodedFrame += onDecodedFrame;
            this.buffered = new Queue<string>();
            this.coder = new JsonCoder();
            nothingReceivedCounter = 0;
            this.timeout = timeoutFrames;
        }

        // Callback for when we are done parsing a full JSON string from the stream
        private void onDecodedFrame(object sender, string[] decoded)
        {
            // Lock the queue since it is accessed by multiple threads
            lock (buffered)
            {
                for (int i = 0; i < decoded.Length; i++)
                {
                    buffered.Enqueue(decoded[i]);
                }
            }
        }

        // We assume the remote host always has some data to stream
        public override bool hasRecording()
        {
            return true;
        }

        // Display one frame relative to the provided anchor
        public override bool PlaybackOneFrame(Matrix anchorTRS)
        {
            // If we are not connected yet, connect
            if (!listening)
            {
                server.connect();
                listening = true;
            }
            int a = server.getClientCount();
            // Listen indefinetely while no one is connected yet
            if (server.getClientCount() < 1)
            {
                return true;
            }

            string frameJSON = null;
            // Once we have a connection, check the queue if we have received & parsed any data
            lock (buffered)
            {
                if (buffered.Count != 0)
                {
                    frameJSON = buffered.Dequeue();
                }
            }
            // If we have any frames to display, convert them to RecordingData[] and display them relative to the provided anchor
            if (frameJSON != null)
            {
                try
                {
                    DeserializedRecordingArray deserialized = coder.Deserialize<DeserializedRecordingArray>(frameJSON);
                    Label3D[] frame = deserialized.toRecordingDataArray();
                    int[] paramLengths = deserialized.getParamLengths();

                    displayAll(frame, paramLengths, anchorTRS);

                }
                catch (Exception e)
                {
                    return true;
                }
            }

            if (buffered.Count == 0)
            {
                if(nothingReceivedCounter++ >= timeout)
                {
                    // If the recording is over (i.e. we reach our timeour), reset variables
                    server.reset();
                    return false;
                }
            }

            return true;
        }

        // Not meant to be used
        public override void StreamOneFrame(Matrix anchorTRS)
        {
            throw new InvalidOperationException("ReceiveStreamAggregator can only receive");
        }

        // Reset once we are done displaying data
        public override void finishPlayback()
        {
            server.reset();
            listening = false;
        }

        // Not meant to be used
        public override void finishRecording()
        {
            throw new InvalidOperationException("ReceiveStreamAggregator can only receive");
        }


    }
}
