using BusinessLayer.DTO;
using BusinessLayer.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Presentation.Helper;

namespace PresentationLayer.Pages.Orders
{
    public class ListOrderModel : PageModel
    {
        #region Attributes
        private readonly IOrderService orderService;
        #endregion

        #region Properties
        public IEnumerable<OrderDTO> Orders { get; set; } = new List<OrderDTO>();

        [BindProperty(SupportsGet = true)]
        public string? Status { get; set; }

        [BindProperty(SupportsGet = true)]
        public int Page { get; set; } = 1;
        #endregion

        public ListOrderModel(
                IOrderService orderService)
        {
            this.orderService = orderService;
        }

        #region Methods
        public async Task OnGet()
        {
            try
            {
                (Guid userId, string role) = CheckClaimHelper.CheckClaim(User);

                var query = new QueryOrderDTO
                {
                    OrderStatus = Status,
                    PageIndex = Page,
                    PageSize = 6
                };

                Orders = await orderService.GetOrders(query, userId, role);
            }
            catch (Exception ex)
            {
                TempData["ToastMessage"] = ex.Message;
                TempData["ToastType"] = "danger";
            }
        }
        #endregion
    }
}