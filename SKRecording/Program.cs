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

            // rotate
            Pose windowPose = new Pose(0, 0.2f, -0.3f, Quat.LookDir(0, 0, 1));

            bool recording = false;
            bool playing = false;

            // Core application loop
            while (SK.Step(() =>
            {
                UI.WindowBegin("Window", ref windowPose, new Vec2(20, 0) * U.cm);
                if (!playing)
                {
                    UI.Toggle(recording ? "Stop Recording" : "Record", ref recording);
                }
                if (rightRecorder.hasRecording() && !recording)
                {
                    UI.Toggle(playing ? "Stop Playing" : "Play", ref playing);
                }
                UI.WindowEnd();

                if (recording)
                {
                    rightRecorder.recordHandFrame(Input.Hand(Handed.Right));
                    leftRecorder.recordHandFrame(Input.Hand(Handed.Left));
                    headRecorder.recordHeadFrame(Input.Head);
                }
                else if (playing)
                {
                    bool rightStillPlaying = rightRecorder.playbackHandFrame();
                    bool lefttStillPlaying = leftRecorder.playbackHandFrame();
                    bool headStillPlaying = headRecorder.playbackHeadFrame();
                    playing = rightStillPlaying && lefttStillPlaying;

                }

            })) ;
            SK.Shutdown();
        }

    }
}
