using StereoKit;

namespace SKRecording
{
    // Interface for interacting with 3D models
    interface ModelWrapper
    {
        // Implements the logic to display this instance's model based on the provided RecordingData
        void show(Label3D[] data);
    }
}
