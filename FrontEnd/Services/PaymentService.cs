using System.Text;
using System.Text.Json;

namespace FrontEnd.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly HttpClient _httpClient;
        private readonly string _paymentServiceUrl;

        public PaymentService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient();
            _paymentServiceUrl = configuration.GetValue<string>("ServiceUrls:PaymentService");
        }

        public async Task<bool> MakePaymentAsync(int licenseId, int tenantId, decimal amount)
        {
            try
            {
                var request = new
                {
                    licenseId = licenseId,
                    tenantId = tenantId,
                    amount = amount
                };

                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_paymentServiceUrl}/api/payment/make-payment", content);
                
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}