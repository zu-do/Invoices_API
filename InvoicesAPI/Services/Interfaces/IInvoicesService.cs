using InvoicesAPI.Data;

namespace InvoicesAPI.Services.Interfaces
{
    public interface IInvoicesService
    {
        Task<decimal?> GetInvoiceAmount(CreateInvoiceRequestDto invoiceDetails);
    }
}