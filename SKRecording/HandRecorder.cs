using StereoKit;

namespace SKRecording
{
    class HandRecorder : Recorder
    {
        private ModelWrapper handModel;
        private Handed whichHand;

        public HandRecorder(Handed whichHand)
        {
            handModel = new VRHand(whichHand);
            this.whichHand = whichHand;
        }

        public Pose[] getCurrentFrame()
        {
            return handJointsToPoseArray(Input.Hand(whichHand).fingers);
        }

        public void displayFrame(Pose[] poses)
        {
            handModel.show(poses);
        }

        public int getPoseCount()
        {
            return 25;
        }

        private static Pose[] handJointsToPoseArray(HandJoint[] joints)
        {
            Pose[] poses = new Pose[joints.Length];

            for (int i = 0; i < joints.Length; i++)
            {
                poses[i] = new Pose(joints[i].position, joints[i].orientation);
            }
            return poses;
        }
    }
}