using StereoKit;
using System.Collections.Generic;

namespace SKRecording
{
    // Recorder implementation for recording text-based annotations
    class AnnotationRecorder : IRecorder
    {
        // List for saved annotations. 
        // Note: Developer is expected to manage (add and remove annotations) this list externally, 
        // not handled by this class
        private List<Label3D> annotations;

        public AnnotationRecorder(List<Label3D> annotations)
        {
            this.annotations = annotations;
        }

        // Displays every annotation in the provided RecordingData array 
        public void displayFrame(Label3D[] data)
        {
            for(int i = 0; i<data.Length; i++)
            {
                Utils.showAnnotation(data[i]);
            }
        }

        // Returns an array of all created annotations in the current frame 
        public Label3D[] getCurrentFrame()
        {
            return annotations.ToArray();
        }

        // Returns the amount of saved annotations
        public int getObjectCount()
        {
            return annotations.Count;
        }    
    }
}
