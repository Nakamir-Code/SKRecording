using System.Collections.Generic;
using StereoKit;

namespace SKRecording
{
    // DON'T PASS A PATH TO THIS, RECORDING TO DISC DOESN'T WORK YET
    class DynamicLocalRecorderPlaybacker : Recorder, Playbacker
    {
        Queue<string> recording = null;
        LocalRecorder recorder;
        LocalPlaybacker playbacker;

        public DynamicLocalRecorderPlaybacker(string recordingPath = null)
        {
            // If user doesn't specify a recording path, save to memory
            if(recordingPath == null)
            {
                recording = new Queue<string>();
            }
            else
            {
                recorder = new LocalRecorder(recordingPath);
                playbacker = new LocalPlaybacker(recordingPath);
            }

        }

        public void RecordOneFrame(string content)
        {
            if(recording != null)
            {
                recording.Enqueue(content);
            }
            else
            {
                recorder.RecordOneFrame(content);
            }
        }

        public string PlaybackOneFrame()
        {
            if (recording != null)
            {
                if (recording.Count == 0) return null;
                return recording.Dequeue();
            }
            else
            {
                return playbacker.PlaybackOneFrame();
            }
        }

        public bool hasRecording()
        {
            if(recording != null)
            {
                return recording.Count != 0;
            }
            return playbacker != null;
        }

    }
}
