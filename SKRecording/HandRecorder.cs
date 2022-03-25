using StereoKit;

namespace SKRecording
{
    class HandRecorder
    {
        private DynamicLocalRecorderPlaybacker recorder;
        private JsonCoder coder;
        private Handed whichHand;

        public HandRecorder(Handed whichHand, string recPath = null)
        {
            recorder = new DynamicLocalRecorderPlaybacker(recPath);
            coder = new JsonCoder();
            this.whichHand = whichHand;
            VRHands.init();
        }

        public void setRecPath(string recPath)
        {
            recorder = new DynamicLocalRecorderPlaybacker(recPath);
        }

        public void recordHandFrame(StereoKit.Hand hand)
        {
            string recording = coder.Serialize(coder.handJointsToDeserializedHandPose(hand.fingers));
            recorder.RecordOneFrame(recording);
        }

        public bool playbackHandFrame()
        {
            string position = recorder.PlaybackOneFrame();
            if (position == null) return false;
            HandJoint[] joints = coder.deserializedHandPoseToHandJoints(coder.Deserialize<DeserializedHandPose>(position));
            VRHands.ShowHand(whichHand, joints);
            return true;
        }

        public bool hasRecording()
        {
            return recorder.hasRecording();
        }
    }
}
