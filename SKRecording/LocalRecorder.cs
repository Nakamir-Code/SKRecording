using System;
using System.IO;
using System.Text;
using StereoKit;

namespace SKRecording
{
    // NOT WORKING
    class LocalRecorder : Recorder
    {
        private string destinationPath;
        public LocalRecorder(string path)
        {
            destinationPath = path;
        }

        public string GetDestinationPath()
        {
            return destinationPath;
        }

        public void SetDestinationPath(string path)
        {
            destinationPath = path;
        }

        public void RecordOneFrame(string content)
        {
            if(destinationPath == null)
            {
                throw new Exception("Need to set a path to record to");
            }

            using (var file = new FileStream(destinationPath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write))
            {
                using (var writer = new StreamWriter(file, Encoding.UTF8))
                {
                    writer.WriteLine(content);
                }
            }
        }
    }
}
