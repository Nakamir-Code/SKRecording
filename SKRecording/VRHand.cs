using StereoKit;
using System;

// Adapted from https://github.com/ClonedPuppy/SKHands/blob/master/Platforms/SKHands_DotNet/Program.cs
namespace SKRecording
{
    // Wrapper class for a hand 3d model
    public class VRHand : ModelWrapper
    {
        // The actual model
        private Model handModel;
        // Helper struct for managing handjoints
        class JointInfo
        {
            public ModelNode node;
            public FingerId finger;
            public JointId joint;
            public bool rootBone;
            public JointInfo(FingerId fingerId, JointId jointId, ModelNode fingerNode, bool fingerRootBone)
            {
                finger = fingerId;
                joint = jointId;
                node = fingerNode;
                rootBone = fingerRootBone;
            }
        }

        // Joint info of the hand we're managing
        private JointInfo[] jointInfo;
        // Scale with which to display the model
        private float nodeScale;
        // Default rotation of the model's bones
        private Quat defaultBoneRot;

        public VRHand(Handed whichHand)
        {

            string modelName = "Hand_" + whichHand.ToString() + ".glb";
            handModel = Model.FromFile(modelName);
            nodeScale = 1;
            defaultBoneRot = Quat.FromAngles(90, 0, 180);
            var nodes = handModel.Visuals;

            foreach (var node in nodes)
            {
                //making sure the high performaance mat is applied
                node.Material = Material.Default;
            }
            //instantiating the jointinfo class with all the relevant fields set  
            jointInfo = new JointInfo[] {
                // currently SK doesn't have an enum for wrist but populates the thumb root and knucklemajor with the same value,
                // I'm borrowing it here to store the wrist
                new JointInfo(FingerId.Thumb, JointId.Root, null, true),
                new JointInfo(FingerId.Thumb, JointId.KnuckleMajor, null, false),
                new JointInfo(FingerId.Thumb, JointId.KnuckleMid,   null, false),
                new JointInfo(FingerId.Thumb, JointId.KnuckleMinor, null, false),

                new JointInfo(FingerId.Index, JointId.Root,         null, false),
                new JointInfo(FingerId.Index, JointId.KnuckleMajor, null, false),
                new JointInfo(FingerId.Index, JointId.KnuckleMid,   null, false),
                new JointInfo(FingerId.Index, JointId.KnuckleMinor, null, false),

                new JointInfo(FingerId.Middle, JointId.Root,        null, false),
                new JointInfo(FingerId.Middle, JointId.KnuckleMajor,null, false),
                new JointInfo(FingerId.Middle, JointId.KnuckleMid,  null, false),
                new JointInfo(FingerId.Middle, JointId.KnuckleMinor,null, false),

                new JointInfo(FingerId.Ring, JointId.Root,         null, false),
                new JointInfo(FingerId.Ring, JointId.KnuckleMajor, null, false),
                new JointInfo(FingerId.Ring, JointId.KnuckleMid,   null, false),
                new JointInfo(FingerId.Ring, JointId.KnuckleMinor, null, false),

                new JointInfo(FingerId.Little, JointId.Root,        null, false),
                new JointInfo(FingerId.Little, JointId.KnuckleMajor,null, false),
                new JointInfo(FingerId.Little, JointId.KnuckleMid,  null, false),
                new JointInfo(FingerId.Little, JointId.KnuckleMinor,null, false) };

            //adding the rig bones to the ointinfo class
            foreach (JointInfo j in jointInfo)
            {
                if (j.rootBone)
                {
                    string jointName = "Hand.Wrist." + whichHand.ToString();
                    j.node = handModel.FindNode(jointName);

                }
                else
                {
                    string jointName = j.finger.ToString() + "." + j.joint.ToString() + "." + whichHand.ToString();
                    j.node = handModel.FindNode(jointName);
                }

            }

        }

        // Display a hand with the provided joint information
        public void show(RecordingData[] data)
        {
            if (data.Length != 26)
            {
                throw new Exception("Expected exactly 26 joints");
            }
            foreach (var j in jointInfo)
            {
                //rootbone is the wrist
                if (j.rootBone)
                {

                    Pose joint = data[data.Length-1].pose;
                    j.node.ModelTransform = Matrix.TRS(joint.position, joint.orientation * defaultBoneRot, nodeScale);

                }
                // all other fingers and joints
                else 
                {
                    Pose joint = GetJoint(data, j.finger, j.joint).pose;
                    j.node.ModelTransform = Matrix.TRS(joint.position, joint.orientation * defaultBoneRot, nodeScale);
                }
            }
            

            handModel.Draw(Matrix.Identity);
        }

        // Helper function for identifying index of specific joint inside of an array
        private RecordingData GetJoint(RecordingData[] poses, FingerId finger, JointId joint)
        {
            // As documented on stereokit
            return poses[5 * (int)finger + (int)joint];
        }

    }
}
