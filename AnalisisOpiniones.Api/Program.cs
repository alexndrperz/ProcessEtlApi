using AnalisisOpiniones.Etl.Core.Interfaces;
using AnalisisOpiniones.Etl.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("AnalisisOpiniones")
    ?? throw new InvalidOperationException("Falta la cadena de conexión 'AnalisisOpiniones'.");

builder.Services.AddComentariosSociales(connectionString);

var app = builder.Build();

app.MapGet("/health", () => Results.Ok("OK"));

app.MapGet("/api/social-comments", async (int? page, int? pageSize, IComentarioSocialRepository repositorio, CancellationToken ct) =>
{
    var pagina = Math.Max(page ?? 1, 1);
    var tamano = Math.Clamp(pageSize ?? 100, 1, 1000);

    return Results.Ok(await repositorio.ObtenerPaginaAsync(pagina, tamano, ct));
});

app.Run();
