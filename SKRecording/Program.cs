using StereoKit;
using System;
using Windows.Perception.Spatial;

namespace SKRecording
{
    class Program
    {
        static void Main(string[] args)
        {
            // Initialize StereoKit
            SKSettings settings = new SKSettings
            {
                appName = "SKRecording",
                assetsFolder = "Assets",
            };
            if (!SK.Initialize(settings))
                Environment.Exit(1);

            Matrix floorTransform = Matrix.TS(0, -1.5f, 0, new Vec3(30, 0.1f, 30));
            Material floorMaterial = new Material(Shader.FromFile("floor.hlsl"));
            floorMaterial.Transparency = Transparency.Blend;

            HandRecorder rightRecorder = new HandRecorder(Handed.Right);
            //HandRecorder leftRecorder = new HandRecorder(Handed.Left);
            HeadRecorder headRecorder = new HeadRecorder();

            string IP = "10.0.0.5";
            int port = 12345;
            string myIP = Utils.GetLocalIPAddress();


            Recorder[] recorders = new Recorder[] { headRecorder, rightRecorder/*, leftRecorder*/ };
            RecordingAggregator aggregator = new DynamicRecorderAggregator(recorders);
            RecordingAggregator streamReceiver = new ReceiveStreamAggregator(recorders, myIP, port, 100);

            Pose windowPoseInput = new Pose(0, 0.1f, -0.2f, Quat.Identity);
            string inputState = "Enter label text";
            
            // rotate
            Pose windowPose = new Pose(0, 0.2f, -0.3f, Quat.LookDir(0, 0, 1));
            
            bool recording = false;
            bool playing = false;
            bool streaming = false;
            bool receivingStream = false;
            bool wasRecording = false;

            // Core application loop
            while (SK.Step(() =>
            {
                UI.WindowBegin("Window Input", ref windowPoseInput);

                // Add a small label in front of it on the same line
                UI.Label("Text:");
                UI.SameLine();
                if (UI.Input("Text", ref inputState))
                    Log.Info($"Input text just changed");

                UI.WindowEnd();


                UI.WindowBegin("Window", ref windowPose, new Vec2(20, 0) * U.cm);
                if (!playing && !receivingStream && !streaming)
                {
                    UI.Toggle(recording ? "Stop Recording" : "Record", ref recording);
                }
                if (aggregator.hasRecording() && !recording && !receivingStream && !streaming)
                {
                    UI.Toggle(playing ? "Stop Playing" : "Play", ref playing);
                }
                if (!playing && !receivingStream && !recording)
                {
                    UI.Toggle(playing ? "Stop Streaming" : "Stream", ref streaming);
                }
                if (!recording && !playing && !streaming)
                {
                    UI.Toggle(receivingStream ? "Stop Receiving" : "Start Receiving", ref receivingStream);
                }
                UI.WindowEnd();

                if (recording || streaming)
                {
                    aggregator.RecordOneFrame(windowPose.ToMatrix());
                }
                else if (playing)
                {
                    playing = aggregator.PlaybackOneFrame(windowPose.ToMatrix());
                    if (!playing)
                    {
                        aggregator.finishPlayback();
                    }
                }
                else if (receivingStream)
                {
                    receivingStream = streamReceiver.PlaybackOneFrame(windowPose.ToMatrix());
                    if (!receivingStream)
                    {
                        streamReceiver.finishPlayback();
                    }
                }

                if(wasRecording && !recording && !streaming)
                {
                    aggregator.finishRecording();
                }

                wasRecording = recording || streaming;

            })) ;
            SK.Shutdown();
        }

    }
}
