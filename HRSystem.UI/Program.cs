using HRSystem.Core;
using HRSystem.Infrastructure;
using HRSystem.Services;
using POSSystem.Infrastructure.Logging;
using Serilog;
using Serilog.Events;
using static Org.BouncyCastle.Math.EC.ECCurve;

var builder = WebApplication.CreateBuilder(args);

//Log.Logger = new LoggerConfiguration()
//    .ReadFrom.Configuration(builder.Configuration)
//    .CreateLogger();

//try
//{
//    Log.Logger = new LoggerConfiguration()
//        .ReadFrom.Configuration(builder.Configuration)
//        .CreateLogger();
//}
//catch (Exception ex)
//{
//    Console.WriteLine($"Logger configuration failed: {ex}");
//    throw;
//}
//var logDirectory = Path.Combine(Directory.GetCurrentDirectory(), "logs");
//Directory.CreateDirectory(logDirectory); // No-op if already exists

//Log.Logger = new LoggerConfiguration()
//    //.WriteTo.Console()
//    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 7, shared: true)
//    //.WriteTo.MySQL(connectionString: builder.Configuration.GetConnectionString("DefaultConnection"), tableName: "Logs", storeTimestampInUtc: true)
//    .CreateLogger();


SerilogConfig.ConfigureLogging(builder.Configuration);

builder.Host.UseSerilog();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.InjectCore()
    .InjectInfrastructure(builder.Configuration)
    .InjectServices();


var app = builder.Build();

Log.Logger.Error("Hello, world!");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
