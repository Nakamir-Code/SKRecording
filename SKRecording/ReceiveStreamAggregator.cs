using System;
using System.Collections.Generic;
using StereoKit;

namespace SKRecording
{
    class ReceiveStreamAggregator : RecordingAggregator
    {
        private TCPServer server;
        private Queue<string> buffered;
        private JsonCoder coder;
        private bool listening = false;
        private int nothingReceivedCounter;
        // How many frames we wait until we assume the connection is gone
        private int timeout;

        public ReceiveStreamAggregator(Recorder[] recs, string ip, int port, int timeoutFrames) : base(recs)
        {
            server = new TCPServer(ip, port, '{', '}', ';');
            server.decodedFrame += onDecodedFrame;
            this.buffered = new Queue<string>();
            this.coder = new JsonCoder();
            nothingReceivedCounter = 0;
            this.timeout = timeoutFrames;
        }

        private void onDecodedFrame(object sender, string[] decoded)
        {
            lock (buffered)
            {
                for (int i = 0; i < decoded.Length; i++)
                {
                    buffered.Enqueue(decoded[i]);
                }
            }
        }

        public override bool hasRecording()
        {
            return true;
        }

        public override bool PlaybackOneFrame(Matrix anchorTRS)
        {
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
            lock (buffered)
            {
                if (buffered.Count != 0)
                {
                    frameJSON = buffered.Dequeue();
                }
            }
            if (frameJSON != null)
            {
                try
                {
                    Pose[] frame = coder.Deserialize<DeserializedPoseArray>(frameJSON).toPoseArray();
                    displayPoses(frame, anchorTRS);
                }
                catch (Exception e)
                {
                    return true;
                }
            }

            // If the recordings over, reset variables
            if (buffered.Count == 0)
            {
                if(nothingReceivedCounter++ >= timeout)
                {
                    server.reset();
                    return false;
                }
            }

            return true;
        }

        public override void RecordOneFrame(Matrix anchorTRS)
        {
            throw new InvalidOperationException("ReceiveStreamAggregator can only receive");
        }

        public override void finishPlayback()
        {
            server.reset();
            listening = false;
        }

        public override void finishRecording()
        {
            throw new InvalidOperationException("ReceiveStreamAggregator can only receive");
        }


    }
}
