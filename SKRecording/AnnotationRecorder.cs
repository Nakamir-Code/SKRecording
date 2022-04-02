using StereoKit;
using System.Collections.Generic;

namespace SKRecording
{
    struct Annotation
    {
        public Pose pose;
        public string text;

        public Annotation(Pose pose, string text)
        {
            this.pose = pose;
            this.text = text;
        }
    }

    class AnnotationRecorder : Recorder
    {
        List<string> texts;
        List<Pose> poses;

        public AnnotationRecorder()
        {
            texts = new List<string>();
            poses = new List<Pose>();
        }

        public void displayFrame(RecordingData[] data)
        {
            int i = 0;
            foreach(string text in texts)
            {
                UI.WindowBegin("", ref data[i].pose, UIWin.Body);
                UI.Label(data[i].text);
                UI.WindowEnd();
                i++;
            }

        }

        public RecordingData[] getCurrentFrame()
        {
            throw new System.NotImplementedException();
        }

        public int getObjectCount()
        {
            throw new System.NotImplementedException();
        }
    }
}
