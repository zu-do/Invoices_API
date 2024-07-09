using InvoicesAPI.Constants;
using InvoicesAPI.Data;
using InvoicesAPI.Services.Interfaces;
using Newtonsoft.Json.Linq;

namespace InvoicesAPI.Services
{
    public class VATService : IVATService
    {
        private readonly HttpClient _httpClient;

        public VATService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<decimal?> GetVATRate(string countryCode)
        {
            _httpClient.BaseAddress = new Uri(ExternalURLConstants.VAT_RATES_URL);

            var response = await _httpClient.GetAsync($"/rates/{countryCode}");

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to fetch VAT rates.");
            }

            decimal? standartRate = 0.00m;

            if(response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var jsonObject = JObject.Parse(content);

                standartRate = jsonObject["rates"]?
                    .FirstOrDefault(rate => rate["name"]?
                    .ToString() == "Standard")?["rates"]?
                    .FirstOrDefault()?.ToObject<decimal>();
            }

            return standartRate;
        }
    }
}
