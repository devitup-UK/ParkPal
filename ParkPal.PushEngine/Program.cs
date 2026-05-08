using ParkPal.PushEngine.Models;
using ParkPal.PushEngine.Workers;
using Serilog;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSerilog((services, loggerConfiguration) => loggerConfiguration
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "ParkPal.PushEngine")
    .WriteTo.Console()
    .WriteTo.Seq(builder.Configuration.GetConnectionString("LoggingConnection") ?? "http://localhost:5341")
);

// Register the background worker
builder.Services.AddHostedService<AlertEvaluationWorker>();
builder.Services.AddHostedService<LiveActivityWorker>();

builder.Services.Configure<ApplePushSettings>(builder.Configuration.GetSection("ApplePush"));

var host = builder.Build();
host.Run();