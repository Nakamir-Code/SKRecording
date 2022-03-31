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

        // This is used on the sending side, meanong we get the poses relative to worldspace and need to convert them to our anchor
        protected Pose[] getCurrentPoses(Matrix worldAnchorTRS)
        {
            Matrix inversedWorldAnchor = worldAnchorTRS.Inverse;
            for( int i = 0; i<recorders.Length; i++)
            {
                Pose[] poses = recorders[i].getCurrentFrame();
                for(int j =0; j<poses.Length; j++)
                {
                    poseAggregator[i + j] = (inversedWorldAnchor * poses[j].ToMatrix()).Pose;
                }
            }

            return poseAggregator;
        }
        // This is used on the receiving side, meaning we get the poses relative to the anchor and need to convert them to worldspace
        protected void displayPoses(Pose[] poses, Matrix worldAnchorTRS)
        {
            int i = 0;
            foreach (Recorder r in recorders)
            {
                for (int j = 0; j < poses.Length; j++)
                {
                    Matrix WorldPoseTRS = worldAnchorTRS * poses[j].ToMatrix();
                    poses[j] = WorldPoseTRS.Pose;
                }
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
        public abstract bool PlaybackOneFrame(Matrix anchorTRS);
        public abstract void RecordOneFrame(Matrix anchorTRS);
        public abstract bool hasRecording();

    }
}
