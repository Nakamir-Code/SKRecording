using System;
using System.Collections.Generic;
using StereoKit;

namespace SKRecording
{
    class RemoteRecordingAggregator : RecordingAggregator
    {
        private RecordingTCPClient client;
        Queue<string> recording;
        JsonCoder coder;
        bool initiatedPlayback = false;
        bool connected = false;

        public RemoteRecordingAggregator(Recorder[] recs, string ip, int port) : base(recs)
        {
            client = new RecordingTCPClient(ip, port);
            client.decodedFrame += onDecodedFrame;
            this.recording = new Queue<string>();
            this.coder = new JsonCoder();
        }

        private void onDecodedFrame(object sender, string[] decoded)
        {
            lock (recording)
            {
                for (int i = 0; i < decoded.Length; i++)
                {
                    recording.Enqueue(decoded[i]);
                }
            }
        }

        public override bool hasRecording()
        {
            return true;
        }

        public override bool PlaybackOneFrame(Matrix anchorTRS)
        {
            if (!connected)
            {
                client.connect();
                connected = true;
            }

            if (!initiatedPlayback)
            {
                client.send("P", true);
                initiatedPlayback = true;
                // We don't receive anything on the first invokation yet, but return true to indicate stream is still going
                return true;
            }

            string frameJSON = null;
            lock (recording)
            {
                if (recording.Count != 0) {
                    frameJSON = recording.Dequeue();
                }
            }
            if(frameJSON != null)
            {
                try
                {
                    Pose[] frame = coder.Deserialize<DeserializedPoseArray>(frameJSON).toPoseArray();
                    displayPoses(frame, anchorTRS);
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine("Caught an exception when deserializing/displaying: " + e.Message);
                    return true;
                }
            }

            // If the recordings over, reset variables
            if (client.hasReceivedAll() && recording.Count == 0)
            {
                client.reset();
                initiatedPlayback = false;
                return false;
            }

            return true;
        }

        public override void RecordOneFrame(Matrix anchorTRS)
        {
            if (!connected)
            {
                client.connect();
                connected = true;
            }
            Pose[] poses = getCurrentPoses(anchorTRS);
            string serializedPoses = coder.Serialize(DeserializedPoseArray.fromPoseArray(poses));
            client.send(serializedPoses);
        }

        public override void finishPlayback()
        {
            client.reset();
            connected = false;
            initiatedPlayback = false;
        }

        public override void finishRecording()
        {
            client.reset();
            connected = false;
        }


    }
}
