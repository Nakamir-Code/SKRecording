using StereoKit;

namespace SKRecording
{
    public class DeserializedRecordingArray
    {
        public int[] paramLenghts;
        public float[][] Quats { get; set; }
        public float[][] Tvecs { get; set; }
        public string[] texts { get; set; }


        public RecordingData[] toRecordingDataArray()
        {

            RecordingData[] result = new RecordingData[Utils.sum(paramLenghts)];

            for (int i = 0; i < result.Length; i++)
            {
                result[i] = new RecordingData(new Pose(
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

        public static DeserializedRecordingArray fromRecordingDataArray(RecordingData[] recordingData, int[] paramLengths)
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
