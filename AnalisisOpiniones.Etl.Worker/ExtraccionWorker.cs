using System.Diagnostics;
using AnalisisOpiniones.Etl.Core.Interfaces;
using Microsoft.Extensions.Options;

namespace AnalisisOpiniones.Etl.Worker
{
    public class ExtraccionWorker : BackgroundService
    {
        private readonly IEnumerable<IExtractor> _extractores;
        private readonly WorkerOptions _options;
        private readonly IHostApplicationLifetime _lifetime;
        private readonly ILogger<ExtraccionWorker> _logger;

        public ExtraccionWorker(IEnumerable<IExtractor> extractores, IOptions<WorkerOptions> options,
            IHostApplicationLifetime lifetime, ILogger<ExtraccionWorker> logger)
        {
            _extractores = extractores;
            _options = options.Value;
            _lifetime = lifetime;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await EjecutarExtraccionAsync(stoppingToken);

                if (_options.IntervaloMinutos <= 0)
                    break;

                _logger.LogInformation("Próxima extracción en {Minutos} minutos.", _options.IntervaloMinutos);
                await Task.Delay(TimeSpan.FromMinutes(_options.IntervaloMinutos), stoppingToken);
            }

            _lifetime.StopApplication();
        }

        private async Task EjecutarExtraccionAsync(CancellationToken cancellationToken)
        {
            var extractores = _extractores.ToList();
            if (extractores.Count == 0)
            {
                _logger.LogWarning("No hay fuentes habilitadas en la configuración.");
                return;
            }

            _logger.LogInformation("Iniciando extracción de {Cantidad} fuentes: {Fuentes}",
                extractores.Count, string.Join(", ", extractores.Select(e => e.Nombre)));

            var total = Stopwatch.StartNew();

            var resultados = new List<(bool Ok, int Filas)>();
            foreach (var etapa in extractores.GroupBy(e => e.Orden).OrderBy(g => g.Key))
            {
                _logger.LogInformation("Etapa {Orden}: {Fuentes}", etapa.Key, string.Join(", ", etapa.Select(e => e.Nombre)));
                resultados.AddRange(await Task.WhenAll(etapa.Select(e => ExtraerFuenteAsync(e, cancellationToken))));
            }

            total.Stop();
            _logger.LogInformation("Extracción finalizada en {Ms} ms: {Ok} fuentes correctas, {Error} con error, {Filas} filas en total.",
                total.ElapsedMilliseconds, resultados.Count(r => r.Ok), resultados.Count(r => !r.Ok), resultados.Sum(r => r.Filas));
        }

        private async Task<(bool Ok, int Filas)> ExtraerFuenteAsync(IExtractor extractor, CancellationToken cancellationToken)
        {
            var reloj = Stopwatch.StartNew();
            try
            {
                _logger.LogInformation("{Fuente}: extrayendo...", extractor.Nombre);
                var filas = await extractor.ExtraerAsync(cancellationToken);
                _logger.LogInformation("{Fuente}: {Filas} filas extraídas en {Ms} ms.", extractor.Nombre, filas, reloj.ElapsedMilliseconds);
                return (true, filas);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                _logger.LogWarning("{Fuente}: extracción cancelada.", extractor.Nombre);
                return (false, 0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Fuente}: error durante la extracción después de {Ms} ms.", extractor.Nombre, reloj.ElapsedMilliseconds);
                return (false, 0);
            }
        }
    }
}
