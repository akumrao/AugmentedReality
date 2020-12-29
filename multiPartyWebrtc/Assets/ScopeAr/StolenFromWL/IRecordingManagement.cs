namespace Scope.Core
{
    public interface IRecordingManagement
    {
        long RecordingLength { get; }
        void StartRecording(IFileRecordingStreamProvider streamProvider);
        void StopRecording();
    }
}