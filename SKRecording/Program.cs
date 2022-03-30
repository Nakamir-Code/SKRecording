using StereoKit;
using System;
using System.Net;

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
            HandRecorder leftRecorder = new HandRecorder(Handed.Left);
            HeadRecorder headRecorder = new HeadRecorder();

            string IP = "10.0.0.6";
            int port = 12345;
            string hostName = Dns.GetHostName();
            string myIP = Utils.GetLocalIPAddress();


            Recorder[] recorders = new Recorder[] { rightRecorder, leftRecorder, headRecorder };
            RecordingAggregator aggregator = new RemoteRecordingAggregator(recorders, IP, port);
            RecordingAggregator streamReceiver = new ReceiveStreamAggregator(recorders, myIP, port, 100);

            // rotate
            Pose windowPose = new Pose(0, 0.2f, -0.3f, Quat.LookDir(0, 0, 1));

            bool recording = false;
            bool playing = false;
            bool receivingStream = false;
            bool wasRecording = false;

            // Core application loop
            while (SK.Step(() =>
            {
                UI.WindowBegin("Window", ref windowPose, new Vec2(20, 0) * U.cm);
                if (!playing && !receivingStream)
                {
                    UI.Toggle(recording ? "Stop Recording" : "Record", ref recording);
                }
                if (aggregator.hasRecording() && !recording && !receivingStream)
                {
                    UI.Toggle(playing ? "Stop Playing" : "Play", ref playing);
                }
                if (!playing && !receivingStream)
                {
                    UI.Toggle(playing ? "Stop Streaming" : "Stream", ref recording);
                }
                if (!recording && !playing)
                {
                    UI.Toggle(receivingStream ? "Stop Receiving" : "Start Receiving", ref receivingStream);
                }
                UI.WindowEnd();

                if (recording)
                {
                    aggregator.RecordOneFrame();
                }
                else if (playing)
                {
                    playing = aggregator.PlaybackOneFrame();
                    if (!playing)
                    {
                        aggregator.finishPlayback();
                    }
                }
                else if (receivingStream)
                {
                    receivingStream = streamReceiver.PlaybackOneFrame();
                    if (!receivingStream)
                    {
                        streamReceiver.finishPlayback();
                    }
                }

                if(wasRecording && !recording)
                {
                    aggregator.finishRecording();
                }

                wasRecording = recording;

            })) ;
            SK.Shutdown();
        }

    }
}
