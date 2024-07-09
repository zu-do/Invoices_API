using InvoicesAPI.Data;
using Newtonsoft.Json.Linq;
using InvoicesAPI.Services.Interfaces;
using InvoicesAPI.Constants;

namespace InvoicesAPI.Services
{
    public class CountriesService : ICountriesService
    {
        private readonly HttpClient _httpClient;

        private static readonly HashSet<string> EUMemberCountries =
        [
            "AT", "BE", "BG", "HR", "CY", "CZ", "DK", "EE", "FI", "FR", "DE", "GR",
            "HU", "IE", "IT", "LV", "LT", "LU", "MT", "NL", "PL", "PT", "RO", "SK",
            "SI", "ES", "SE"
        ];

        public CountriesService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        private async Task<Dictionary<string, CountryDto>> GetCountriesAsync()
        {
            var response = await _httpClient.GetAsync(ExternalURLConstants.COUNTRIES_URL);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to fetch countries data.");
            }

            var content = await response.Content.ReadAsStringAsync();
            var jsonObject = JObject.Parse(content);

            var countriesData = jsonObject?["data"]?
                .ToObject<Dictionary<string, CountryDto>>();

            return countriesData;
        }

        public async Task<bool> IsCountryInEUAsync(string countryCode)
        {
            var countries = await GetCountriesAsync();

            if (countries.TryGetValue(countryCode.ToUpper(), out var country))
            {
                if (country.Region == "Europe")
                {
                    bool isInEU = EUMemberCountries.Contains(countryCode.ToUpper());
                    return isInEU;
                }
            }
            else
            {
                throw new Exception("The country with the provided country code does not exist.");
            }

            return false;
        }
    }
}
