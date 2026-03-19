using BusinessLayer.DTO;

namespace BusinessLayer.Interface
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderDTO>> GetOrders(
            QueryOrderDTO dto,
            Guid callerId,
            string callerRole);

        Task<OrderDTO> CreateOrder(
            CreateOrderDTO dto);

        Task<OrderDTO> FinishOrder(
            long orderCode);

        Task<IEnumerable<CouponDTO>> GetCoupons(
            Guid callerId,
            string callerRole);
    }
}
