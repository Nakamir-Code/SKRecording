using StereoKit;

namespace SKRecording
{
    // Global struct that is passed around containing data for one object in one frame
    public struct RecordingData
    {
        public Pose pose;
        public string text;
        public RecordingData(Pose pose, string text)
        {
            this.pose = pose;
            this.text = text;
        }
        public RecordingData(Pose pose)
        {
            this.pose = pose;
            this.text = "";
        }
    }
}
