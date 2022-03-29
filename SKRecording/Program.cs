using StereoKit;
using System;

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

            string IP = "10.0.0.5";
            int port = 12345;

            Recorder[] recorders = new Recorder[] { rightRecorder, leftRecorder, headRecorder };
            RecordingAggregator aggregator = new RemoteRecordingAggregator(recorders, IP, port);

            // rotate
            Pose windowPose = new Pose(0, 0.2f, -0.3f, Quat.LookDir(0, 0, 1));

            bool recording = false;
            bool playing = false;
            bool wasPlaying = false;
            System.Diagnostics.Debug.WriteLine("GO!");


            // Core application loop
            while (SK.Step(() =>
            {
                UI.WindowBegin("Window", ref windowPose, new Vec2(20, 0) * U.cm);
                if (!playing)
                {
                    UI.Toggle(recording ? "Stop Recording" : "Record", ref recording);
                }
                if (aggregator.hasRecording() && !recording)
                {
                    UI.Toggle(playing ? "Stop Playing" : "Play", ref playing);
                }
                UI.WindowEnd();

                if (recording)
                {
                    aggregator.RecordOneFrame();
                }
                else if (playing)
                {
                    playing = aggregator.PlaybackOneFrame();
                }

                if(wasPlaying && !playing)
                {
                    aggregator.finishPlayback();
                }

                wasPlaying = playing;

            })) ;
            SK.Shutdown();
        }

    }
}
