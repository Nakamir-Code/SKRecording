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

        public RecordingData[] getCurrentFrame()
        {
            return handJointsToRecordingDataArray(Input.Hand(whichHand).fingers);
        }

        public void displayFrame(RecordingData[] data)
        {
            handModel.show(data);
        }

        public int getObjectCount()
        {
            return 25;
        }

        private static RecordingData[] handJointsToRecordingDataArray(HandJoint[] joints)
        {
            RecordingData[] recordingData = new RecordingData[joints.Length];

            for (int i = 0; i < joints.Length; i++)
            {
                recordingData[i] = new RecordingData(new Pose(joints[i].position, joints[i].orientation));
            }
            return recordingData;
        }
    }
}