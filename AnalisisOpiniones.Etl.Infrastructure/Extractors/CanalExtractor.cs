using AnalisisOpiniones.Etl.Core.Interfaces;
using AnalisisOpiniones.Etl.Core.Staging;
using AnalisisOpiniones.Etl.Infrastructure.Options;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace AnalisisOpiniones.Etl.Infrastructure.Extractors
{
    public class CanalExtractor : SqlExtractorBase<CanalStaging>
    {
        public CanalExtractor(IConfiguration configuration, IOptions<FuentesOptions> options, IStagingWriter staging)
            : base(configuration, options, staging)
        {
        }

        public override string Nombre => "Base de datos (Canales)";

        public override int Orden => 1;

        protected override string TablaStaging => "stg.Canales";

        protected override string Consulta => @"
            SELECT IdCanal, Nombre
            FROM dbo.Canales";

        protected override CanalStaging Mapear(SqlDataReader reader, DateTime fechaExtraccion) => new()
        {
            IdCanal = reader.GetInt32(0),
            Nombre = Texto(reader, 1),
            FechaExtraccion = fechaExtraccion
        };
    }
}
