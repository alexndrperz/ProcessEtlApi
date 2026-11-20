using AnalisisOpiniones.Etl.Core.Interfaces;
using AnalisisOpiniones.Etl.Core.Staging;
using AnalisisOpiniones.Etl.Infrastructure.Options;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace AnalisisOpiniones.Etl.Infrastructure.Extractors
{
    public class ClienteExtractor : SqlExtractorBase<ClienteStaging>
    {
        public ClienteExtractor(IConfiguration configuration, IOptions<FuentesOptions> options, IStagingWriter staging)
            : base(configuration, options, staging)
        {
        }

        public override string Nombre => "Base de datos (Clientes)";

        public override int Orden => 1;

        protected override string TablaStaging => "stg.Clientes";

        protected override string Consulta => @"
            SELECT IdCliente, Nombre, Email
            FROM dbo.Clients";

        protected override ClienteStaging Mapear(SqlDataReader reader, DateTime fechaExtraccion) => new()
        {
            IdCliente = reader.GetInt32(0),
            Nombre = Texto(reader, 1),
            Email = Texto(reader, 2),
            FechaExtraccion = fechaExtraccion
        };
    }
}
