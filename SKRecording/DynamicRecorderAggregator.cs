using System.Collections.Generic;
using StereoKit;
using System;

namespace SKRecording
{
    class DynamicRecorderAggregator : RecordingAggregator
    {
        Queue<string> recording;
        Recorder[] recorders;
        Pose[] poseAggregator;
        JsonCoder coder;

        public DynamicRecorderAggregator(Recorder[] recorders)
        {
            this.recorders = recorders;
            this.recording = new Queue<string>();
            this.coder = new JsonCoder();
            int poseCount = 0;
            foreach(Recorder r in recorders)
            {
                poseCount += r.getPoseCount();
            }

            poseAggregator = new Pose[poseCount];

        }

        public void RecordOneFrame()
        {
            int i = 0;
            foreach(Recorder r in recorders)
            {
                r.getCurrentFrame().CopyTo(poseAggregator, i);
                i += +r.getPoseCount();
            }
            
            recording.Enqueue(coder.Serialize(DeserializedPoseArray.fromPoseArray(poseAggregator)));
        }

        public bool PlaybackOneFrame()
        {
            if (recording.Count == 0) return false;
            Pose[] frame = coder.Deserialize<DeserializedPoseArray>(recording.Dequeue()).toPoseArray();

            int i = 0;
            foreach(Recorder r in recorders)
            {
                r.displayFrame(new ArraySegment<Pose>(frame, i, r.getPoseCount()).ToArray());
                i += r.getPoseCount();
            }

            return true;

        }

        public bool hasRecording()
        {
            return recording.Count != 0;
        }

    }
}
