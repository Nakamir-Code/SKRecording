using StereoKit;

namespace SKRecording
{
    // Recorder instance for tracking a Head
    class HeadTrackerShower : IPoseTrackerShower

    {
        // The model to display
        private ModelWrapper headModel;

        public HeadTrackerShower()
        {
            headModel = new VRHead();
        }

        // Returns a one length array of RecordingData, that of the head that is being tracked
        public Label3D[] getCurrentFrame()
        {
            Label3D[] res = { new Label3D(Input.Head) };
            return res;
        }

        // Expects a one length array of RecordingData and displays the data in the format of a head model. 
        public void displayFrame(Label3D[] data)
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
