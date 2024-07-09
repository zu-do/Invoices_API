using InvoicesAPI.Services.Interfaces;
using InvoicesAPI.Services;
using Moq;
using InvoicesAPI.Data;

namespace InvoicesAPI.Tests
{
    public class InvoicesServiceTests
    {
        private readonly Mock<ICountriesService> _countriesServiceMock;
        private readonly Mock<IVATService> _VATServiceMock;
        private readonly InvoicesService _invoicesService;

        public InvoicesServiceTests()
        {
            _countriesServiceMock = new Mock<ICountriesService>();
            _VATServiceMock = new Mock<IVATService>();
            _invoicesService = new InvoicesService(_countriesServiceMock.Object, _VATServiceMock.Object);
        }

        [Fact]
        public async Task GetInvoiceAmount_VendorOutsideEU_ThrowsException()
        {
            // Arrange
            var invoiceDetails = new CreateInvoiceRequestDto
            {
                VendorsCountryCode = "US",
                IsVendorVATRegistered = true,
                NetAmount = 100,
                CustomersCountryCode = "FR"
            };

            _countriesServiceMock.Setup(service => service.IsCountryInEUAsync(invoiceDetails.VendorsCountryCode))
                .ReturnsAsync(false);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _invoicesService.GetInvoiceAmount(invoiceDetails));
        }


        [Fact]
        public async Task GetInvoiceAmount_VendorNotVATRegistered_ReturnsAmountWithoutVAT()
        {
            // Arrange
            var invoiceDetails = new CreateInvoiceRequestDto
            {
                VendorsCountryCode = "lt",
                IsVendorVATRegistered = false,
                NetAmount = 100,
                CustomersCountryCode = "FR"
            };

            _countriesServiceMock.Setup(service => service.IsCountryInEUAsync(invoiceDetails.VendorsCountryCode))
                .ReturnsAsync(true);

            //Act
            var amountWithTaxes = await _invoicesService.GetInvoiceAmount(invoiceDetails);

            //Assert
            Assert.Equal(invoiceDetails.NetAmount, amountWithTaxes);
        }

        [Fact]
        public async Task GetInvoiceAmount_CustomerOutsideEU_ReturnsAmountWithoutVAT()
        {
            // Arrange
            var invoiceDetails = new CreateInvoiceRequestDto
            {
                VendorsCountryCode = "lt",
                IsVendorVATRegistered = true,
                NetAmount = 100,
                CustomersCountryCode = "us"
            };

            _countriesServiceMock.Setup(service => service.IsCountryInEUAsync(invoiceDetails.VendorsCountryCode))
                .ReturnsAsync(true);
            _countriesServiceMock.Setup(service => service.IsCountryInEUAsync(invoiceDetails.CustomersCountryCode))
                .ReturnsAsync(false);

            //Act
            var amountWithTaxes = await _invoicesService.GetInvoiceAmount(invoiceDetails);

            //Assert
            Assert.Equal(invoiceDetails.NetAmount, amountWithTaxes);
        }

        [Fact]
        public async Task GetInvoiceAmount_JuridicalCustomerIntraEU_ReturnsAmountWithoutVAT()
        {
            // Arrange
            var invoiceDetails = new CreateInvoiceRequestDto
            {
                VendorsCountryCode = "lt",
                IsVendorVATRegistered = true,
                NetAmount = 100,
                CustomersCountryCode = "lv",
                IsCustomerJuridical = true
            };

            _countriesServiceMock.Setup(service => service.IsCountryInEUAsync(invoiceDetails.VendorsCountryCode))
                .ReturnsAsync(true);
            _countriesServiceMock.Setup(service => service.IsCountryInEUAsync(invoiceDetails.CustomersCountryCode))
                .ReturnsAsync(true);

            //Act
            var amountWithTaxes = await _invoicesService.GetInvoiceAmount(invoiceDetails);

            //Assert
            Assert.Equal(invoiceDetails.NetAmount, amountWithTaxes);
        }

        [Fact]
        public async Task GetInvoiceAmount_IndividualCustomerDiffEUCountry_ReturnsAmountWithCustomerCountriesVAT()
        {
            // Arrange
            var invoiceDetails = new CreateInvoiceRequestDto
            {
                VendorsCountryCode = "lt",
                IsVendorVATRegistered = true,
                NetAmount = 100,
                CustomersCountryCode = "fr",
                IsCustomerJuridical = false
            };

            var vendorsCountryVATRate = 15;
            var customersCountryVATRate = 20;

            _countriesServiceMock.Setup(service => service.IsCountryInEUAsync(invoiceDetails.VendorsCountryCode))
                .ReturnsAsync(true);
            _countriesServiceMock.Setup(service => service.IsCountryInEUAsync(invoiceDetails.CustomersCountryCode))
                .ReturnsAsync(true);

            _VATServiceMock.Setup(service => service.GetVATRate(invoiceDetails.VendorsCountryCode))
                .ReturnsAsync(15);
            _VATServiceMock.Setup(service => service.GetVATRate(invoiceDetails.CustomersCountryCode))
            .ReturnsAsync(20);

            var expectedResult = (invoiceDetails.NetAmount * customersCountryVATRate / 100) + invoiceDetails.NetAmount;

            //Act
            var amountWithTaxes = await _invoicesService.GetInvoiceAmount(invoiceDetails);

            //Assert
            Assert.Equal(expectedResult, amountWithTaxes);
        }

        [Fact]
        public async Task GetInvoiceAmount_IndividualCustomerSameEUCountry_ReturnsAmountWithVAT()
        {
            // Arrange
            var invoiceDetails = new CreateInvoiceRequestDto
            {
                VendorsCountryCode = "lt",
                IsVendorVATRegistered = true,
                NetAmount = 100,
                CustomersCountryCode = "lt",
                IsCustomerJuridical = false
            };

            var VATRate = 15;

            _countriesServiceMock.Setup(service => service.IsCountryInEUAsync(invoiceDetails.VendorsCountryCode))
                .ReturnsAsync(true);
            _countriesServiceMock.Setup(service => service.IsCountryInEUAsync(invoiceDetails.CustomersCountryCode))
                .ReturnsAsync(true);

            _VATServiceMock.Setup(service => service.GetVATRate(invoiceDetails.VendorsCountryCode))
                .ReturnsAsync(VATRate);
            _VATServiceMock.Setup(service => service.GetVATRate(invoiceDetails.CustomersCountryCode))
                .ReturnsAsync(VATRate);

            var expectedResult = (invoiceDetails.NetAmount * VATRate / 100) + invoiceDetails.NetAmount;

            //Act
            var amountWithTaxes = await _invoicesService.GetInvoiceAmount(invoiceDetails);

            //Assert
            Assert.Equal(expectedResult, amountWithTaxes);
        }

        [Fact]
        public async Task GetInvoiceAmount_JuridicalCustomerSameEUCountry_ReturnsAmountWithVAT()
        {
            // Arrange
            var invoiceDetails = new CreateInvoiceRequestDto
            {
                VendorsCountryCode = "lt",
                IsVendorVATRegistered = true,
                NetAmount = 100,
                CustomersCountryCode = "lt",
                IsCustomerJuridical = true
            };

            var VATRate = 15;

            _countriesServiceMock.Setup(service => service.IsCountryInEUAsync(invoiceDetails.VendorsCountryCode))
                .ReturnsAsync(true);
            _countriesServiceMock.Setup(service => service.IsCountryInEUAsync(invoiceDetails.CustomersCountryCode))
                .ReturnsAsync(true);

            _VATServiceMock.Setup(service => service.GetVATRate(invoiceDetails.VendorsCountryCode))
                .ReturnsAsync(VATRate);
            _VATServiceMock.Setup(service => service.GetVATRate(invoiceDetails.CustomersCountryCode))
                .ReturnsAsync(VATRate);

            var expectedResult = (invoiceDetails.NetAmount * VATRate / 100) + invoiceDetails.NetAmount;

            //Act
            var amountWithTaxes = await _invoicesService.GetInvoiceAmount(invoiceDetails);

            //Assert
            Assert.Equal(expectedResult, amountWithTaxes);
        }
    }
}
