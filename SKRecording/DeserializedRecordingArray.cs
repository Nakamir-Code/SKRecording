using StereoKit;

namespace SKRecording
{
    // Intermediate class for converting between RecordingData arrays and JSON strings
    public class DeserializedRecordingArray
    {
        // Tracks the length of each Recorder we're tracking.
        // E.g. We have a HeadRecorder and an AnnotationRecorder with 4 annotations. 
        // Then, this value would be [1,4], as we are tracking 1 head and 4 annotations.
        public int[] paramLenghts;
        // Orientation of each object we're tracking
        public float[][] Quats { get; set; }
        // Position of each object we're tracking
        public float[][] Tvecs { get; set; }
        // Text associated with each object we're tracking (currently only used by AnnotationRecorder)
        public string[] texts { get; set; }

        // Converts from this DeserializedRecordingArray to an array of RecordingData
        public Label3D[] toRecordingDataArray()
        {

            // RecordingData arrays simply concatenate the RecordingData for the seperate objects behind each other
            Label3D[] result = new Label3D[Utils.sum(paramLenghts)];

            for (int i = 0; i < result.Length; i++)
            {
                result[i] = new Label3D(new Pose(
                    new System.Numerics.Vector3(this.Tvecs[i][0], this.Tvecs[i][1], this.Tvecs[i][2]),
                    new System.Numerics.Quaternion(this.Quats[i][0], this.Quats[i][1], this.Quats[i][2], this.Quats[i][3])),
                    this.texts[i]);
            }

            return result;

        }

        public int[] getParamLengths()
        {
            return paramLenghts;
        }

        // Converts an array of RecordingData to a DeserializedRecordingArray
        public static DeserializedRecordingArray fromRecordingDataArray(Label3D[] recordingData, int[] paramLengths)
        {
            float[][] orientations = new float[recordingData.Length][];
            float[][] positions = new float[recordingData.Length][];
            string[] texts = new string[recordingData.Length];

            for (int i = 0; i < recordingData.Length; i++)
            {
                float[] orientationArray = { recordingData[i].pose.orientation.q.X, recordingData[i].pose.orientation.q.Y, recordingData[i].pose.orientation.q.Z, recordingData[i].pose.orientation.q.W };
                orientations[i] = orientationArray;
                float[] positionArray = { recordingData[i].pose.position.v.X, recordingData[i].pose.position.v.Y, recordingData[i].pose.position.v.Z };
                positions[i] = positionArray;

                texts[i] = recordingData[i].text;

            }

            DeserializedRecordingArray deserializedRecordingArray = new DeserializedRecordingArray();
            deserializedRecordingArray.paramLenghts = paramLengths;
            deserializedRecordingArray.Quats = orientations;
            deserializedRecordingArray.Tvecs = positions;
            deserializedRecordingArray.texts = texts;
            return deserializedRecordingArray;
        }

    }
}
