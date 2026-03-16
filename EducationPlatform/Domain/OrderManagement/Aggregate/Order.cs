using Domain.DomainExceptions;
using Domain.OrderManagement.Enum;
using Domain.OrderManagement.ValueObject;

namespace Domain.OrderManagement.Aggregate
{
    public class Order
    {
        #region Attributes
        #endregion

        #region Properties
        public Guid OrderID { get; private set; }
        public long OrderCode { get; private set; }
        public decimal PlatformAmount { get; private set; }
        public decimal TeacherAmount { get; private set; }
        public OrderMethod Method { get; private set; }
        public OrderStatus Status { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? PaidAt { get; private set; }

        public Guid StudentID { get; private set; }
        public Guid CourseID { get; private set; }
        #endregion

        protected Order() { }

        public Order(
            Guid orderId,
            Commission commission,
            Guid studentId,
            Guid courseId)
        {
            if (orderId == Guid.Empty)
                throw new DomainException(
                    "Order ID cannot be empty");

            if (studentId == Guid.Empty)
                throw new DomainException(
                    "Student ID cannot be empty");

            if (courseId == Guid.Empty)
                throw new DomainException(
                    "Course ID cannot be empty");

            OrderID = orderId;
            OrderCode = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            PlatformAmount = commission.PlatformAmount;
            TeacherAmount = commission.TeacherAmount;
            Method = OrderMethod.PayOS;
            Status = OrderStatus.Pending;
            CreatedAt = DateTime.UtcNow;
            StudentID = studentId;
            CourseID = courseId;
        }

        #region Methods
        public void FinishOrder()
        {
            Status = OrderStatus.Paid;
            PaidAt = DateTime.UtcNow;
        }
        #endregion
    }
}
