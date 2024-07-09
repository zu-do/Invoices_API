using InvoicesAPI.Data;
using InvoicesAPI.Services.Interfaces;

namespace InvoicesAPI.Services
{
    public class InvoiceHtmlService : IInvoiceHtmlService
    {
        #region Generate invoice html
        public string GenerateInvoiceHtml(CreateInvoiceRequestDto createInvoiceRequestDto, decimal amountWithTaxes, out string invoiceNumber)
        {
            invoiceNumber = GenerateInvoiceNumber();

            string htmlContent = $@"
            <!DOCTYPE html>
            <html>
            <head>
                <style>
                    body {{ font-family: Arial, sans-serif; margin: 20px; }}
                    .header {{ text-align: center; margin-bottom: 20px; }}
                    .header h1 {{ margin: 0; }}
                    .details {{ width: 100%; margin-bottom: 20px; }}
                    .details th, .details td {{ border: 1px solid #ddd; padding: 8px; width: 50%;}}
                    .details th {{ background-color: #f2f2f2; text-align: left; }}
                </style>
            </head>
            <body>
                <div class='header'>
                    <h1>Invoice</h1>
                    <p>Invoice Number: {invoiceNumber}</p>
                    <p>Date: {DateTime.Now.ToShortDateString()}</p>
                </div>
                <table class='details'>
                    <tr>
                        <th>From</th>
                        <th>To</th>
                    </tr>
                    <tr>
                        <td>
                            <strong>{createInvoiceRequestDto.VendorName}</strong><br/>
                        </td>
                        <td>
                            <strong>{createInvoiceRequestDto.CustomersName}</strong><br/>
                        </td>
                    </tr>
                </table>
                <table class='details'>
                    <tr>
                        <th>VAT</th>
                        <th>Total</th>
                    </tr>
                    <tr>
                        <td>
                            <strong>{amountWithTaxes - createInvoiceRequestDto.NetAmount} EUR</strong><br/>
                        </td>
                        <td>
                            <strong>{amountWithTaxes} EUR</strong><br/>
                        </td>
                    </tr>
                </table>
            </body>
            </html>";

            return htmlContent;
        }
        #endregion

        private string GenerateInvoiceNumber()
        {
            Random random = new();
            int randomNumber = random.Next(1000, 10000);
            return $"INV-{randomNumber}";
        }
    }
}
