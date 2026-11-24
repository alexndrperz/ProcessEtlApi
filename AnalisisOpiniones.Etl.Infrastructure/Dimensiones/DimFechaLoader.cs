using AnalisisOpiniones.Etl.Core.Dimensiones;
using AnalisisOpiniones.Etl.Core.Interfaces;
using AnalisisOpiniones.Etl.Infrastructure.Comun;
using AnalisisOpiniones.Etl.Infrastructure.Options;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace AnalisisOpiniones.Etl.Infrastructure.Dimensiones
{
    public class DimFechaLoader : IDimensionLoader
    {
        private const string Procedimiento = "dbo.usp_CargarDimFecha";

        private const string ConsultaRango = @"
            SELECT MIN(Fecha), MAX(Fecha)
            FROM (
                SELECT TRY_CONVERT(DATE, Fecha) AS Fecha FROM stg.Encuestas
                UNION ALL SELECT Fecha FROM stg.ResenasWeb
                UNION ALL SELECT Fecha FROM stg.ComentariosSociales
                UNION ALL SELECT FechaCarga FROM stg.FuentesDatos
            ) fechas";

        private readonly string _connectionString;
        private readonly CargaOptions _options;
        private readonly IDimensionWriter _writer;

        public DimFechaLoader(IConfiguration configuration, IOptions<CargaOptions> options, IDimensionWriter writer)
        {
            _connectionString = configuration.GetConnectionString("Staging")
                ?? throw new InvalidOperationException("Falta la cadena de conexión 'Staging'.");
            _options = options.Value;
            _writer = writer;
        }

        public string Nombre => "DimFecha";

        public async Task<ResultadoCarga> CargarAsync(CancellationToken cancellationToken)
        {
            var (minima, maxima) = await ObtenerRangoAsync(cancellationToken);
            if (minima is null || maxima is null)
                return new ResultadoCarga(0, 0, 0);

            var inicio = new DateTime(minima.Value.Year, 1, 1);
            var fin = new DateTime(maxima.Value.Year, 12, 31);

            var parametros = new Dictionary<string, object>
            {
                ["@FechaInicio"] = inicio,
                ["@FechaFin"] = fin
            };

            var resultado = await _writer.EjecutarAsync(Procedimiento, parametros, cancellationToken);
            return resultado with { Leidas = (fin - inicio).Days + 1 };
        }

        private async Task<(DateTime? Minima, DateTime? Maxima)> ObtenerRangoAsync(CancellationToken cancellationToken)
        {
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);

            await using var command = new SqlCommand(ConsultaRango, connection) { CommandTimeout = _options.TimeoutSegundos };
            await using var reader = await command.ExecuteReaderAsync(cancellationToken);

            if (!await reader.ReadAsync(cancellationToken))
                return (null, null);

            return (reader.Fecha(0), reader.Fecha(1));
        }
    }
}
