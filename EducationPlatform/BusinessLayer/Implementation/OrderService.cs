using AutoMapper;
using BusinessLayer.BusinessException;
using BusinessLayer.DTO;
using BusinessLayer.Interface;
using DataAccessLayer.Interface;
using Domain.EnrollmentManagement.Aggregate;
using Domain.IdentityManagement.ValueObject;
using Domain.OrderManagement.Aggregate;
using Domain.OrderManagement.ValueObject;

namespace BusinessLayer.Implementation
{
    public class OrderService : IOrderService
    {
        #region Attributes
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private const decimal PLATFORM_COMMISSION_RATE = 0.1m; // 10% platform fee
        #endregion

        #region Properties
        #endregion

        public OrderService(
            IMapper mapper,
            IUnitOfWork unitOfWork)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
        }

        #region Methods
        public async Task<IEnumerable<OrderDTO>> GetOrders(
            QueryOrderDTO dto,
            Guid callerId,
            string callerRole)
        {
            // Role parsing
            if (!Enum.TryParse<Role>(callerRole, true, out var role))
                throw new AuthenticateException("Invalid role");

            // Teacher scoping
            Guid? teacherId = role == Role.Teacher ? callerId : null;

            // Query
            var list = await unitOfWork
                .GetRepository<IOrderRepository>()
                .GetOrders(
                    dto.OrderStatus,
                    dto.PageIndex,
                    dto.PageSize,
                    teacherId);

            if (list == null || !list.Any())
                throw new NotFound("Order list is not found or empty");

            return mapper.Map<IEnumerable<OrderDTO>>(list);
        }

        public async Task<OrderDTO> CreateOrder(CreateOrderDTO dto)
        {
            // Validate course existence
            var course = await unitOfWork
                .GetRepository<ICourseRepository>()
                .GetByIdAsync(dto.CourseID);

            if (course == null)
                throw new NotFound(
                    $"Course with ID: {dto.CourseID} not found.");

            // Validate student existence
            var student = await unitOfWork
                .GetRepository<IUserRepository>()
                .GetByIdAsync(dto.StudentID);

            if (student == null)
                throw new NotFound(
                    $"Student with ID: {dto.StudentID} not found.");

            // Create commission from course price
            var commission = Commission.Create(
                PLATFORM_COMMISSION_RATE,
                course.Price.Amount);

            // Apply domain - create Order
            var order = new Order(
                Guid.NewGuid(),
                commission,
                dto.StudentID,
                dto.CourseID);

            // Apply persistence
            await unitOfWork.BeginTransactionAsync();
            unitOfWork
                .GetRepository<IOrderRepository>()
                .Add(order);
            await unitOfWork.CommitAsync();

            // Return OrderDTO
            return mapper.Map<OrderDTO>(order);
        }

        public async Task<OrderDTO> FinishOrder(long orderCode)
        {
            // Validate order existence
            var order = await unitOfWork
                .GetRepository<IOrderRepository>()
                .GetOrderByOrderCode(orderCode);

            if (order == null)
                throw new NotFound(
                    $"Order with code: {orderCode} not found.");

            // Validate course existence
            var course = await unitOfWork
                .GetRepository<ICourseRepository>()
                .GetCourseDetailByID(order.CourseID);

            if (course == null)
                throw new NotFound(
                    $"Course with ID: {order.CourseID} not found");

            // Apply domain: start student enrollment and update order status
            order.FinishOrder();

            var enrollment = new Enrollment(
                Guid.NewGuid(),
                order.StudentID,
                order.CourseID);

            // Apply persistence
            await unitOfWork.BeginTransactionAsync();
            unitOfWork
                .GetRepository<IOrderRepository>()
                .Update(order.OrderID, order);
            unitOfWork
                .GetRepository<IEnrollmentRepository>()
                .Add(enrollment);
            await unitOfWork.CommitAsync();

            // Return OrderDTO
            return mapper.Map<OrderDTO>(order);
        }
        #endregion
    }
}
