using BusinessLayer.Interface;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;

namespace BusinessLayer.Implementation
{
    public class SpeechToTextService : ISpeechToTextService
    {
        #region Attributes
        #endregion

        #region Properties
        #endregion

        public SpeechToTextService()
        {

        }

        #region Methods
        public Task<string> TranscribeVideo(string fullPath)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
