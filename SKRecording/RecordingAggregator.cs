using StereoKit;
using System;

namespace SKRecording
{
    abstract class RecordingAggregator
    {

        private Recorder[] recorders;
        private RecordingData[] recordingDataAggregator;
        

        protected RecordingAggregator(Recorder[] recorders)
        {
            this.recorders = recorders;
            int poseCount = 0;
            foreach (Recorder r in recorders)
            {
                poseCount += r.getObjectCount();
            }

            recordingDataAggregator = new RecordingData[poseCount];

        }

        // This is used on the sending side, meanong we get the poses relative to worldspace and need to convert them to our anchor
        protected RecordingData[] getCurrentRecordingData(Matrix worldAnchorTRS)
        {
            Matrix inversedWorldAnchor = worldAnchorTRS.Inverse;
            for( int i = 0; i<recorders.Length; i++)
            {
                RecordingData[] recordingData = recorders[i].getCurrentFrame();
                for(int j =0; j<recordingData.Length; j++)
                {
                    // The correct multiplication is inversedWorldAnchor * pose. However, matrix multiplication in C# is inverted, meaning if we want to 
                    // do  A * B, what we write in C# is B*A. More info here: https://stackoverflow.com/questions/58712092/matrix-struct-gives-wrong-output
                    recordingDataAggregator[i + j] = recordingData[j];
                    recordingDataAggregator[i + j].pose = (recordingData[j].pose.ToMatrix() * inversedWorldAnchor).Pose;
                }
            }

            return recordingDataAggregator;
        }
        // This is used on the receiving side, meaning we get the poses relative to the anchor and need to convert them to worldspace
        protected void displayAll(RecordingData[] recorderDatas, Matrix worldAnchorTRS)
        {
            for (int i = 0; i<recorders.Length; i++)
            {
                RecordingData[] toDisplay = new RecordingData[recorders[i].getObjectCount()];
                for (int j = 0; j < toDisplay.Length; j++)
                {
                    // The correct multiplication is worldAnchorTRS * pose. However, matrix multiplication in C# is inverted, meaning if we want to 
                    // do  A * B, what we write in C# is B*A. More info here: https://stackoverflow.com/questions/58712092/matrix-struct-gives-wrong-output
                    toDisplay[j] = recorderDatas[i + j];
                    toDisplay[j].pose = (recorderDatas[i + j].pose.ToMatrix() * worldAnchorTRS).Pose;
                }
                recorders[i].displayFrame(toDisplay);
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
