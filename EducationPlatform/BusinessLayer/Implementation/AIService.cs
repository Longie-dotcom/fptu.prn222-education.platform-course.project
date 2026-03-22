using BusinessLayer.Interface;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BusinessLayer.Implementation
{
    public class AIService : IAIService
    {
        #region Attributes
        private readonly HttpClient httpClient;
        #endregion

        #region Properties
        #endregion

        public AIService()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            var aiLocal = configuration["AI:LocalPath"]
                ?? throw new InvalidOperationException("AI:LocalPath missing");

            httpClient = new HttpClient
            {
                BaseAddress = new Uri(aiLocal),
                Timeout = TimeSpan.FromMinutes(2)
            };
        }

        #region Methods
        public Task<string> GenerateQuizAsync(string grade, string subject, string transcript)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
