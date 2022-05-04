using System.Collections.Generic;
using StereoKit;

namespace SKRecording
{
    // Recording aggregator that uses a locally stored Queue of JSON strings to store recording data 
    class DynamicRecorderAggregator : RecordingAggregator
    {
        // The queue in which our recording is stored and retrieved
        Queue<string> recording;
        // JSON en- and decoder
        JsonCoder coder;

        public DynamicRecorderAggregator(Recorder[] recorders): base(recorders)
        {
            this.recording = new Queue<string>();
            this.coder = new JsonCoder();
        }

        // Converts one frame of RecordingData to JSON and saves it to the queue relative to the provided anchor
        public override void RecordOneFrame(Matrix anchorTRS)
        {
            // Fetch the current frame data relative to the provided anchor
            RecordingData[] poses = getCurrentRecordingData(anchorTRS);
            int[] paramLengths = getCurrentParamLengths();
            
            // Convert the RecordingData[] to a JSON string and save it to the queue
            recording.Enqueue(coder.Serialize(DeserializedRecordingArray.fromRecordingDataArray(poses,paramLengths)));
        }

        // Fetches the latest JSON string from the queue, converts it to RecordingData 
        // relative to the provided anchor, and displays it. Returns true on success and false on failure.
        public override bool PlaybackOneFrame(Matrix anchorTRS)
        {
            if (recording.Count == 0) return false;
            // Fetch the JSON string and convert it back to RecordingData[]
            DeserializedRecordingArray deserialized = coder.Deserialize<DeserializedRecordingArray>(recording.Dequeue());
            RecordingData[] frame = deserialized.toRecordingDataArray();
            int[] paramLengths = deserialized.getParamLengths();

            // Display the RecordingData relative to the provided anchor
            displayAll(frame, paramLengths, anchorTRS);

            return true;

        }

        public override bool hasRecording()
        {
            return recording.Count != 0;
        }

    }
}
