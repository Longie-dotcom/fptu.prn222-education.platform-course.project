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
        public async Task<string> GenerateQuizAsync(
      string grade,
      string subject,
      string transcript)
        {
            var cleanGrade = grade.Replace("Grade ", "").Trim();
            var prompt = $$"""
            Bạn là giáo viên {{subject}} lớp {{cleanGrade}} tại Việt Nam.

            Nhiệm vụ:
            Tạo CHÍNH XÁC 10 câu hỏi trắc nghiệm giúp học sinh ôn tập.

            Ngữ cảnh bài học:
            {{transcript}}

            Yêu cầu:
            - Câu hỏi phù hợp trình độ lớp {{cleanGrade}}
            - Bám sát nội dung bài học
            - Ưu tiên kiểm tra các phần học sinh yếu
            - Mỗi câu có 4 đáp án (A, B, C, D)
            - Chỉ có 1 đáp án đúng
            - Có giải thích ngắn gọn

            QUAN TRỌNG:
            - Trả về DUY NHẤT JSON ARRAY
            - Không markdown
            - Không giải thích ngoài JSON

            Format:
            [
              {
                "question": "...",
                "options": ["A", "B", "C", "D"],
                "answer": "...",
                "explanation": "..."
              }
            ]
            """;

            var body = new
            {
                model = "gemma3:4b",
                prompt = prompt,
                stream = false,
                options = new
                {
                    temperature = 0.3
                }
            };

            var json = System.Text.Json.JsonSerializer.Serialize(body);

            var request = new HttpRequestMessage(
                HttpMethod.Post,
                "http://localhost:11434/api/generate");

            request.Content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.SendAsync(request);
            var responseString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception(responseString);

            var parsed = JObject.Parse(responseString);
            var aiText = parsed["response"]?.ToString() ?? "";

            // Clean AI output
            aiText = aiText.Replace("```json", "").Replace("```", "").Trim();

            var start = aiText.IndexOf('[');
            var end = aiText.LastIndexOf(']');

            if (start != -1 && end != -1)
                aiText = aiText.Substring(start, end - start + 1);

            // Validate JSON
            try
            {
                var quizArray = JArray.Parse(aiText);
                return quizArray.ToString(Newtonsoft.Json.Formatting.None);
            }
            catch
            {
                throw new Exception("AI returned invalid JSON format.");
            }
        }
        #endregion
    }

}
