using System;
using System.IO;

namespace SKRecording
{
    // NOT WORKING
    class LocalPlaybacker : Playbacker
    {
        private string sourcePath;
        
        public LocalPlaybacker(string path)
        {
            sourcePath = path;
        }

        public void SetSourcePath(string sourcePath)
        {
            this.sourcePath = sourcePath;
        }

        public string getSourcePath()
        {
            return sourcePath;
        }

        public string PlaybackOneFrame()
        {
            if (sourcePath == null || !File.Exists(sourcePath))
            {
                throw new Exception("Need to be an existing path to playback from");
            }

            using (FileStream fs = File.OpenRead(sourcePath))
            {
                using (StreamReader sr = new StreamReader(sourcePath))
                {
                    return sr.ReadLine();
                }
            }
        }

    }
}
