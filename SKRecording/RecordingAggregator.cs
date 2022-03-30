using StereoKit;
using System;

namespace SKRecording
{
    abstract class RecordingAggregator
    {

        private Recorder[] recorders;
        private Pose[] poseAggregator;
        

        protected RecordingAggregator(Recorder[] recorders)
        {
            this.recorders = recorders;
            int poseCount = 0;
            foreach (Recorder r in recorders)
            {
                poseCount += r.getPoseCount();
            }

            poseAggregator = new Pose[poseCount];

        }

        protected Pose[] getCurrentPoses()
        {
            int i = 0;
            foreach (Recorder r in recorders)
            {
                r.getCurrentFrame().CopyTo(poseAggregator, i);
                i += +r.getPoseCount();
            }

            return poseAggregator;
        }

        protected void displayPoses(Pose[] poses)
        {
            int i = 0;
            foreach (Recorder r in recorders)
            {
                r.displayFrame(new ArraySegment<Pose>(poses, i, r.getPoseCount()).ToArray());
                i += r.getPoseCount();
            }
        }

        // To be called after recording is finished
        public virtual void finishRecording()
        { 
            return;
        }

        // To be called after playback is finished
        public virtual void finishPlayback()
        {
            return;
        }


        // To be implemented by inheritors
        public abstract bool PlaybackOneFrame();
        public abstract void RecordOneFrame();
        public abstract bool hasRecording();

    }
}
