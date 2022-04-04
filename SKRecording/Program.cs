using StereoKit;
using System;
using System.Collections.Generic;

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

            List<RecordingData> annotations = new List<RecordingData>();
            HandRecorder rightRecorder = new HandRecorder(Handed.Right);
            HandRecorder leftRecorder = new HandRecorder(Handed.Left);
            HeadRecorder headRecorder = new HeadRecorder();
            AnnotationRecorder annotationRecorder = new AnnotationRecorder(annotations);

            string IP = "10.0.0.5";
            int port = 12345;
            string myIP = Utils.GetLocalIPAddress();

            Recorder[] recorders = new Recorder[] { headRecorder, rightRecorder, leftRecorder, annotationRecorder };
            RecordingAggregator aggregator = new RemoteRecordingAggregator(recorders,IP,port);
            RecordingAggregator streamReceiver = new ReceiveStreamAggregator(recorders, myIP, port, 100);

            Pose windowPoseInput = new Pose(0, 0.1f, -0.2f, Quat.LookDir(0, 0, 1));
            string inputState = "Enter annotation text";
            
            // rotate
            Pose windowPose = new Pose(0, 0.2f, -0.3f, Quat.LookDir(0, 0, 1));
            
            bool recording = false;
            bool playing = false;
            bool streaming = false;
            bool wasRecording = false;
            bool receivingStream = false;
            bool addingAnnotation = false;

            // Core application loop
            while (SK.Step(() =>
            {

                if (addingAnnotation)
                {

                    UI.WindowBegin("New annotation", ref windowPoseInput);
                if (UI.Button("Done"))
                {
                    annotations.Add(new RecordingData(windowPoseInput, inputState));
                    addingAnnotation = false;
                    inputState = "Enter annotation text";
                }

                UI.Input("AnnotationText", ref inputState, default, TextContext.Text);
                    

                    UI.WindowEnd();
            }
                else
                    {
                        windowPoseInput = Input.Head;
                    }


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
                if (!addingAnnotation)
                {
                    addingAnnotation = UI.Button("Add Annotation");
                }

                UI.WindowEnd();

                if(!playing && !receivingStream)
                {
                    for (int i = 0; i < annotations.Count; i++)
                    {
                        Utils.showDeletableAnnotation(annotations[i],annotations);
                    }
                }


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
