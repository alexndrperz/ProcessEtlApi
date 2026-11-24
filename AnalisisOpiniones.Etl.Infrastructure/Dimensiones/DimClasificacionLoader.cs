using AnalisisOpiniones.Etl.Core.Dimensiones;
using AnalisisOpiniones.Etl.Core.Interfaces;
using AnalisisOpiniones.Etl.Infrastructure.Comun;
using AnalisisOpiniones.Etl.Infrastructure.Options;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace AnalisisOpiniones.Etl.Infrastructure.Dimensiones
{
    public class DimClasificacionLoader : DimensionLoaderBase<DimClasificacion>
    {
        public DimClasificacionLoader(IConfiguration configuration, IOptions<CargaOptions> options, IDimensionWriter writer)
            : base(configuration, options, writer)
        {
        }

        public override string Nombre => "DimClasificacion";

        protected override string Consulta => @"
            SELECT IdClasificacion, Nombre
            FROM stg.Clasificaciones
            WHERE IdClasificacion > 0";

        protected override string Procedimiento => "dbo.usp_CargarDimClasificacion";

        protected override string TipoTabla => "dbo.ClasificacionTipo";

        protected override DimClasificacion Transformar(SqlDataReader reader) => new(
            reader.GetInt32(0),
            Transformacion.Texto(reader.Texto(1)));

        protected override int Clave(DimClasificacion fila) => fila.IdClasificacion;
    }
}
