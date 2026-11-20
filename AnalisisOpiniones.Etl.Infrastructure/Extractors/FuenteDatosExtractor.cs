using AnalisisOpiniones.Etl.Core.Interfaces;
using AnalisisOpiniones.Etl.Core.Staging;
using AnalisisOpiniones.Etl.Infrastructure.Options;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace AnalisisOpiniones.Etl.Infrastructure.Extractors
{
    public class FuenteDatosExtractor : SqlExtractorBase<FuenteDatosStaging>
    {
        public FuenteDatosExtractor(IConfiguration configuration, IOptions<FuentesOptions> options, IStagingWriter staging)
            : base(configuration, options, staging)
        {
        }

        public override string Nombre => "Base de datos (Fuentes de datos)";

        public override int Orden => 1;

        protected override string TablaStaging => "stg.FuentesDatos";

        protected override string Consulta => @"
            SELECT f.IdFuente, t.Nombre AS TipoFuente, f.FechaCarga
            FROM dbo.FuenteDatos f
            LEFT JOIN dbo.TiposFuente t ON t.IdTipoFuente = f.IdTipoFuente";

        protected override FuenteDatosStaging Mapear(SqlDataReader reader, DateTime fechaExtraccion) => new()
        {
            IdFuente = reader.GetInt32(0),
            TipoFuente = Texto(reader, 1),
            FechaCarga = Fecha(reader, 2),
            FechaExtraccion = fechaExtraccion
        };
    }
}
