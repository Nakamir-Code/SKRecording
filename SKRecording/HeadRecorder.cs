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

        public Pose[] getCurrentFrame()
        {
            Pose[] res = { Input.Head };
            return res;
        }

        public void displayFrame(Pose[] poses)
        {
            headModel.show(poses);
        }

        public int getPoseCount()
        {
            return 1;
        }
    }
}
