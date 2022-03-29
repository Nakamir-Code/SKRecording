using System.Collections.Generic;
using StereoKit;

namespace SKRecording
{
    class DynamicRecorderAggregator : RecordingAggregator
    {
        Queue<string> recording;
        JsonCoder coder;

        public DynamicRecorderAggregator(Recorder[] recorders): base(recorders)
        {
            this.recording = new Queue<string>();
            this.coder = new JsonCoder();
        }

        public override void RecordOneFrame()
        {
            Pose[] poses = getCurrentPoses();
            
            recording.Enqueue(coder.Serialize(DeserializedPoseArray.fromPoseArray(poses)));
        }

        public override bool PlaybackOneFrame()
        {
            if (recording.Count == 0) return false;
            Pose[] frame = coder.Deserialize<DeserializedPoseArray>(recording.Dequeue()).toPoseArray();

            displayPoses(frame);

            return true;

        }

        public override bool hasRecording()
        {
            return recording.Count != 0;
        }

    }
}
