using AnalisisOpiniones.Etl.Infrastructure;
using AnalisisOpiniones.Etl.Worker;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.Configure<WorkerOptions>(builder.Configuration.GetSection(WorkerOptions.Seccion));
builder.Services.AddExtraccion(builder.Configuration);
builder.Services.AddHostedService<ExtraccionWorker>();

var host = builder.Build();
host.Run();
