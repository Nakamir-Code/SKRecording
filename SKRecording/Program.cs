using StereoKit;
using System;
using System.Collections.Generic;

namespace SKRecording
{
    // Main program class
    class Program
    {
        static void Main(string[] args)
        {
            /* 
                START SK Boilerplate 
            */

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

            /* 
                END SK Boilerplate 
            */


            // List to track annotations in
            List<RecordingData> annotations = new List<RecordingData>();
            // Create Recorders for all objects we want to track
            HandRecorder rightRecorder = new HandRecorder(Handed.Right);
            HandRecorder leftRecorder = new HandRecorder(Handed.Left);
            HeadRecorder headRecorder = new HeadRecorder();
            AnnotationRecorder annotationRecorder = new AnnotationRecorder(annotations);

            // Server we connect to 
            string IP = "10.0.0.5";
            int port = 12345;
            // Our IP
            string myIP = Utils.GetLocalIPAddress();

            Recorder[] recorders = new Recorder[] { headRecorder, rightRecorder, leftRecorder, annotationRecorder };
            // Recordingaggregators (Here for streaming & server-side recording)
            RecordingAggregator aggregator = new RemoteRecordingAggregator(recorders,IP,port);
            RecordingAggregator streamReceiver = new ReceiveStreamAggregator(recorders, myIP, port, 100);

            // Information for window to add annotation
            Pose windowPoseInput = new Pose(0, 0.1f, -0.2f, Quat.LookDir(0, 0, 1));
            string inputState = "Enter annotation text";
            
            // Information for main control window
            Pose windowPose = new Pose(0, 0.2f, -0.3f, Quat.LookDir(0, 0, 1));
            
            // Control variables
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
                    // If we are adding an annotation, display the annotation window
                    UI.WindowBegin("New annotation", ref windowPoseInput);
                    if (UI.Button("Done"))
                    {
                        // Add the inputted annotation to the list of annotations
                        annotations.Add(new RecordingData(windowPoseInput, inputState));
                        addingAnnotation = false;
                        inputState = "Enter annotation text";
                    }

                    UI.Input("AnnotationText", ref inputState, default, TextContext.Text);
                        

                    UI.WindowEnd();
                }
                // If we don't have to display the annotation window, just have it invisible in the user's head
                else
                {
                    windowPoseInput = Input.Head;
                }


            // Logic for main control window
            UI.WindowBegin("Window", ref windowPose, new Vec2(20, 0) * U.cm);
                // If we are not doing anything, allow the user to start a recording
                if (!playing && !receivingStream && !streaming)
                {
                    UI.Toggle(recording ? "Stop Recording" : "Record", ref recording);
                }
                // If we are not doing anything and have a recording, allow the user to start playing a recording
                if (aggregator.hasRecording() && !recording && !receivingStream && !streaming)
                {
                    UI.Toggle(playing ? "Stop Playing" : "Play", ref playing);
                }
                // If we are not doing anything, allow the user to stream (seperate if 
                // statement because we want the order of the buttons to be record - play - stream - receive)
                if (!playing && !receivingStream && !recording)
                {
                    UI.Toggle(playing ? "Stop Streaming" : "Stream", ref streaming);
                }
                // If we are not doing anything, allow the user to receive (seperate if 
                // statement because we want the order of the buttons to be record - play - stream - receive)                
                if (!recording && !playing && !streaming)
                {
                    UI.Toggle(receivingStream ? "Stop Receiving" : "Start Receiving", ref receivingStream);
                }
                // If we are not already adding an annotation, allow the user to add an annotation
                if (!addingAnnotation)
                {
                    addingAnnotation = UI.Button("Add Annotation");
                }

                UI.WindowEnd();

                // If we are not currently playbacking/receiving a different stream, display the locally created annotations
                if(!playing && !receivingStream)
                {
                    for (int i = 0; i < annotations.Count; i++)
                    {
                        Utils.showDeletableAnnotation(annotations[i],annotations);
                    }
                }


                // If we are recording or streaming (equivalent in this case), call record on the recordingaggregator
                if (recording || streaming)
                {
                    aggregator.RecordOneFrame(windowPose.ToMatrix());
                }
                // If we are instead playbacking, try playbacking from the aggregator and end the playback if this call fails
                else if (playing)
                {
                    playing = aggregator.PlaybackOneFrame(windowPose.ToMatrix());
                    if (!playing)
                    {
                        aggregator.finishPlayback();
                    }
                }
                // If we are instead receiving a stream, try receving the next frame from the streamreceiver and 
                // end the stream receiving if this call fails
                else if (receivingStream)
                {
                    receivingStream = streamReceiver.PlaybackOneFrame(windowPose.ToMatrix());
                    if (!receivingStream)
                    {
                        streamReceiver.finishPlayback();
                    }
                }

                // If we were recording, but no longer are, call the cleanup method
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
