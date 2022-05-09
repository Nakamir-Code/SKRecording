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
        private RecordingTCPClient _client;
        // For receiving recording data from a server
        Queue<string> _recording;
        // Encode and decode JSON strings
        JsonCoder _coder;
        // Are we currently playbacking data?
        bool _initiatedPlayback = false;
        // Are we connected to a server?
        bool _connected = false;

        public RemoteRecordingAggregator(IPoseTrackerShower[] recs, string ip, int port) : base(recs)
        {
            _client = new RecordingTCPClient(ip, port);
            _client.decodedFrame += onDecodedFrame;
            this._recording = new Queue<string>();
            this._coder = new JsonCoder();
        }

        // If we receive a decoded frame, save it for playback later
        private void onDecodedFrame(object sender, string[] decoded)
        {
            lock (_recording)
            {
                for (int i = 0; i < decoded.Length; i++)
                {
                    _recording.Enqueue(decoded[i]);
                }
            }
        }

        // Assume the server always has a recording
        public override bool hasRecording() // TODO: if always true, why create a function for this!
        {
            return true;
        }

        // Playback the last frame in the queue relative to the provided anchor
        public override bool PlaybackOneFrame(Matrix anchorTRS)
        {
            if (!_connected)
            {
                _client.connect();
                _connected = true;
            }

            // Request the recording from the server using a 'P' packet.
            if (!_initiatedPlayback)
            {
                _client.send("P", true);
                _initiatedPlayback = true;
                // We don't receive anything on the first invokation yet, but return true to indicate stream is still going
                return true;
            }

            // Check if we have a JSON string to playback
            string frameJSON = null;
            lock (_recording)
            {
                if (_recording.Count != 0) {
                    frameJSON = _recording.Dequeue();
                }
            }

            if(frameJSON != null)
            {
                // If we do it, deserialize it into RecordingData[] and display it using the logic of the associated Recorders.
                try
                {
                    DeserializedRecordingArray deserialized = _coder.Deserialize<DeserializedRecordingArray>(_recording.Dequeue());
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
            if (_client.hasReceivedAll() && _recording.Count == 0)
            {
                _client.reset();
                _initiatedPlayback = false;
                return false;
            }

            return true;
        }

        // Record/Stream one frame to the server relative to the provided anchor
        public override void StreamOneFrame(Matrix anchorTRS)
        {
            if (!_connected)
            {
                _client.connect();
                _connected = true;
            }

            // Fetch the current recording data, serialize it, and send it off
            Label3D[] labels = getCurrentRecordingData(anchorTRS);
            int[] paramLengths = getCurrentParamLengths();
            
            string serializedRecording = _coder.Serialize(DeserializedRecordingArray.fromRecordingDataArray(labels, paramLengths));
            _client.send(serializedRecording);
        }

        // Reset & disconnect the client upon finishing playback
        public override void finishPlayback()
        {
            _client.reset();
            _connected = false;
            _initiatedPlayback = false;
        }

        // Reset & disconnect the client upon finishing playback
        public override void finishRecording()
        {
            _client.reset();
            _connected = false;
        }


    }
}
