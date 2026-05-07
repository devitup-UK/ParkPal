using ParkPal.PushEngine.Models;
using ParkPal.PushEngine.Workers;

var builder = Host.CreateApplicationBuilder(args);

// Register the background worker
builder.Services.AddHostedService<AlertEvaluationWorker>();
builder.Services.AddHostedService<LiveActivityWorker>();

builder.Services.Configure<ApplePushSettings>(builder.Configuration.GetSection("ApplePush"));

var host = builder.Build();
host.Run();