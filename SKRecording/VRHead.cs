using StereoKit;

namespace SKRecording
{

    // 3d head model representation
    public class VRHead : ModelWrapper
    {
        // The actual model
        private Model headModel;
        // Scale with which its displayed
        private float nodeScale;
        // Default rotation of the model (only has one bone)
        private Quat defaultRot;

        public VRHead()
        {
            headModel = Model.FromFile("head_11k_centered.obj");
            nodeScale = 1;
            defaultRot = Quat.FromAngles(0, 180f, 0);
        }

        // Displays the head with the first index of the RecordingData array provided
        public void show(RecordingData[] data)
        {
            headModel.RootNode.ModelTransform = Matrix.TRS(data[0].pose.position, data[0].pose.orientation * defaultRot, nodeScale);
            headModel.Draw(Matrix.Identity);
        }
    }
}
