using Newtonsoft.Json;
using System.Collections.Generic;

namespace SKRecording
{
    public class JsonCoder
    {

        public JsonCoder()
        {
        }
        public T Deserialize<T>(string jsonStr)
        {
            // Get a single json string in case multiple are concatenated e.g. {}{}{}
            var endOfStr = jsonStr.IndexOf("}");
            jsonStr = jsonStr.Substring(0, endOfStr + 1);

            // Deserialize into desired class object
            return JsonConvert.DeserializeObject<T>(jsonStr);
        }

        public string Serialize<T>(T param)
        {
            return JsonConvert.SerializeObject(param);
        }

        public StereoKit.HandJoint[] deserializedHandPoseToHandJoints(DeserializedHandPose deserializedHandPose)
        {
            StereoKit.HandJoint[] joints = new StereoKit.HandJoint[25];

            for (int i = 0; i < 25; i++)
            {
                joints[i] = new StereoKit.HandJoint(
                    new System.Numerics.Vector3(deserializedHandPose.Tvecs[i][0], deserializedHandPose.Tvecs[i][1], deserializedHandPose.Tvecs[i][2]),
                    new System.Numerics.Quaternion(deserializedHandPose.Quats[i][0], deserializedHandPose.Quats[i][1], deserializedHandPose.Quats[i][2], deserializedHandPose.Quats[i][3]),
                    deserializedHandPose.Radii[i]);
            }

            return joints;

        }

        public DeserializedHandPose handJointsToDeserializedHandPose(StereoKit.HandJoint[] joints)
        {
            float[][] orientations = new float[25][];
            float[][] positions = new float[25][];
            float[] radii = new float[25];

            for(int i =0; i<25; i++)
            {
                float[] orientationArray = { joints[i].orientation.q.X, joints[i].orientation.q.Y, joints[i].orientation.q.Z, joints[i].orientation.q.W };
                orientations[i] = orientationArray;
                float[] positionArray = { joints[i].position.v.X, joints[i].position.v.Y, joints[i].position.v.Z};
                positions[i] = positionArray;
                radii[i] = joints[i].radius;
            }

            DeserializedHandPose deserializedHandPose = new DeserializedHandPose();
            deserializedHandPose.Quats = orientations;
            deserializedHandPose.Tvecs = positions;
            deserializedHandPose.Radii = radii;
            return deserializedHandPose;
        }


    }
}