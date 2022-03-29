using System;
using SuperSimpleTcp;
using System.Collections.Generic;
using StereoKit;

namespace SKRecording
{
    class RemoteRecordingAggregator : RecordingAggregator
    {
        private SimpleTcpClient client;
        Queue<string> recording;
        JsonCoder coder;
        int totalFrameCount;
        int receivedCount;
        bool initiatedPlayback = false;

        public RemoteRecordingAggregator(Recorder[] recs, string ip, int port, int timeout) : base(recs)
        {
            client = new SimpleTcpClient(ip + ":" + port);
            client.Events.DataReceived += OnDataReceived;
            this.recording = new Queue<string>();
            this.coder = new JsonCoder();
            client.Connect();
            receivedCount = 0;
        }

        private void OnDataReceived(object sender, DataReceivedEventArgs e)
        {
            byte[] a = e.Data;
            int decodingStartIndex = 0;
            // First thing that is sent is the length marked with an L (76 in UTF-8)
            if(a[0] == 76)
            {
                totalFrameCount = BitConverter.ToInt32(a, 1);
                decodingStartIndex = 4;
            }

            // 32 bits or less types are automatically thread safe
            string data = System.Text.Encoding.UTF8.GetString(a,decodingStartIndex,a.Length - decodingStartIndex);
            string[] jsonStrs = data.Split(';');
            // First and last json might be cutoff, check
            int startingIndex = jsonStrs[0].Length != 0 && jsonStrs[0][0] == '{' ? 0 : 1;
            // Last character of the last element in jsonStrs
            int iterationCount;
            string lastElement = jsonStrs[jsonStrs.Length - 1];

            if (! (lastElement.Length == 0) && lastElement[lastElement.Length-1] == '}')
            {
                iterationCount = jsonStrs.Length;
            }
            else
            {
                iterationCount = jsonStrs.Length - 1;
                // Since we are skipping a frame, still save that we processed it. We do this only when skipping
                // frames at the end, as if we did it for front AND back we'd be counting frames double
                receivedCount++;
            }

            lock (recording)
            {
                for(int i = startingIndex; i < iterationCount; i++)
                {
                    recording.Enqueue(jsonStrs[i]);
                }
            }
        }

        public override bool hasRecording()
        {
            return true;
        }

        public override bool PlaybackOneFrame()
        {
            if (!initiatedPlayback)
            {
                client.Send("P");
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
                System.Diagnostics.Debug.WriteLine(frameJSON);
                try
                {
                    Pose[] frame = coder.Deserialize<DeserializedPoseArray>(frameJSON).toPoseArray();
                    displayPoses(frame);
                }
                catch(Exception e)
                {
                    return true;
                }

                receivedCount++;
            }

            // If the recordings over, reset variables
            if (receivedCount >= totalFrameCount)
            {
                receivedCount = 0;
                totalFrameCount = 0;
                initiatedPlayback = false;
                return false;
            }

            return true;
        }

        public override void RecordOneFrame()
        {
            Pose[] poses = getCurrentPoses();
            string serializedPoses = coder.Serialize(DeserializedPoseArray.fromPoseArray(poses));
            client.Send(serializedPoses);
        }

        public override void finishPlayback()
        {
            receivedCount = 0;
            totalFrameCount = 0;
            initiatedPlayback = false;
        }
    }
}
