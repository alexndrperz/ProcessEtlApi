using AnalisisOpiniones.Etl.Core.Interfaces;
using AnalisisOpiniones.Etl.Core.Staging;
using AnalisisOpiniones.Etl.Infrastructure.Options;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace AnalisisOpiniones.Etl.Infrastructure.Extractors
{
    public class ProductoExtractor : SqlExtractorBase<ProductoStaging>
    {
        public ProductoExtractor(IConfiguration configuration, IOptions<FuentesOptions> options, IStagingWriter staging)
            : base(configuration, options, staging)
        {
        }

        public override string Nombre => "Base de datos (Productos)";

        public override int Orden => 1;

        protected override string TablaStaging => "stg.Productos";

        protected override string Consulta => @"
            SELECT p.IdProducto, p.Nombre, c.Nombre AS Categoria
            FROM dbo.Products p
            LEFT JOIN dbo.Categorias c ON c.IdCategoria = p.IdCategoria";

        protected override ProductoStaging Mapear(SqlDataReader reader, DateTime fechaExtraccion) => new()
        {
            IdProducto = reader.GetInt32(0),
            Nombre = Texto(reader, 1),
            Categoria = Texto(reader, 2),
            FechaExtraccion = fechaExtraccion
        };
    }
}
