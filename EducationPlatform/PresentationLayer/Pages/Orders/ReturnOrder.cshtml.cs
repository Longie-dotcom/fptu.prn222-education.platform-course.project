using BusinessLayer.Interface;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PresentationLayer.ViewModels;

namespace PresentationLayer.Pages.Orders
{
    public class ReturnOrderModel : PageModel
    {
        #region Attributes
        private readonly IOrderService orderService;
        #endregion

        #region Properties
        public ReturnOrderViewModel VM { get; set; } = new();
        #endregion

        public ReturnOrderModel(IOrderService orderService)
        {
            this.orderService = orderService;
        }

        #region Methods
        public async Task OnGet(
            string code,
            string id,
            bool cancel,
            string status,
            long orderCode)
        {
            VM = new ReturnOrderViewModel
            {
                OrderCode = orderCode,
                Status = status,
                IsSuccess = status == "PAID" && !cancel
            };

            if (VM.IsSuccess)
            {
                await orderService.FinishOrder(orderCode);
                VM.Message = "Payment successful! Your course is now available.";
            }
            else
            {
                VM.Message = "Payment was cancelled or failed. Please try again.";
            }
        }
        #endregion
    }
}