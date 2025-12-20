using Microsoft.EntityFrameworkCore;
using PaymentService.Data;
using PaymentService.Services;

var builder = WebApplication.CreateBuilder(args);


var connectionString =  builder.Configuration.GetConnectionString("DefaultConnection");



    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(connectionString));

builder.Services.AddHttpClient<ILicenseServiceClient, LicenseServiceClient>();

builder.Services.AddScoped<IPaymentService, PaymentService.Services.PaymentService>();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.Run();