using StereoKit;

namespace SKRecording
{

    public class VRHead : ModelWrapper
    {
        private Model headModel;
        private float nodeScale;
        private Quat defaultRot;

        public VRHead()
        {
            headModel = Model.FromFile("head_11k_centered.obj");
            nodeScale = 1;
            defaultRot = Quat.FromAngles(0, 180f, 0);
        }

        public void show(RecordingData[] data)
        {
            headModel.RootNode.ModelTransform = Matrix.TRS(data[0].pose.position, data[0].pose.orientation * defaultRot, nodeScale);
            headModel.Draw(Matrix.Identity);
        }
    }
}
