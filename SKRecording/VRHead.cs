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

        public void show(Pose[] pose)
        {
            headModel.RootNode.ModelTransform = Matrix.TRS(pose[0].position, pose[0].orientation * defaultRot, nodeScale);
            headModel.Draw(Matrix.Identity);
        }
    }
}
