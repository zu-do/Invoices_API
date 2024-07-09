using PdfSharpCore.Pdf;
using PdfSharpCore;
using TheArtOfDev.HtmlRenderer.PdfSharp;
using InvoicesAPI.Services.Interfaces;

namespace InvoicesAPI.Services
{
    public class PdfService : IPdfService
    {
        public byte[] GenerateInvoicePdf(string htmlContent)
        {
            var document = new PdfDocument();
            PdfGenerator.AddPdfPages(document, htmlContent, PageSize.A4);

            byte[]? response = null;
            using (MemoryStream ms = new())
            {
                document.Save(ms);
                response = ms.ToArray();
            }

            return response;
        }
    }
}
