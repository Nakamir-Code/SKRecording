using StereoKit;

namespace SKRecording
{
    class HeadRecorder : Recorder
    {
        private ModelWrapper headModel;

        public HeadRecorder()
        {
            headModel = new VRHead();
        }

        public RecordingData[] getCurrentFrame()
        {
            RecordingData[] res = { new RecordingData(Input.Head) };
            return res;
        }

        public void displayFrame(RecordingData[] data)
        {
            headModel.show(data);
        }

        public int getObjectCount()
        {
            return 1;
        }
    }
}
