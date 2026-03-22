using BusinessLayer.DTO;
using BusinessLayer.Implementation;
using BusinessLayer.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PresentationLayer.Pages.Statistics
{
    public class StatisticModel : PageModel
    {
        #region Attributes
        private readonly IStatisticService statisticService;
        private readonly IAcademicService academicService;
        #endregion

        #region Properties
        public SummaryStatisticDTO Summary { get; set; } = new();

        public IEnumerable<GradeDTO> Grades { get; set; } = new List<GradeDTO>();
        public IEnumerable<SubjectDTO> Subjects { get; set; } = new List<SubjectDTO>();

        [BindProperty(SupportsGet = true)]
        public QuerySummaryDTO SummaryQuery { get; set; } = new();
        #endregion

        public StatisticModel(
            IStatisticService statisticService,
            IAcademicService academicService)
        {
            this.statisticService = statisticService;
            this.academicService = academicService;
        }

        #region Methods
        public async Task OnGetAsync()
        {
            // 1. Set default dates if not provided
            SummaryQuery.From ??= DateTime.Now.AddMonths(-1);
            SummaryQuery.To ??= DateTime.Now;

            // 2. Fetch Academic Data for the dropdown filters
            Grades = await academicService.GetGrades();
            Subjects = await academicService.GetSubjects();

            // 3. Fetch Summary Data
            Summary = await statisticService.SummaryStatistic(SummaryQuery);
        }

        public async Task<JsonResult> OnGetAnalyticsDataAsync(QueryAnalyticsGrowthDTO query)
        {
            var data = await statisticService.AnalyticsGrowth(query);
            return new JsonResult(data);
        }

        public async Task<JsonResult> OnGetAnalyticsDemandAndSupplyAsync(QueryAnalyticsGrowthDTO query)
        {
            var data = await statisticService.AnalyticsDemandAndSupply(query);
            return new JsonResult(data);
        }

        public async Task<JsonResult> OnGetAnalyticsNormalizedGrowthAsync(QueryAnalyticsGrowthDTO query)
        {
            var data = await statisticService.AnalyticsNormalizedGrowth(query);
            return new JsonResult(data);
        }

        public async Task<JsonResult> OnGetTopPerformanceAsync(QueryTopPerformanceDTO query)
        {
            var data = await statisticService.GetTopPerformance(query);
            return new JsonResult(data);
        }
        #endregion
    }
}
