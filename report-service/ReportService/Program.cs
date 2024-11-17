using Microsoft.EntityFrameworkCore;
using ReportService.Data;
using ReportService.Interfaces;
using ReportService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add DbContext
builder.Services.AddDbContext<ReportDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
    new MySqlServerVersion(new Version(8, 0, 27))));

// Add RabbitMQ services
builder.Services.AddSingleton<RabbitMQProducerService>(); // RabbitMQProducerService kayıt edildi
builder.Services.AddHostedService<RabbitMQConsumerService>(); // Consumer servisi ekliyoruz

// Add HttpClient
builder.Services.AddHttpClient<HotelServiceClient>();

// Add custom services
builder.Services.AddScoped<IReportServiceT, ReportServiceT>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

// Add custom middleware
app.UseMiddleware<ExceptionMiddleware>();

// Map controllers
app.MapControllers();

app.Run();
