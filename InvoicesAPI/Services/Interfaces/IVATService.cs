namespace InvoicesAPI.Services.Interfaces
{
    public interface IVATService
    {
        Task<decimal?> GetVATRate(string countryCode);
    }
}