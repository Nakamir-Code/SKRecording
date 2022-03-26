namespace SKRecording
{
    interface RecordingAggregator
    {
        void RecordOneFrame();
        bool PlaybackOneFrame();
        bool hasRecording();
    }
}
