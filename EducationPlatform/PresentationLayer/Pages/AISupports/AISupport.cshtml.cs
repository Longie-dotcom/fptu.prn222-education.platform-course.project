using BusinessLayer.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json.Linq;
using Presentation.Helper;
using PresentationLayer.ViewModels;
using System.Text;

namespace PresentationLayer.Pages.AISupports
{
    [Authorize(Roles = "Student")]
    public class AISupportModel : PageModel
    {
        #region Attributes
        private readonly IEnrollmentService enrollmentService;
        private readonly HttpClient httpClient;
        private readonly IConfiguration config;
        #endregion

        #region Properties
        [BindProperty]
        public AISupportViewModel ViewModel { get; set; } = new();
        #endregion

        public AISupportModel(
            IEnrollmentService enrollmentService,
            IConfiguration config,
            IHttpClientFactory factory)
        {
            this.enrollmentService = enrollmentService;
            this.config = config;
            this.httpClient = factory.CreateClient();
        }

        #region Methods

        public async Task OnGet()
        {
            try
            {
                var (userId, role) = CheckClaimHelper.CheckClaim(User);

                var enrollments = await enrollmentService.GetStudentEnrollments(userId);

                ViewModel.Enrollments = enrollments.ToList();
            }
            catch (Exception ex)
            {
                TempData["ToastMessage"] = ex.Message;
                TempData["ToastType"] = "danger";
            }
        }

        public async Task<IActionResult> OnPostGenerateQuiz()
        {
            try
            {
                var (userId, role) = CheckClaimHelper.CheckClaim(User);

                // Load enrollments
                var enrollments = await enrollmentService.GetStudentEnrollments(userId);
                ViewModel.Enrollments = enrollments.ToList();

                if (ViewModel.SelectedEnrollmentId == null)
                {
                    TempData["ToastMessage"] = "Please choose a course.";
                    TempData["ToastType"] = "warning";
                    return Page();
                }

                // Get student weaknesses
                var weaknesses = await enrollmentService
                    .GetEnrollmentWeakness(ViewModel.SelectedEnrollmentId.Value);

                if (!weaknesses.Any())
                {
                    TempData["ToastMessage"] = "No weak topics found.";
                    TempData["ToastType"] = "warning";
                    return Page();
                }

                ViewModel.Weaknesses = weaknesses.ToList();

                var weakTopics = string.Join(", ", weaknesses.Select(x => x.LessonTitle));
                var apiKey = config["AI:GroqKey"];

                var prompt = $"""
You are a Vietnamese teacher.

Create exactly 5 multiple choice questions.

Topics:
{weakTopics}

Return ONLY a valid JSON array.

Each object MUST have EXACTLY these properties:
- question (string)
- options (array of 4 strings)
- answer (string, must match one option exactly)
- explanation (string)

Do NOT rename properties.
Do NOT add extra properties.
Do NOT wrap in markdown.
""";

                // Correct Groq model
                var body = new
                {
                    model = "llama-3.1-8b-instant", // ✅ Valid Groq model
                    messages = new[]
                    {
                new
                {
                    role = "user",
                    content = prompt
                }
            },
                    temperature = 0.3
                };

                var json = System.Text.Json.JsonSerializer.Serialize(body);

                var request = new HttpRequestMessage(
                    HttpMethod.Post,
                    "https://api.groq.com/openai/v1/chat/completions");

                request.Headers.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);
                request.Headers.Accept.Add(
                    new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                request.Content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await httpClient.SendAsync(request);
                var responseString = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    throw new Exception(responseString);

                // Parse the AI response
                var parsed = JObject.Parse(responseString);
                var aiText = parsed["choices"]?[0]?["message"]?["content"]?.ToString() ?? "";

                // Clean up any extra markdown or whitespace
                aiText = aiText.Replace("```json", "").Replace("```", "").Trim();

                // Extract JSON array
                var start = aiText.IndexOf('[');
                var end = aiText.LastIndexOf(']');
                if (start != -1 && end != -1)
                    aiText = aiText.Substring(start, end - start + 1);

                // Parse JSON
                var quizArray = JArray.Parse(aiText);

                ViewModel.GeneratedQuiz = quizArray.ToString(Newtonsoft.Json.Formatting.None);

                return Page();
            }
            catch (Exception ex)
            {
                TempData["ToastMessage"] = ex.Message;
                TempData["ToastType"] = "danger";
                return Page();
            }
        }
        #endregion
    }
}