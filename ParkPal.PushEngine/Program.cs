using ParkPal.PushEngine.Workers;

var builder = Host.CreateApplicationBuilder(args);

// Register the background worker
builder.Services.AddHostedService<AlertEvaluationWorker>();
builder.Services.AddHostedService<LiveActivityWorker>();

var host = builder.Build();
host.Run();