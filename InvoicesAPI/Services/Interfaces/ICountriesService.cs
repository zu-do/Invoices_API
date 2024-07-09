using InvoicesAPI.Data;

namespace InvoicesAPI.Services.Interfaces
{
    public interface ICountriesService
    {
        Task<bool> IsCountryInEUAsync(string countryCode);
    }
}