using StereoKit;

namespace SKRecording
{
    // Recorder instance for tracking a Head
    class HeadRecorder : Recorder
    {
        // The model to display
        private ModelWrapper headModel;

        public HeadRecorder()
        {
            headModel = new VRHead();
        }

        // Returns a one length array of RecordingData, that of the head that is being tracked
        public RecordingData[] getCurrentFrame()
        {
            RecordingData[] res = { new RecordingData(Input.Head) };
            return res;
        }

        // Expects a one length array of RecordingData and displays the data in the format of a head model. 
        public void displayFrame(RecordingData[] data)
        {
            headModel.show(data);
        }

        // One head, so we return 1
        public int getObjectCount()
        {
            return 1;
        }
    }
}
