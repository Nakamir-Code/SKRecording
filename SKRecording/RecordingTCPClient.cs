using SuperSimpleTcp;
using System;

namespace SKRecording
{
    class RecordingTCPClient : TCPClient
    {
        private int totalFrameCount = -1;

        public RecordingTCPClient(string ip, int port) : base(ip, port, '{', '}', ';')
        {
            totalFrameCount = 0;
        }

        protected override string preParseData(DataReceivedEventArgs e)
        {
            byte[] a = e.Data;
            int start = 0;
            // First thing that is sent is the length marked with an L (76 in UTF-8)
            if (a[0] == 76)
            {
                totalFrameCount = BitConverter.ToInt32(a, 1);
                // 1 byte for the L char, 4 bytes for the totalframecount means we only start at byte 5
                start = 5;
            }

            return System.Text.Encoding.UTF8.GetString(a, start, a.Length - start);
        }

        public override void reset()
        {
            disconnect();
            receivedCount = 0;
            totalFrameCount = -1;
        }

        public bool hasReceivedAll()
        {
            return totalFrameCount != -1 && receivedCount >= totalFrameCount;
        }

    }
}
