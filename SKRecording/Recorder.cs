using StereoKit;
namespace SKRecording
{
    interface Recorder
    {
        Pose[] getCurrentFrame();
        void displayFrame(Pose[] poses);
        int getPoseCount();
    }
}
