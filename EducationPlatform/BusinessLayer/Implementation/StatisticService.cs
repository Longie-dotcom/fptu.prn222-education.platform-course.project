using AutoMapper;
using BusinessLayer.DTO;
using BusinessLayer.Enums;
using BusinessLayer.Interface;
using DataAccessLayer.Interface;

namespace BusinessLayer.Implementation
{
    public class StatisticService : IStatisticService
    {
        #region Attributes
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        #endregion

        #region Properties
        #endregion

        public StatisticService(
            IMapper mapper,
            IUnitOfWork unitOfWork)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
        }

        #region Methods
        public Task<AnalyticsGrowthDTO> AnalyticsDemandAndSupply(
            QueryAnalyticsGrowthDTO dto)
        {
            throw new NotImplementedException();
        }

        public Task<AnalyticsGrowthDTO> AnalyticsGrowth(
            QueryAnalyticsGrowthDTO dto)
        {
            throw new NotImplementedException();
        }

        public Task<AnalyticsGrowthDTO> AnalyticsNormalizedGrowth(
            QueryAnalyticsGrowthDTO dto)
        {
            throw new NotImplementedException();
        }

        public Task<TopPerformanceDTO> GetTopPerformance(
            QueryTopPerformanceDTO query)
        {
            throw new NotImplementedException();
        }

        public Task<SummaryStatisticDTO> SummaryStatistic(
            QuerySummaryDTO dto)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
