using StereoKit;
using System.Collections.Generic;

namespace SKRecording
{
    // Recorder implementation for recording text-based annotations
    class AnnotationTrackerShower : IPoseTrackerShower
    {
        // List for saved annotations. 
        // Note: Developer is expected to manage (add and remove annotations) this list externally, 
        // not handled by this class
        private List<Label3D> _labels;

        public AnnotationTrackerShower(List<Label3D> labels)
        {
            this._labels = labels;
        }

        // Displays every annotation in the provided RecordingData array 
        public void displayFrame(Label3D[] labels)
        {
            for(int i = 0; i<labels.Length; i++)
            {
                Utils.showAnnotation(labels[i]);
            }
        }

        // Returns an array of all created annotations in the current frame 
        public Label3D[] getCurrentFrame()
        {
            return _labels.ToArray();
        }

        // Returns the amount of saved annotations
        public int getObjectCount()
        {
            return _labels.Count;
        }
    }
}
