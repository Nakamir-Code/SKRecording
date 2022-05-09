using StereoKit;

namespace SKRecording
{
    // Global class that is passed around containing data for one object in one frame 
    public class RecordingData
    {
        public Pose pose;
        public string text;
        public float timeStamp;
        
        public RecordingData(Pose pose, float timeStamp)
        {
            this.pose = pose;
            this.timeStamp = timeStamp;
        }
        public RecordingData clone()
        {
            return new RecordingData(this.pose, this.timeStamp);
        }
    }
}
