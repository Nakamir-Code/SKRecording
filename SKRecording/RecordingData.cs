using StereoKit;

namespace SKRecording
{
    // Global class that is passed around containing data for one object in one frame 
    public class RecordingData
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
        public RecordingData clone()
        {
            return new RecordingData(this.pose, this.text);
        }
    }
}
