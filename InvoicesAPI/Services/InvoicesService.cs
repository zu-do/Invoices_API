using InvoicesAPI.Data;
using InvoicesAPI.Services.Interfaces;

namespace InvoicesAPI.Services
{
    public class InvoicesService : IInvoicesService
    {
        private readonly ICountriesService _countriesService;
        private readonly IVATService _VATService;

        public InvoicesService(ICountriesService countriesService, IVATService VATService)
        {
            _countriesService = countriesService;
            _VATService = VATService;
        }

        public async Task<decimal?> GetInvoiceAmount(CreateInvoiceRequestDto invoiceDetails)
        {
            await ValidateVendorCountry(invoiceDetails.VendorsCountryCode);

            var isCustomerInEu = await _countriesService.IsCountryInEUAsync(invoiceDetails.CustomersCountryCode);

            if (!invoiceDetails.IsVendorVATRegistered ||
                !isCustomerInEu ||
                IsIntraEuB2BService(invoiceDetails, isCustomerInEu))
            {
                return invoiceDetails.NetAmount;
            }

            return await CalculateTotalAmountApplicableVAT(invoiceDetails);
        }

        #region Calculate total amount with VAT
        private async Task<decimal?> CalculateTotalAmountApplicableVAT(CreateInvoiceRequestDto invoiceDetails)
        {
            if (ShouldApplyCustomersCountryVAT(invoiceDetails, true))
            {
                return await GetAmountWithVAT(invoiceDetails.NetAmount, invoiceDetails.CustomersCountryCode);
            }

            if (!IsCustomerInDifferentCountry(invoiceDetails))
            {
                return await GetAmountWithVAT(invoiceDetails.NetAmount, invoiceDetails.VendorsCountryCode);
            }

            return invoiceDetails.NetAmount;
        }

        private async Task<decimal?> GetAmountWithVAT(decimal netAmount, string countryCode)
        {
            var rate = await _VATService.GetVATRate(countryCode);
            return (netAmount * rate / 100) + netAmount;
        }
        #endregion

        #region VAT rules
        private async Task ValidateVendorCountry(string vendorCountryCode)
        {
            if (!await _countriesService.IsCountryInEUAsync(vendorCountryCode))
            {
                throw new Exception("The provided country code for the vendor is not correct; the vendor has to be in an EU country.");
            }
        }

        private bool ShouldApplyCustomersCountryVAT(CreateInvoiceRequestDto invoiceDetails, bool isCustomerInEu)
        {
            bool isCustomerIndividual = !invoiceDetails.IsCustomerJuridical;

            return isCustomerInEu && isCustomerIndividual && IsCustomerInDifferentCountry(invoiceDetails);
        }

        private bool IsIntraEuB2BService(CreateInvoiceRequestDto invoiceDetails, bool isCustomerInEu)
        {
            return isCustomerInEu && invoiceDetails.IsCustomerJuridical && IsCustomerInDifferentCountry(invoiceDetails);
        }

        private bool IsCustomerInDifferentCountry(CreateInvoiceRequestDto invoiceDetails)
        {
            return invoiceDetails.CustomersCountryCode != invoiceDetails.VendorsCountryCode;
        }
        #endregion
    }
}
