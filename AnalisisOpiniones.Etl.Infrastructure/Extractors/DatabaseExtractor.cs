using AnalisisOpiniones.Etl.Core.Interfaces;
using AnalisisOpiniones.Etl.Core.Staging;
using AnalisisOpiniones.Etl.Infrastructure.Options;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace AnalisisOpiniones.Etl.Infrastructure.Extractors
{
    public class DatabaseExtractor : SqlExtractorBase<ResenaWebStaging>
    {
        public DatabaseExtractor(IConfiguration configuration, IOptions<FuentesOptions> options, IStagingWriter staging)
            : base(configuration, options, staging)
        {
        }

        public override string Nombre => "Base de datos (Reseñas web)";

        protected override string TablaStaging => "stg.ResenasWeb";

        protected override string Consulta => @"
            SELECT IdReview, IdCliente, IdProducto, Fecha,
                   CAST(Comentario AS NVARCHAR(MAX)) AS Comentario, Rating, IdFuente
            FROM dbo.WebReview";

        protected override ResenaWebStaging Mapear(SqlDataReader reader, DateTime fechaExtraccion) => new()
        {
            IdReview = reader.GetInt32(0),
            IdCliente = reader.GetInt32(1),
            IdProducto = reader.GetInt32(2),
            Fecha = Fecha(reader, 3),
            Comentario = Texto(reader, 4),
            Rating = reader.IsDBNull(5) ? null : reader.GetDecimal(5),
            IdFuente = reader.IsDBNull(6) ? null : reader.GetInt32(6),
            FechaExtraccion = fechaExtraccion
        };
    }
}
