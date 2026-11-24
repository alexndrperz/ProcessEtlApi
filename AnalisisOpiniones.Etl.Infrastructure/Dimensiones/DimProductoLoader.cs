using AnalisisOpiniones.Etl.Core.Dimensiones;
using AnalisisOpiniones.Etl.Core.Interfaces;
using AnalisisOpiniones.Etl.Infrastructure.Comun;
using AnalisisOpiniones.Etl.Infrastructure.Options;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace AnalisisOpiniones.Etl.Infrastructure.Dimensiones
{
    public class DimProductoLoader : DimensionLoaderBase<DimProducto>
    {
        public DimProductoLoader(IConfiguration configuration, IOptions<CargaOptions> options, IDimensionWriter writer)
            : base(configuration, options, writer)
        {
        }

        public override string Nombre => "DimProducto";

        protected override string Consulta => @"
            SELECT IdProducto, Nombre, Categoria
            FROM stg.Productos
            WHERE IdProducto > 0";

        protected override string Procedimiento => "dbo.usp_CargarDimProducto";

        protected override string TipoTabla => "dbo.ProductoTipo";

        protected override DimProducto Transformar(SqlDataReader reader) => new(
            reader.GetInt32(0),
            Transformacion.Texto(reader.Texto(1)),
            Transformacion.Texto(reader.Texto(2)));

        protected override int Clave(DimProducto fila) => fila.IdProducto;
    }
}
