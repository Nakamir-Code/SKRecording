using StereoKit;
using System;

namespace SKRecording
{
    // Abstract class for managing multiple recorders at once. The main detail that is up to
    // child classes to inherit is how they store and fetch the recorded data.
    abstract class RecordingAggregator
    {
        // Recorders this instance keeps track off
        private IPoseTrackerShower[] _poseTrackerShowerList;
        // Buffer for temporarily storing one frame of data before the child class decides how to permanently store it
        private Label3D[] _labels;
        
        protected RecordingAggregator(IPoseTrackerShower[] recorders)
        {
            this._poseTrackerShowerList = recorders;
            _labels = new Label3D[getObjectCount()];
        }

        // Returns an array representing the data captured in the current frame relative to the provided anchor.
        protected Label3D[] getCurrentRecordingData(Matrix worldAnchorTRS)
        {
            // Check if some objects were deleted/added and adjust buffer size accordingly
            if(_labels.Length != getObjectCount())
            {
                Array.Resize(ref _labels, getObjectCount()); // TODO: why not just use list?
            }
            int recAggIndex = 0;

            // We get the poses relative to worldspace and need to convert them to being relative to our anchor // TODO: move this to the server side
            Matrix inversedWorldAnchor = worldAnchorTRS.Inverse;
            
            for( int i = 0; i<_poseTrackerShowerList.Length; i++)
            {
                Label3D[] recordingData = _poseTrackerShowerList[i].getCurrentFrame();
                for(int j =0; j<recordingData.Length; j++)
                {
                    // The correct multiplication is inversedWorldAnchor * pose. However, matrix multiplication in C# is inverted, meaning if we want to 
                    // do  A * B, what we write in C# is B*A. More info here: https://stackoverflow.com/questions/58712092/matrix-struct-gives-wrong-output
                    _labels[recAggIndex] = recordingData[j].clone();  // TODO: This is expensive!
                    _labels[recAggIndex].pose = (recordingData[j].pose.ToMatrix() * inversedWorldAnchor).Pose;
                    recAggIndex++;
                }
            }

            return _labels;
        }

        // Returns an array where each index is the amount of objects each recorder at that index is tracking 
        protected int[] getCurrentParamLengths()
        {
            int[] pLengths = new int[_poseTrackerShowerList.Length];
            for(int i = 0; i<_poseTrackerShowerList.Length; i++)
            {
                pLengths[i] = _poseTrackerShowerList[i].getObjectCount();
            }

            return pLengths;

        }
        
        // Displays the objects its recorders are tracking using the provided RecordingData which expected to be relative to the provided anchor.
        protected void displayAll(Label3D[] recorderDatas, int[] paramLengths, Matrix worldAnchorTRS)
        {
            int recAggIndex = 0;

            // We get the poses relative to the anchor and need to convert them to worldspace.
            for (int i = 0; i<_poseTrackerShowerList.Length; i++)
            {
                Label3D[] toDisplay = new Label3D[paramLengths[i]];
                for (int j = 0; j < toDisplay.Length; j++)
                {
                    // The correct multiplication is worldAnchorTRS * pose. However, matrix multiplication in C# is inverted, meaning if we want to 
                    // do  A * B, what we write in C# is B*A. More info here: https://stackoverflow.com/questions/58712092/matrix-struct-gives-wrong-output
                    toDisplay[j] = recorderDatas[recAggIndex];
                    toDisplay[j].pose = (recorderDatas[recAggIndex].pose.ToMatrix() * worldAnchorTRS).Pose;
                    recAggIndex++;
                }
                _poseTrackerShowerList[i].displayFrame(toDisplay);
            }
        }

        // Returns how many total objects this aggregator's recorders track
        protected int getObjectCount()
        {
            int objectCount = 0;
            foreach (IPoseTrackerShower r in _poseTrackerShowerList)
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
        public abstract void StreamOneFrame(Matrix anchorTRS);
        public abstract bool hasRecording();

    }
}
