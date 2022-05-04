using SuperSimpleTcp;
using System;

namespace SKRecording
{
    // TCP Client for receiving a fixed length recording from a server.
    class RecordingTCPClient : TCPClient
    {
        // To be sent by server - how many frames the recording contains
        private int totalFrameCount = -1;

        public RecordingTCPClient(string ip, int port) : base(ip, port, '{', '}', ';')
        {
        }

        // We expect the first chunk of data to be an 'L' followed by how many frames to expect. 
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

            // Parse the rest of the packet in the standard way
            return System.Text.Encoding.UTF8.GetString(a, start, a.Length - start);
        }
        
        // Resetting disconnects the client and resets the frame counter variables
        public override void reset()
        {
            disconnect();
            receivedCount = 0;
            totalFrameCount = -1;
        }

        // Check if we have received all frames of the recording
        public bool hasReceivedAll()
        {
            return totalFrameCount != -1 && receivedCount >= totalFrameCount;
        }

    }
}
