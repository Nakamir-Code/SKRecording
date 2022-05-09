using StereoKit;

namespace SKRecording
{
    // Recorder to track one hand
    class HandTrackerShower : IPoseTrackerShower
    {
        // Handmodel to display
        private ModelWrapper _handModel;

        // Which hand are we tracking (right or left)
        private Handed _whichHand;

        public HandTrackerShower(Handed whichHand)
        {
            _handModel = new VRHand(whichHand);
            _whichHand = whichHand;
        }

        // Recording data is returned for every joint (26 joints) in the hand
        public Label3D[] getCurrentFrame()
        {
            return handJointsToRecordingDataArray(Input.Hand(_whichHand).fingers, _whichHand);
        }

        // Expects a RecordingData array of 26 handjoint poses and displays it in the form of the tracked Handmodel 
        public void displayFrame(Label3D[] data)
        {
            _handModel.show(data);
        }

        // Each hand tracks 26 joints
        public int getObjectCount()
        {
            return Constants.Count;  // TODO: returning constants doesn't make this class reusable
        }


        // Helper function for merging all handjoint poses into one RecordingData array 
        private static Label3D[] handJointsToRecordingDataArray(HandJoint[] joints, Handed whichHand)
        {
            Label3D[] labels = new Label3D[joints.Length+1];
            
            for (int i = 0; i < joints.Length; i++)
            {
                labels[i] = new Label3D(new Pose(joints[i].position, joints[i].orientation));
            }
            labels[joints.Length] = new Label3D(Input.Hand(whichHand).wrist);  // TODO: why not reuse joints? Calling Input.Hands() is expenseive as
            // mentioned in its docstring.
            
            return labels;
        }
    }
}