using StereoKit;

namespace SKRecording
{

    public static class VRHead
    {
        static Model headModel;
        static bool initialized;
        static float nodeScale;
        static Quat defaultRot;


        public static void init()
        {
            if (initialized)
            {
                return;
            }
            initialized = true;
            headModel = Model.FromFile("head_11k_centered.obj");
            nodeScale = 1;
            defaultRot = Quat.FromAngles(0, 180f, 0);

        }

        public static void showHead(Pose pose)
        {
            headModel.RootNode.ModelTransform = Matrix.TRS(pose.position, pose.orientation * defaultRot, nodeScale);
            headModel.Draw(Matrix.Identity);
        }
    }
}
