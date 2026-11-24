using System.Data;
using AnalisisOpiniones.Etl.Core.Dimensiones;
using AnalisisOpiniones.Etl.Core.Interfaces;
using AnalisisOpiniones.Etl.Infrastructure.Comun;
using AnalisisOpiniones.Etl.Infrastructure.Options;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace AnalisisOpiniones.Etl.Infrastructure.DataWarehouse
{
    public class SqlDimensionWriter : IDimensionWriter
    {
        private readonly string _connectionString;
        private readonly CargaOptions _options;

        public SqlDimensionWriter(IConfiguration configuration, IOptions<CargaOptions> options)
        {
            _connectionString = configuration.GetConnectionString("DataWarehouse")
                ?? throw new InvalidOperationException("Falta la cadena de conexión 'DataWarehouse'.");
            _options = options.Value;
        }

        public async Task<ResultadoCarga> CargarAsync<T>(string procedimiento, string tipoTabla,
            IReadOnlyCollection<T> filas, CancellationToken cancellationToken)
        {
            var parametro = new SqlParameter("@Filas", SqlDbType.Structured)
            {
                TypeName = tipoTabla,
                Value = TablaDatos.Crear(filas)
            };

            var resultado = await EjecutarProcedimientoAsync(procedimiento, [parametro], cancellationToken);
            return resultado with { Leidas = filas.Count };
        }

        public Task<ResultadoCarga> EjecutarAsync(string procedimiento, IReadOnlyDictionary<string, object> parametros,
            CancellationToken cancellationToken)
        {
            var sqlParametros = parametros.Select(p => new SqlParameter(p.Key, p.Value)).ToArray();
            return EjecutarProcedimientoAsync(procedimiento, sqlParametros, cancellationToken);
        }

        private async Task<ResultadoCarga> EjecutarProcedimientoAsync(string procedimiento, SqlParameter[] parametros,
            CancellationToken cancellationToken)
        {
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);

            await using var command = new SqlCommand(procedimiento, connection)
            {
                CommandType = CommandType.StoredProcedure,
                CommandTimeout = _options.TimeoutSegundos
            };
            command.Parameters.AddRange(parametros);

            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            if (!await reader.ReadAsync(cancellationToken))
                return new ResultadoCarga(0, 0, 0);

            return new ResultadoCarga(0, reader.GetInt32(0), reader.GetInt32(1));
        }
    }
}
