using BusinessLayer.DTO;

namespace BusinessLayer.Interface
{
    public interface IStatisticService
    {
        public Task<SummaryStatisticDTO> SummaryStatistic(
            QuerySummaryDTO dto);

        public Task<AnalyticsGrowthDTO> AnalyticsGrowth(
            QueryAnalyticsGrowthDTO dto);

        Task<AnalyticsGrowthDTO> AnalyticsDemandAndSupply(
            QueryAnalyticsGrowthDTO dto);

        Task<AnalyticsGrowthDTO> AnalyticsNormalizedGrowth(
            QueryAnalyticsGrowthDTO dto);

        Task<TopPerformanceDTO> GetTopPerformance(
            QueryTopPerformanceDTO query);
    }
}
