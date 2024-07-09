using InvoicesAPI.Data;

namespace InvoicesAPI.Services.Interfaces
{
    public interface IInvoiceHtmlService
    {
        string GenerateInvoiceHtml(CreateInvoiceRequestDto createInvoiceRequestDto, decimal amountWithTaxes, out string invoiceNumber);
    }
}