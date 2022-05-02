﻿using StereoKit;

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
            return handJointsToRecordingDataArray(Input.Hand(whichHand).fingers, whichHand);
        }

        public void displayFrame(RecordingData[] data)
        {
            handModel.show(data);
        }

        public int getObjectCount()
        {
            return 26;
        }

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