using SuperSimpleTcp;
using System;

namespace SKRecording
{
    // Super class that contains functionality shared by TCPClient and TCPServer
    abstract class TCPPeer
    {

        // Counts how many frames were received
        protected int receivedCount;
        // Intermediate buffer for saving cut-off frame
        private string incompleteFrame = null;
        // Symbol indicating the start of a frame (e.g. { in JSON)
        protected char startSymbol;
        // Symbol indicating the end of a frame (e.g. } in JSON)
        protected char endSymbol;
        // Symbol seperating two frames (e.g. ; in our protocl )
        protected char seperatorSymbol;

        public TCPPeer(char startSymbol, char endSymbol, char seperatorSymbol)
        {
            this.startSymbol = startSymbol;
            this.endSymbol = endSymbol;
            this.seperatorSymbol = seperatorSymbol;
            receivedCount = 0;
        }

        // Let children handle if they want
        protected virtual void OnConnected(object sender, ConnectionEventArgs e)
        {
            return;
        }

        // Let children handle if they want
        protected virtual void OnDisconnected(object sender, ConnectionEventArgs e)
        {
            return;
        }

        // Called when data is received by this peer
        protected void OnDataReceived(object sender, DataReceivedEventArgs e)
        {
            // Prepare e into a sequence of packet strings to be parsed
            string data = preParseData(e);
            // Split the sequence into individual frames
            string[] frames = data.Split(seperatorSymbol);
            // Parse the individual frames into the format expected to continue working with them
            string[] result = parseFrames(frames);
            receivedCount += frames.Length;
            // Pass the now parsed data forward
            OnPacketDecoded(result);
        }

        // Frame parsing logic that handles such things as e.g. incomplete frames
        private string[] parseFrames(string[] frames)
        {
            // First and last frames might be cutoff, check
            int start = 0;

            // Empty string? Skip
            if (frames[0].Length == 0)
            {
                if (frames.Length == 1)
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

            // Check if we perfectly parsed all frames or if we have some cutoff
            if (end == frames.Length && start == 0)
            {
                // Perfectly parsed
                return frames;
            }
            
            // Handle cutoff case and only forward the complete frames
            string[] result = new string[end - start];
            Array.Copy(frames, start, result, 0, result.Length);
            return result;
        }

        public virtual void reset()
        {
            receivedCount = 0;
        }

        // To be implemented by children
        public abstract void send(string toSend, bool raw = false);
        public abstract void connect();
        public abstract void disconnect();

        // To be overriden by children in case data needs to be further modified before its parsed
        protected virtual string preParseData(DataReceivedEventArgs e)
        {
            return System.Text.Encoding.UTF8.GetString(e.Data);
        }

        // Logic to implement the event for a decoded frame
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
