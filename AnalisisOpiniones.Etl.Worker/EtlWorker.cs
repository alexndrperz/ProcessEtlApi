using System.Diagnostics;
using AnalisisOpiniones.Etl.Core.Dimensiones;
using AnalisisOpiniones.Etl.Core.Interfaces;
using Microsoft.Extensions.Options;

namespace AnalisisOpiniones.Etl.Worker
{
    public class EtlWorker : BackgroundService
    {
        private readonly IEnumerable<IExtractor> _extractores;
        private readonly IEnumerable<IDimensionLoader> _dimensiones;
        private readonly WorkerOptions _options;
        private readonly IHostApplicationLifetime _lifetime;
        private readonly ILogger<EtlWorker> _logger;

        public EtlWorker(IEnumerable<IExtractor> extractores, IEnumerable<IDimensionLoader> dimensiones,
            IOptions<WorkerOptions> options, IHostApplicationLifetime lifetime, ILogger<EtlWorker> logger)
        {
            _extractores = extractores;
            _dimensiones = dimensiones;
            _options = options.Value;
            _lifetime = lifetime;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await EjecutarExtraccionAsync(stoppingToken);
                await EjecutarCargaDimensionesAsync(stoppingToken);

                if (_options.IntervaloMinutos <= 0)
                    break;

                _logger.LogInformation("Próxima ejecución en {Minutos} minutos.", _options.IntervaloMinutos);
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

        private async Task EjecutarCargaDimensionesAsync(CancellationToken cancellationToken)
        {
            var dimensiones = _dimensiones.ToList();
            if (dimensiones.Count == 0)
            {
                _logger.LogWarning("La carga de dimensiones está deshabilitada en la configuración.");
                return;
            }

            _logger.LogInformation("Iniciando carga de {Cantidad} dimensiones: {Dimensiones}",
                dimensiones.Count, string.Join(", ", dimensiones.Select(d => d.Nombre)));

            var total = Stopwatch.StartNew();
            var resultados = await Task.WhenAll(dimensiones.Select(d => CargarDimensionAsync(d, cancellationToken)));
            total.Stop();

            var correctas = resultados.OfType<ResultadoCarga>().ToList();
            _logger.LogInformation("Carga de dimensiones finalizada en {Ms} ms: {Ok} correctas, {Error} con error, {Insertadas} filas insertadas y {Actualizadas} actualizadas.",
                total.ElapsedMilliseconds, correctas.Count, resultados.Length - correctas.Count,
                correctas.Sum(r => r.Insertadas), correctas.Sum(r => r.Actualizadas));
        }

        private async Task<ResultadoCarga?> CargarDimensionAsync(IDimensionLoader dimension, CancellationToken cancellationToken)
        {
            var reloj = Stopwatch.StartNew();
            try
            {
                _logger.LogInformation("{Dimension}: cargando...", dimension.Nombre);
                var resultado = await dimension.CargarAsync(cancellationToken);
                _logger.LogInformation("{Dimension}: {Leidas} leídas, {Insertadas} insertadas, {Actualizadas} actualizadas y {SinCambios} sin cambios en {Ms} ms.",
                    dimension.Nombre, resultado.Leidas, resultado.Insertadas, resultado.Actualizadas, resultado.SinCambios, reloj.ElapsedMilliseconds);
                return resultado;
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                _logger.LogWarning("{Dimension}: carga cancelada.", dimension.Nombre);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Dimension}: error durante la carga después de {Ms} ms.", dimension.Nombre, reloj.ElapsedMilliseconds);
                return null;
            }
        }
    }
}
