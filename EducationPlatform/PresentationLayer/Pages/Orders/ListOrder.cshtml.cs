using BusinessLayer.DTO;
using BusinessLayer.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Presentation.Helper;
using System.Security.Cryptography;
using System.Text;

namespace PresentationLayer.Pages.Orders
{
    public class ListOrderModel : PageModel
    {
        #region Attributes
        private readonly IOrderService orderService;
        private readonly HttpClient httpClient;
        private readonly IConfiguration config;
        #endregion

        #region Properties
        public IEnumerable<OrderDTO> Orders { get; set; } = new List<OrderDTO>();

        [BindProperty(SupportsGet = true)]
        public string? Status { get; set; }

        [BindProperty(SupportsGet = true)]
        public int Page { get; set; } = 1;
        #endregion

        public ListOrderModel(
                IConfiguration config,
                IOrderService orderService,
                IHttpClientFactory factory)
        {
            this.config = config;
            this.orderService = orderService;
            this.httpClient = factory.CreateClient("PayOSClient");
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

        public async Task<IActionResult> OnPostCreateOrder(Guid courseId)
        {
            var (userId, role) = CheckClaimHelper.CheckClaim(User);

            var order = await orderService.CreateOrder(new CreateOrderDTO()
            {
                CourseID = courseId,
                StudentID = userId,
            });

            var payos = config.GetSection("PayOS");

            long orderCode = order.OrderCode;
            int amount = (int)(order.PlatformAmount + order.TeacherAmount);
            string description = "CourseOrder";

            string signature = GenerateSignature(
                orderCode,
                amount,
                description,
                payos["ReturnUrl"]!,
                payos["CancelUrl"]!,
                payos["ChecksumKey"]!
            );

            long expiredAt = DateTimeOffset.UtcNow
                .AddMinutes(15)
                .ToUnixTimeSeconds();

            var payload = new
            {
                orderCode,
                amount,
                description,
                cancelUrl = payos["CancelUrl"],
                returnUrl = payos["ReturnUrl"],
                expiredAt,
                signature
            };

            var json = JsonConvert.SerializeObject(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Add("x-client-id", payos["ClientId"]);
            httpClient.DefaultRequestHeaders.Add("x-api-key", payos["ApiKey"]);

            var request = new HttpRequestMessage(
                HttpMethod.Post,
                "https://api-merchant.payos.vn/v2/payment-requests");

            request.Headers.Add("x-client-id", payos["ClientId"]);
            request.Headers.Add("x-api-key", payos["ApiKey"]);
            request.Headers.Add("accept", "application/json");

            request.Content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                var errorDetails = await response.Content.ReadAsStringAsync();
                throw new Exception($"PayOS Error: {response.StatusCode} - {errorDetails}");
            }

            var responseJson = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<dynamic>(responseJson);

            string checkoutUrl = result?.data?.checkoutUrl;

            return Redirect(checkoutUrl);
        }

        private static string GenerateSignature(
            long orderCode,
            int amount,
            string description,
            string returnUrl,
            string cancelUrl,
            string checksumKey)
        {
            string raw =
                $"amount={amount}&cancelUrl={cancelUrl}&description={description}&orderCode={orderCode}&returnUrl={returnUrl}";

            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(checksumKey));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(raw));
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }
        #endregion
    }
}