using StereoKit;
namespace SKRecording
{
    // Intermediate class for converting between Poses and JSON strings
    public class DeserializedPose
    {
        // Orientation
        public float[] Quats { get; set; }
        // Position
        public float[] Tvecs { get; set; }

        // Converts a Pose to a DeserializedPose
        public static DeserializedPose fromPose(Pose pose)
        {
            DeserializedPose deserializedPose = new DeserializedPose();
            float[] orientationArray = { pose.orientation.q.X, pose.orientation.q.Y, pose.orientation.q.Z, pose.orientation.q.W };
            deserializedPose.Quats = orientationArray;
            float[] positionArray = { pose.position.v.X, pose.position.v.Y, pose.position.v.Z };
            deserializedPose.Tvecs = positionArray;
            return deserializedPose;
        }

        // Converts this instance of a DeserializedPose to a Pose
        public Pose toPose()
        {
            return new Pose(
                new System.Numerics.Vector3(this.Tvecs[0], this.Tvecs[1], this.Tvecs[2]),
                new System.Numerics.Quaternion(this.Quats[0], this.Quats[1], this.Quats[2], this.Quats[3]));
        }

    }
}
