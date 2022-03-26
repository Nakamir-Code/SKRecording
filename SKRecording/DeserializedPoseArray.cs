using StereoKit;

namespace SKRecording
{
    public class DeserializedPoseArray
    {
        public int count;
        public float[][] Quats { get; set; }
        public float[][] Tvecs { get; set; }


        public Pose[] toPoseArray()
        {

            Pose[] result = new Pose[this.count];

            for (int i = 0; i < result.Length; i++)
            {
                result[i] = new Pose(
                    new System.Numerics.Vector3(this.Tvecs[i][0], this.Tvecs[i][1], this.Tvecs[i][2]),
                    new System.Numerics.Quaternion(this.Quats[i][0], this.Quats[i][1], this.Quats[i][2], this.Quats[i][3]));
            }

            return result;

        }

        public static DeserializedPoseArray fromPoseArray(Pose[] poses)
        {
            float[][] orientations = new float[poses.Length][];
            float[][] positions = new float[poses.Length][];

            for (int i = 0; i < poses.Length; i++)
            {
                float[] orientationArray = { poses[i].orientation.q.X, poses[i].orientation.q.Y, poses[i].orientation.q.Z, poses[i].orientation.q.W };
                orientations[i] = orientationArray;
                float[] positionArray = { poses[i].position.v.X, poses[i].position.v.Y, poses[i].position.v.Z };
                positions[i] = positionArray;
            }

            DeserializedPoseArray deserializedPoseArray = new DeserializedPoseArray();
            deserializedPoseArray.count = poses.Length;
            deserializedPoseArray.Quats = orientations;
            deserializedPoseArray.Tvecs = positions;
            return deserializedPoseArray;
        }

    }
}
