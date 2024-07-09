using InvoicesAPI.Services;
using InvoicesAPI.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();

builder.Services.AddTransient<IInvoicesService, InvoicesService>();
builder.Services.AddTransient<ICountriesService, CountriesService>();
builder.Services.AddTransient<IVATService, VATService>();
builder.Services.AddTransient<IInvoiceHtmlService, InvoiceHtmlService>();
builder.Services.AddTransient<IPdfService, PdfService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();
app.Run();
