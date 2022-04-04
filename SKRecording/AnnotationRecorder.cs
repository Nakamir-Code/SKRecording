using StereoKit;
using System.Collections.Generic;

namespace SKRecording
{
    class AnnotationRecorder : Recorder
    {
        private List<RecordingData> annotations;

        public AnnotationRecorder(List<RecordingData> annotations)
        {
            this.annotations = annotations;
        }

        public void displayFrame(RecordingData[] data)
        {
            for(int i = 0; i<data.Length; i++)
            {
                Utils.showAnnotation(data[i]);
            }
        }

        public RecordingData[] getCurrentFrame()
        {
            return annotations.ToArray();
        }

        public int getObjectCount()
        {
            return annotations.Count;
        }    
    }
}
