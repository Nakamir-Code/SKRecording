using SuperSimpleTcp;
using System;

namespace SKRecording
{
    abstract class TCPClient
    {

        protected SimpleTcpClient client;
        protected int receivedCount;
        // Intermediate buffer for saving cut-off frame
        private string incompleteFrame = null;
        // Symbol indicating the start of a frame (e.g. { in JSON)
        private char startSymbol;
        // Symbol indicating the end of a frame (e.g. } in JSON)
        private char endSymbol;
        // Symbol seperating two frames (e.g. ; in our protocl )
        private char seperatorSymbol;

        public TCPClient(string ip, int port, char startSymbol, char endSymbol, char seperatorSymbol)
        {
            this.startSymbol = startSymbol;
            this.endSymbol = endSymbol;
            this.seperatorSymbol = seperatorSymbol;

            client = new SimpleTcpClient(ip + ":" + port);
            client.Events.DataReceived += OnDataReceived;
            receivedCount = 0;
        }

        public void start()
        {
            client.Connect();
        }

        private void OnDataReceived(object sender, DataReceivedEventArgs e)
        {
            string data = preParseData(e);
            string[] frames = data.Split(seperatorSymbol);
            string[] result = parseFrames(frames);
            receivedCount += frames.Length;
            OnPacketDecoded(result);
        }

        private string[] parseFrames(string[] frames)
        {
            // First and last frames might be cutoff, check
            int start = 0;

            // Empty string? Skip
            if (frames[0].Length == 0)
            {
                if(frames.Length == 1)
                {
                    return new string[0];
                }
                start = 1;
            }
            // Incomplete string? Start must have been end of last packet since we're using TCP and nothing gets lost
            else if (frames[0][0] != startSymbol)
            {
                frames[0] = incompleteFrame + frames[0];
                incompleteFrame = null;
            }

            // Last character of the last element in jsonStrs
            int end = frames.Length;
            string lastElement = frames[frames.Length - 1];

            // Again, skip empty strings
            if (lastElement.Length == 0)
            {
                end = frames.Length - 1;
            }
            // Incomplete? Rest will come in next packet.
            else if (lastElement[lastElement.Length - 1] != endSymbol)
            {
                incompleteFrame = lastElement;
                end = frames.Length - 1;
            }

            if (end == frames.Length && start == 0)
            {
                return frames;
            }

            string[] result = new string[end - start];
            Array.Copy(frames, start, result, 0, result.Length);
            return result;
        }

        public virtual void reset()
        {
            receivedCount = 0;
        }


        public void send(string toSend, bool raw = false)
        {
            if (raw)
            {
                client.Send(toSend);
                return;
            }
            client.Send(toSend + seperatorSymbol.ToString());

        }

        // To be overriden by children in case data needs to be modified before its parsed
        protected abstract string preParseData(DataReceivedEventArgs e);

        protected virtual void OnPacketDecoded(string[] e)
        {
            EventHandler<string[]> handler = decodedFrame;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public event EventHandler<string[]> decodedFrame;

    }
}
