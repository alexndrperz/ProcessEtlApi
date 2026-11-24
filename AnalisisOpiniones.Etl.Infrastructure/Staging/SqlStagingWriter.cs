using System.Data;
using AnalisisOpiniones.Etl.Core.Interfaces;
using AnalisisOpiniones.Etl.Infrastructure.Comun;
using AnalisisOpiniones.Etl.Infrastructure.Options;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AnalisisOpiniones.Etl.Infrastructure.Staging
{
    public class SqlStagingWriter : IStagingWriter
    {
        private readonly string _connectionString;
        private readonly StagingOptions _options;
        private readonly ILogger<SqlStagingWriter> _logger;

        public SqlStagingWriter(IConfiguration configuration, IOptions<StagingOptions> options, ILogger<SqlStagingWriter> logger)
        {
            _connectionString = configuration.GetConnectionString("Staging")
                ?? throw new InvalidOperationException("Falta la cadena de conexión 'Staging'.");
            _options = options.Value;
            _logger = logger;
        }

        public async Task GuardarAsync<T>(string tabla, IReadOnlyCollection<T> filas, CancellationToken cancellationToken)
        {
            var datos = TablaDatos.Crear(filas);

            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);
            await using var transaction = connection.BeginTransaction();

            await using (var truncate = new SqlCommand($"TRUNCATE TABLE {tabla}", connection, transaction))
                await truncate.ExecuteNonQueryAsync(cancellationToken);

            using (var bulk = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, transaction))
            {
                bulk.DestinationTableName = tabla;
                bulk.BatchSize = _options.TamanoLote;
                foreach (DataColumn columna in datos.Columns)
                    bulk.ColumnMappings.Add(columna.ColumnName, columna.ColumnName);

                await bulk.WriteToServerAsync(datos, cancellationToken);
            }

            await transaction.CommitAsync(cancellationToken);
            _logger.LogInformation("{Tabla}: {Filas} filas guardadas en staging.", tabla, filas.Count);
        }
    }
}
