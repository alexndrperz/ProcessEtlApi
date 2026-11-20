using System.Globalization;
using AnalisisOpiniones.Etl.Core.Interfaces;
using AnalisisOpiniones.Etl.Core.Staging;
using AnalisisOpiniones.Etl.Infrastructure.Options;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AnalisisOpiniones.Etl.Infrastructure.Extractors
{
    public class CsvExtractor : IExtractor
    {
        private const string TablaStaging = "stg.Encuestas";

        private readonly CsvOptions _options;
        private readonly IStagingWriter _staging;
        private readonly ILogger<CsvExtractor> _logger;

        public CsvExtractor(IOptions<FuentesOptions> options, IStagingWriter staging, ILogger<CsvExtractor> logger)
        {
            _options = options.Value.Csv;
            _staging = staging;
            _logger = logger;
        }

        public string Nombre => "CSV (Encuestas)";

        public int Orden => 2;

        public async Task<int> ExtraerAsync(CancellationToken cancellationToken)
        {
            var ruta = Path.IsPathRooted(_options.RutaArchivo)
                ? _options.RutaArchivo
                : Path.Combine(AppContext.BaseDirectory, _options.RutaArchivo);

            if (!File.Exists(ruta))
                throw new FileNotFoundException($"No se encontró el archivo CSV: {ruta}");

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                TrimOptions = TrimOptions.Trim,
                MissingFieldFound = null,
                BadDataFound = null
            };

            using var reader = new StreamReader(ruta);
            using var csv = new CsvReader(reader, config);

            var fechaExtraccion = DateTime.Now;
            var filas = new List<EncuestaStaging>();
            var rechazadas = 0;

            await foreach (var fila in csv.GetRecordsAsync<EncuestaCsv>(cancellationToken))
            {
                if (string.IsNullOrWhiteSpace(fila.IdOpinion) || string.IsNullOrWhiteSpace(fila.Comentario))
                {
                    rechazadas++;
                    continue;
                }

                filas.Add(new EncuestaStaging
                {
                    IdOpinion = fila.IdOpinion,
                    IdCliente = fila.IdCliente,
                    IdProducto = fila.IdProducto,
                    Fecha = fila.Fecha,
                    Comentario = fila.Comentario,
                    Clasificacion = fila.Clasificacion,
                    PuntajeSatisfaccion = fila.PuntajeSatisfaccion,
                    Fuente = fila.Fuente,
                    FechaExtraccion = fechaExtraccion
                });
            }

            if (rechazadas > 0)
                _logger.LogWarning("{Fuente}: {Rechazadas} filas rechazadas por datos incompletos.", Nombre, rechazadas);

            await _staging.GuardarAsync(TablaStaging, filas, cancellationToken);
            return filas.Count;
        }

        private class EncuestaCsv
        {
            public string IdOpinion { get; set; } = string.Empty;
            public string IdCliente { get; set; } = string.Empty;
            public string IdProducto { get; set; } = string.Empty;
            public string Fecha { get; set; } = string.Empty;
            public string Comentario { get; set; } = string.Empty;

            [Name("Clasificación")]
            public string Clasificacion { get; set; } = string.Empty;

            [Name("PuntajeSatisfacción")]
            public string PuntajeSatisfaccion { get; set; } = string.Empty;

            public string Fuente { get; set; } = string.Empty;
        }
    }
}
