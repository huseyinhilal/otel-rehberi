using Microsoft.EntityFrameworkCore;
using ReportService.Data;
using ReportService.Interfaces;
using ReportService.Services;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddDbContext<ReportDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
    new MySqlServerVersion(new Version(8, 0, 27))));


// Add RabbitMQ services
builder.Services.AddSingleton<IRabbitMQProducerService, RabbitMQProducerService>();
builder.Services.AddHostedService<RabbitMQConsumerService>();

builder.Services.AddHttpClient<HotelServiceClient>();

builder.Services.AddScoped<IReportServiceT, ReportServiceT>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Exception Middleware
app.UseMiddleware<ExceptionMiddleware>();

app.Run();
