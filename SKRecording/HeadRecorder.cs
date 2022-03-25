using StereoKit;

namespace SKRecording
{
    class HeadRecorder
    {
        private DynamicLocalRecorderPlaybacker recorder;
        private JsonCoder coder;

        public HeadRecorder(string recPath = null)
        {
            recorder = new DynamicLocalRecorderPlaybacker(recPath);
            coder = new JsonCoder();
            VRHead.init();
        }

        public void setRecPath(string recPath)
        {
            recorder = new DynamicLocalRecorderPlaybacker(recPath);
        }

        public void recordHeadFrame(Pose head)
        {
            string recording = coder.Serialize(coder.poseToDeserializedPose(head));
            recorder.RecordOneFrame(recording);
        }

        public bool playbackHeadFrame()
        {
            string position = recorder.PlaybackOneFrame();
            if (position == null) return false;
            Pose pose = coder.deserializedPoseToPose(coder.Deserialize<DeserializedPose>(position));
            VRHead.showHead(pose);
            return true;
        }

        public bool hasRecording()
        {
            return recorder.hasRecording();
        }
    }
}
