using Domain.OrderManagement.Aggregate;

namespace DataAccessLayer.Interface
{
    public interface IOrderRepository :
        IGenericRepository<Order>,
        IRepositoryBase
    {
        Task<IEnumerable<Order>> GetOrders(
            string? status,
            int pageIndex,
            int pageSize,
            Guid? teacherId);

        Task<Order?> GetOrderByOrderCode(
            long orderCode);

        Task<IEnumerable<Penalty>> GetPenalties(
            Guid? teacherId);

        void CreatePenalty(
            Penalty penalty);

        Task<IEnumerable<Coupon>> GetCoupons(
            Guid? studentId);

        Task<Coupon?> GetCouponDetailById(
            Guid couponId);

        void CreateCoupons(IEnumerable<Coupon> coupons);
    }
}
