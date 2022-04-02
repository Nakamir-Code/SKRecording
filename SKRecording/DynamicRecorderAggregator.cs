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

        public override void RecordOneFrame(Matrix anchorTRS)
        {
            RecordingData[] poses = getCurrentRecordingData(anchorTRS);
            
            recording.Enqueue(coder.Serialize(DeserializedRecordingArray.fromRecordingDataArray(poses)));
        }

        public override bool PlaybackOneFrame(Matrix anchorTRS)
        {
            if (recording.Count == 0) return false;
            RecordingData[] frame = coder.Deserialize<DeserializedRecordingArray>(recording.Dequeue()).toRecordingDataArray();

            displayAll(frame, anchorTRS);

            return true;

        }

        public override bool hasRecording()
        {
            return recording.Count != 0;
        }

    }
}
