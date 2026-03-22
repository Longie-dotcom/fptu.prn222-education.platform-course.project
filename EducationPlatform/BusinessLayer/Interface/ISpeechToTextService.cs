namespace BusinessLayer.Interface
{
    public interface ISpeechToTextService
    {
        Task<string> TranscribeVideo(string fullPath);
    }
}
