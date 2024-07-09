namespace InvoicesAPI.Data
{
    public class CreateInvoiceRequestDto
    {
        public decimal NetAmount { get; set; }  
        public string CustomersName { get; set; }
        public string CustomersCountryCode { get; set; }
        public string VendorName { get; set; }
        public string VendorsCountryCode { get; set; }
        public bool IsVendorVATRegistered { get; set; }
        public bool IsCustomerJuridical { get; set; }
    }
}
