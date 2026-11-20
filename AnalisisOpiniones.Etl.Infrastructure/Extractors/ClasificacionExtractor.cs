using AnalisisOpiniones.Etl.Core.Interfaces;
using AnalisisOpiniones.Etl.Core.Staging;
using AnalisisOpiniones.Etl.Infrastructure.Options;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace AnalisisOpiniones.Etl.Infrastructure.Extractors
{
    public class ClasificacionExtractor : SqlExtractorBase<ClasificacionStaging>
    {
        public ClasificacionExtractor(IConfiguration configuration, IOptions<FuentesOptions> options, IStagingWriter staging)
            : base(configuration, options, staging)
        {
        }

        public override string Nombre => "Base de datos (Clasificaciones)";

        public override int Orden => 1;

        protected override string TablaStaging => "stg.Clasificaciones";

        protected override string Consulta => @"
            SELECT IdClasificacion, Nombre
            FROM dbo.Clasificaciones";

        protected override ClasificacionStaging Mapear(SqlDataReader reader, DateTime fechaExtraccion) => new()
        {
            IdClasificacion = reader.GetInt32(0),
            Nombre = Texto(reader, 1),
            FechaExtraccion = fechaExtraccion
        };
    }
}
