using System;
using System.Collections.Generic;
using StereoKit;

namespace SKRecording
{
    // Recording aggregator that stores data to a server.
    // NOTE: In fact, this aggregator only sends data to a server, but does not check wether the server also saves it. 
    // I.e., this class can also be used for streaming if the receiver is a Hololens streaming the data rather than storing it.
    // TODO: This class has the power to stream data, save it in a server, and playback from a server but NOT 
    // stream received data (Use ReceiveStreamAggregator for this). Probably should put the power to stream into a seperate class.
    class RemoteRecordingAggregator : RecordingAggregator
    {
        // Client for sending recording data
        private RecordingTCPClient client;
        // For receiving recording data from a server
        Queue<string> recording;
        // Encode and decode JSON strings
        JsonCoder coder;
        // Are we currently playbacking data?
        bool initiatedPlayback = false;
        // Are we connected to a server?
        bool connected = false;

        public RemoteRecordingAggregator(IPoseTrackerShower[] recs, string ip, int port) : base(recs)
        {
            client = new RecordingTCPClient(ip, port);
            client.decodedFrame += onDecodedFrame;
            this.recording = new Queue<string>();
            this.coder = new JsonCoder();
        }

        // If we receive a decoded frame, save it for playback later
        private void onDecodedFrame(object sender, string[] decoded)
        {
            lock (recording)
            {
                for (int i = 0; i < decoded.Length; i++)
                {
                    recording.Enqueue(decoded[i]);
                }
            }
        }

        // Assume the server always has a recording
        public override bool hasRecording()
        {
            return true;
        }

        // Playback the last frame in the queue relative to the provided anchor
        public override bool PlaybackOneFrame(Matrix anchorTRS)
        {
            if (!connected)
            {
                client.connect();
                connected = true;
            }

            // Request the recording from the server using a 'P' packet.
            if (!initiatedPlayback)
            {
                client.send("P", true);
                initiatedPlayback = true;
                // We don't receive anything on the first invokation yet, but return true to indicate stream is still going
                return true;
            }

            // Check if we have a JSON string to playback
            string frameJSON = null;
            lock (recording)
            {
                if (recording.Count != 0) {
                    frameJSON = recording.Dequeue();
                }
            }

            if(frameJSON != null)
            {
                // If we do it, deserialize it into RecordingData[] and display it using the logic of the associated Recorders.
                try
                {
                    DeserializedRecordingArray deserialized = coder.Deserialize<DeserializedRecordingArray>(recording.Dequeue());
                    Label3D[] frame = deserialized.toRecordingDataArray();
                    int[] paramLengths = deserialized.getParamLengths();

                    displayAll(frame, paramLengths, anchorTRS);
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine("Caught an exception when deserializing/displaying: " + e.Message);
                    return true;
                }
            }

            // If the recordings over, reset variables
            if (client.hasReceivedAll() && recording.Count == 0)
            {
                client.reset();
                initiatedPlayback = false;
                return false;
            }

            return true;
        }

        // Record/Stream one frame to the server relative to the provided anchor
        public override void RecordOneFrame(Matrix anchorTRS)
        {
            if (!connected)
            {
                client.connect();
                connected = true;
            }

            // Fetch the current recording data, serialize it, and send it off
            Label3D[] data = getCurrentRecordingData(anchorTRS);
            int[] paramLengths = getCurrentParamLengths();
            
            string serializedRecording = coder.Serialize(DeserializedRecordingArray.fromRecordingDataArray(data, paramLengths));
            client.send(serializedRecording);
        }

        // Reset & disconnect the client upon finishing playback
        public override void finishPlayback()
        {
            client.reset();
            connected = false;
            initiatedPlayback = false;
        }

        // Reset & disconnect the client upon finishing playback
        public override void finishRecording()
        {
            client.reset();
            connected = false;
        }


    }
}
