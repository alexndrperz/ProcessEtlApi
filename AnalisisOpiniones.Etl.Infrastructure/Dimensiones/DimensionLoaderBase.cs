using AnalisisOpiniones.Etl.Core.Dimensiones;
using AnalisisOpiniones.Etl.Core.Interfaces;
using AnalisisOpiniones.Etl.Infrastructure.Options;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace AnalisisOpiniones.Etl.Infrastructure.Dimensiones
{
    public abstract class DimensionLoaderBase<T> : IDimensionLoader
    {
        private readonly string _connectionString;
        private readonly CargaOptions _options;
        private readonly IDimensionWriter _writer;

        protected DimensionLoaderBase(IConfiguration configuration, IOptions<CargaOptions> options, IDimensionWriter writer)
        {
            _connectionString = configuration.GetConnectionString("Staging")
                ?? throw new InvalidOperationException("Falta la cadena de conexión 'Staging'.");
            _options = options.Value;
            _writer = writer;
        }

        public abstract string Nombre { get; }

        protected abstract string Consulta { get; }

        protected abstract string Procedimiento { get; }

        protected abstract string TipoTabla { get; }

        protected abstract T Transformar(SqlDataReader reader);

        protected abstract int Clave(T fila);

        public async Task<ResultadoCarga> CargarAsync(CancellationToken cancellationToken)
        {
            var filas = await LeerStagingAsync(cancellationToken);
            var unicas = filas.DistinctBy(Clave).ToList();

            var resultado = await _writer.CargarAsync(Procedimiento, TipoTabla, unicas, cancellationToken);
            return resultado with { Leidas = filas.Count };
        }

        private async Task<List<T>> LeerStagingAsync(CancellationToken cancellationToken)
        {
            var filas = new List<T>();

            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);

            await using var command = new SqlCommand(Consulta, connection) { CommandTimeout = _options.TimeoutSegundos };
            await using var reader = await command.ExecuteReaderAsync(cancellationToken);

            while (await reader.ReadAsync(cancellationToken))
                filas.Add(Transformar(reader));

            return filas;
        }
    }
}
