using Domain.OrderManagement.Enum;

namespace BusinessLayer.DTO
{
    public class OrderDTO
    {
        public Guid OrderID { get; set; }
        public long OrderCode { get; set; }
        public decimal PlatformAmount { get; set; }
        public decimal TeacherAmount { get; set; }
        public OrderMethod Method { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? PaidAt { get; set; }
        public Guid StudentID { get; set; }
        public Guid CourseID { get; set; }
    }

    public class QueryOrderDTO
    {
        public string? OrderStatus { get; set; } = string.Empty;
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 1;
    }

    public class CreateOrderDTO
    {
        public Guid StudentID { get; set; }
        public Guid CourseID { get; set; }
        public List<Guid>? CouponIds { get; set; } = new List<Guid>();
    }

    public class CouponDTO
    {
        public Guid CouponID { get; set; }
        public Guid StudentID { get; set; }
        public string Code { get; set; } = string.Empty;
        public decimal DiscountAmount { get; set; }
        public bool IsUsed { get; set; }
        public string Reason { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
