using StereoKit;
using System;

namespace SKRecording
{
    // Abstract class for managing multiple recorders at once. The main detail that is up to
    // child classes to inherit is how they store and fetch the recorded data.
    abstract class RecordingAggregator
    {

        // Recorders this instance keeps track off
        private Recorder[] recorders;
        // Buffer for temporarily storing one frame of data before the child class decides how to permanently store it
        private RecordingData[] recordingDataAggregator;
        

        protected RecordingAggregator(Recorder[] recorders)
        {
            this.recorders = recorders;
            recordingDataAggregator = new RecordingData[getObjectCount()];
        }

        // Returns an array representing the data captured in the current frame relative to the provided anchor.
        protected RecordingData[] getCurrentRecordingData(Matrix worldAnchorTRS)
        {
            // Check if some objects were deleted/added and adjust buffer size accordingly
            if(recordingDataAggregator.Length != getObjectCount())
            {
                Array.Resize(ref recordingDataAggregator, getObjectCount());
            }
            int recAggIndex = 0;

            // We get the poses relative to worldspace and need to convert them to being relative to our anchor
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

        // Returns an array where each index is the amount of objects each recorder at that index is tracking 
        protected int[] getCurrentParamLengths()
        {
            int[] pLengths = new int[recorders.Length];
            for(int i = 0; i<recorders.Length; i++)
            {
                pLengths[i] = recorders[i].getObjectCount();
            }

            return pLengths;

        }
        // Displays the objects its recorders are tracking using the provided RecordingData which expected to be relative to the provided anchor.
        protected void displayAll(RecordingData[] recorderDatas, int[] paramLengths, Matrix worldAnchorTRS)
        {
            int recAggIndex = 0;

            // We get the poses relative to the anchor and need to convert them to worldspace.
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

        // Returns how many total objects this aggregator's recorders track
        protected int getObjectCount()
        {
            int objectCount = 0;
            foreach (Recorder r in recorders)
            {
                objectCount += r.getObjectCount();
            }
            return objectCount;
        }

        // To be called after recording is finished in case the child needs any extra cleanup
        public virtual void finishRecording()
        { 
            return;
        }

        // To be called after playback is finished in case the child needs any extra cleanup
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
