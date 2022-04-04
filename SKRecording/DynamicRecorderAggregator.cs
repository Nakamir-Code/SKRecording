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
            int[] paramLengths = getCurrentParamLengths();

            recording.Enqueue(coder.Serialize(DeserializedRecordingArray.fromRecordingDataArray(poses,paramLengths)));
        }

        public override bool PlaybackOneFrame(Matrix anchorTRS)
        {
            if (recording.Count == 0) return false;
            DeserializedRecordingArray deserialized = coder.Deserialize<DeserializedRecordingArray>(recording.Dequeue());
            RecordingData[] frame = deserialized.toRecordingDataArray();
            int[] paramLengths = deserialized.getParamLengths();

            displayAll(frame, paramLengths, anchorTRS);

            return true;

        }

        public override bool hasRecording()
        {
            return recording.Count != 0;
        }

    }
}
