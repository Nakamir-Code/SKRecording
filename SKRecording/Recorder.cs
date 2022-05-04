namespace SKRecording
{
    // Interface for recording a specific part of a frame
    interface Recorder
    {
        // Returns an array of RecordingData describing the state of the tracked objects of this Recorder instance 
        // "Record"
        RecordingData[] getCurrentFrame();
        // Displays one frame with the provided recording data using the display logic of the this Recorder instance
        // "Playback"
        void displayFrame(RecordingData[] recordingData);
        // Returns how many RecordingData are managed by this Recorder instance
        int getObjectCount();
    }
}
