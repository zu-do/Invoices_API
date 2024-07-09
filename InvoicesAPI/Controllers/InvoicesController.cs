using InvoicesAPI.Data;
using InvoicesAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace InvoicesAPI.Controllers
{
    [ApiController]
    [Route("api/v1/invoices")]
    public class InvoicesController : ControllerBase
    {
        private readonly IInvoicesService _invoicesService;
        private readonly IInvoiceHtmlService _invoiceHtmlService;
        private readonly IPdfService _pdfService;

        public InvoicesController(IInvoicesService invoicesService, IInvoiceHtmlService htmlService, IPdfService pdfService)
        {
            _invoicesService = invoicesService;
            _invoiceHtmlService = htmlService;
            _pdfService = pdfService;
        }

        [HttpPost("generateInvoice")]
        public async Task<IActionResult> Get(CreateInvoiceRequestDto createInvoiceRequestDto)
        {
            try
            {
                var amountWithTaxes = await _invoicesService.GetInvoiceAmount(createInvoiceRequestDto);

                var htmlContent = _invoiceHtmlService.GenerateInvoiceHtml(createInvoiceRequestDto, (decimal)amountWithTaxes, out string invoiceNumber);

                var generatedPdf = _pdfService.GenerateInvoicePdf(htmlContent);

                string fileName = $"Invoice_{invoiceNumber}.pdf";

                return File(generatedPdf, "application/pdf", fileName);
            }

            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
