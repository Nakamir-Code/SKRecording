using StereoKit;

namespace SKRecording
{
    // Global class that is passed around containing data for one object in one frame 
    public class Label3D
    {
        // TODO: put timestamp.
        public Pose pose;
        public string text;
        public Label3D(Pose pose, string text)
        {
            this.pose = pose;
            this.text = text;
        }
        public Label3D(Pose pose)
        {
            this.pose = pose;
            this.text = "";
        }
        public Label3D clone()
        {
            return new Label3D(this.pose, this.text);
        }
    }
}
