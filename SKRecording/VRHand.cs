using StereoKit;
using System;

// Adapted from https://github.com/ClonedPuppy/SKHands/blob/master/Platforms/SKHands_DotNet/Program.cs
namespace SKRecording
{
    public class VRHand : ModelWrapper
    {
        private Model handModel;

        struct JointInfo
        {
            public ModelNode node;
            public FingerId finger;
            public JointId joint;
            public JointInfo(FingerId fingerId, JointId jointId, ModelNode fingerNode)
            {
                finger = fingerId;
                joint = jointId;
                node = fingerNode;
            }
        }

        private JointInfo[] jointInfo;
        private float nodeScale;
        private Quat defaultBoneRot;

        public VRHand(Handed whichHand)
        {

            if(whichHand == Handed.Left)
            {
                handModel = Model.FromFile("leftHand.glb");
            }
            else
            {
                handModel = Model.FromFile("rightHand.glb");
            }
            jointInfo = new JointInfo[] {
                new JointInfo(FingerId.Thumb, JointId.KnuckleMajor, handModel.FindNode("ThumbMeta")),
                new JointInfo(FingerId.Thumb, JointId.KnuckleMid,   handModel.FindNode("ThumbProxi")),
                new JointInfo(FingerId.Thumb, JointId.KnuckleMinor, handModel.FindNode("ThumbDist")),

                new JointInfo(FingerId.Index, JointId.Root,         handModel.FindNode("IndexMeta")),
                new JointInfo(FingerId.Index, JointId.KnuckleMajor, handModel.FindNode("IndexProxi")),
                new JointInfo(FingerId.Index, JointId.KnuckleMid,   handModel.FindNode("IndexInter")),
                new JointInfo(FingerId.Index, JointId.KnuckleMinor, handModel.FindNode("IndexDist")),

                new JointInfo(FingerId.Middle, JointId.Root,         handModel.FindNode("MiddleMeta")),
                new JointInfo(FingerId.Middle, JointId.KnuckleMajor, handModel.FindNode("MiddleProxi")),
                new JointInfo(FingerId.Middle, JointId.KnuckleMid,   handModel.FindNode("MiddleInter")),
                new JointInfo(FingerId.Middle, JointId.KnuckleMinor, handModel.FindNode("MiddleDist")),

                new JointInfo(FingerId.Ring, JointId.Root,         handModel.FindNode("RingMeta")),
                new JointInfo(FingerId.Ring, JointId.KnuckleMajor, handModel.FindNode("RingProxi")),
                new JointInfo(FingerId.Ring, JointId.KnuckleMid,   handModel.FindNode("RingInter")),
                new JointInfo(FingerId.Ring, JointId.KnuckleMinor, handModel.FindNode("RingDist")),

                new JointInfo(FingerId.Little, JointId.Root,         handModel.FindNode("PinkyMeta")),
                new JointInfo(FingerId.Little, JointId.KnuckleMajor, handModel.FindNode("PinkyProxi")),
                new JointInfo(FingerId.Little, JointId.KnuckleMid,   handModel.FindNode("PinkyInter")),
                new JointInfo(FingerId.Little, JointId.KnuckleMinor, handModel.FindNode("PinkyDist"))};

            defaultBoneRot = Quat.FromAngles(-90f, 0, 0);
            nodeScale = 1;
        }

        // Display a hand with the provided joint information
        public void show(Pose[] poses)
        {
            if (poses.Length != 25)
            {
                throw new Exception("Expected exactly 25 joints");
            }

            foreach (JointInfo j in jointInfo)
            {
                Pose joint = GetJoint(poses, j.finger, j.joint);
                j.node.ModelTransform = Matrix.TRS(joint.position, joint.orientation * defaultBoneRot, nodeScale);
            }

            handModel.Draw(Matrix.Identity);
        }

        private Pose GetJoint(Pose[] poses, FingerId finger, JointId joint)
        {
            return poses[5 * (int)finger + (int)joint];
        }

    }
}
