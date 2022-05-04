using StereoKit;

namespace SKRecording
{
    // Recorder to track one hand
    class HandRecorder : Recorder
    {
        // Handmodel to display
        private ModelWrapper handModel;
        // Which hand are we tracking (right or left)
        private Handed whichHand;

        public HandRecorder(Handed whichHand)
        {
            handModel = new VRHand(whichHand);
            this.whichHand = whichHand;
        }

        // Recording data is returned for every joint (25 joints) in the hand
        public RecordingData[] getCurrentFrame()
        {
            return handJointsToRecordingDataArray(Input.Hand(whichHand).fingers, whichHand);
        }

        // Expects a RecordingData array of 25 handjoint poses and displays it in the form of the tracked Handmodel 
        public void displayFrame(RecordingData[] data)
        {
            handModel.show(data);
        }

        // Each hand tracks 25 joints
        public int getObjectCount()
        {
            return 26;
        }


        // Helper function for merging all handjoint poses into one RecordingData array 
        private static RecordingData[] handJointsToRecordingDataArray(HandJoint[] joints, Handed whichHand)
        {
            RecordingData[] recordingData = new RecordingData[joints.Length+1];
            
            for (int i = 0; i < joints.Length; i++)
            {
                recordingData[i] = new RecordingData(new Pose(joints[i].position, joints[i].orientation));
            }
            recordingData[joints.Length] = new RecordingData(Input.Hand(whichHand).wrist);
            return recordingData;
        }
    }
}