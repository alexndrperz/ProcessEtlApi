using AnalisisOpiniones.Etl.Core.Interfaces;
using AnalisisOpiniones.Etl.Infrastructure.Options;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace AnalisisOpiniones.Etl.Infrastructure.Extractors
{
    public abstract class SqlExtractorBase<T> : IExtractor
    {
        private readonly string _connectionString;
        private readonly BaseDatosOptions _options;
        private readonly IStagingWriter _staging;

        protected SqlExtractorBase(IConfiguration configuration, IOptions<FuentesOptions> options, IStagingWriter staging)
        {
            _connectionString = configuration.GetConnectionString("Origen")
                ?? throw new InvalidOperationException("Falta la cadena de conexión 'Origen'.");
            _options = options.Value.BaseDatos;
            _staging = staging;
        }

        public abstract string Nombre { get; }

        public virtual int Orden => 2;

        protected abstract string TablaStaging { get; }

        protected abstract string Consulta { get; }

        protected abstract T Mapear(SqlDataReader reader, DateTime fechaExtraccion);

        public async Task<int> ExtraerAsync(CancellationToken cancellationToken)
        {
            var fechaExtraccion = DateTime.Now;
            var filas = new List<T>();

            await using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken);

                await using var command = new SqlCommand(Consulta, connection) { CommandTimeout = _options.TimeoutSegundos };
                await using var reader = await command.ExecuteReaderAsync(cancellationToken);

                while (await reader.ReadAsync(cancellationToken))
                    filas.Add(Mapear(reader, fechaExtraccion));
            }

            await _staging.GuardarAsync(TablaStaging, filas, cancellationToken);
            return filas.Count;
        }

        protected static string? Texto(SqlDataReader reader, int i) => reader.IsDBNull(i) ? null : reader.GetString(i);

        protected static DateTime? Fecha(SqlDataReader reader, int i) => reader.IsDBNull(i) ? null : reader.GetDateTime(i);
    }
}
