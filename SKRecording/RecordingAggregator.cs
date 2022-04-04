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
            recordingDataAggregator = new RecordingData[getObjectCount()];

        }

        // This is used on the sending side, meanong we get the poses relative to worldspace and need to convert them to our anchor
        protected RecordingData[] getCurrentRecordingData(Matrix worldAnchorTRS)
        {
            if(recordingDataAggregator.Length != getObjectCount())
            {
                Array.Resize(ref recordingDataAggregator, getObjectCount());
            }
            int recAggIndex = 0;

            Matrix inversedWorldAnchor = worldAnchorTRS.Inverse;
            for( int i = 0; i<recorders.Length; i++)
            {
                RecordingData[] recordingData = recorders[i].getCurrentFrame();
                for(int j =0; j<recordingData.Length; j++)
                {
                    // The correct multiplication is inversedWorldAnchor * pose. However, matrix multiplication in C# is inverted, meaning if we want to 
                    // do  A * B, what we write in C# is B*A. More info here: https://stackoverflow.com/questions/58712092/matrix-struct-gives-wrong-output
                    recordingDataAggregator[recAggIndex] = recordingData[j].clone();
                    recordingDataAggregator[recAggIndex].pose = (recordingData[j].pose.ToMatrix() * inversedWorldAnchor).Pose;
                    recAggIndex++;
                }
            }

            return recordingDataAggregator;
        }

        protected int[] getCurrentParamLengths()
        {
            int[] pLengths = new int[recorders.Length];
            for(int i = 0; i<recorders.Length; i++)
            {
                pLengths[i] = recorders[i].getObjectCount();
            }

            return pLengths;

        }
        // This is used on the receiving side, meaning we get the poses relative to the anchor and need to convert them to worldspace
        protected void displayAll(RecordingData[] recorderDatas, int[] paramLengths, Matrix worldAnchorTRS)
        {
            int recAggIndex = 0;

            for (int i = 0; i<recorders.Length; i++)
            {
                RecordingData[] toDisplay = new RecordingData[paramLengths[i]];
                for (int j = 0; j < toDisplay.Length; j++)
                {
                    // The correct multiplication is worldAnchorTRS * pose. However, matrix multiplication in C# is inverted, meaning if we want to 
                    // do  A * B, what we write in C# is B*A. More info here: https://stackoverflow.com/questions/58712092/matrix-struct-gives-wrong-output
                    toDisplay[j] = recorderDatas[recAggIndex];
                    toDisplay[j].pose = (recorderDatas[recAggIndex].pose.ToMatrix() * worldAnchorTRS).Pose;
                    recAggIndex++;
                }
                recorders[i].displayFrame(toDisplay);
            }
        }

        protected int getObjectCount()
        {
            int objectCount = 0;
            foreach (Recorder r in recorders)
            {
                objectCount += r.getObjectCount();
            }
            return objectCount;
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
