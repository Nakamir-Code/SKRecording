namespace SKRecording
{
    interface Recorder
    {
        RecordingData[] getCurrentFrame();
        void displayFrame(RecordingData[] recordingData);
        int getObjectCount();
    }
}
