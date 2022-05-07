using StereoKit;

namespace SKRecording
{
    // Recorder to track one hand
    class HandRecorder : IRecorder
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

        // Recording data is returned for every joint (26 joints) in the hand
        public Label3D[] getCurrentFrame()
        {
            return handJointsToRecordingDataArray(Input.Hand(whichHand).fingers, whichHand);
        }

        // Expects a RecordingData array of 26 handjoint poses and displays it in the form of the tracked Handmodel 
        public void displayFrame(Label3D[] data)
        {
            handModel.show(data);
        }

        // Each hand tracks 26 joints
        public int getObjectCount()
        {
            return Constants.Count;
        }


        // Helper function for merging all handjoint poses into one RecordingData array 
        private static Label3D[] handJointsToRecordingDataArray(HandJoint[] joints, Handed whichHand)
        {
            Label3D[] recordingData = new Label3D[joints.Length+1];
            
            for (int i = 0; i < joints.Length; i++)
            {
                recordingData[i] = new Label3D(new Pose(joints[i].position, joints[i].orientation));
            }
            recordingData[joints.Length] = new Label3D(Input.Hand(whichHand).wrist);
            return recordingData;
        }
    }
}