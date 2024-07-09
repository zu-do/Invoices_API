namespace InvoicesAPI.Services.Interfaces
{
    public interface IPdfService
    {
        byte[] GenerateInvoicePdf(string htmlContent);
    }
}