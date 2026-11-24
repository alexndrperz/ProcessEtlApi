using AnalisisOpiniones.Etl.Core.Dimensiones;
using AnalisisOpiniones.Etl.Core.Interfaces;
using AnalisisOpiniones.Etl.Infrastructure.Comun;
using AnalisisOpiniones.Etl.Infrastructure.Options;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace AnalisisOpiniones.Etl.Infrastructure.Dimensiones
{
    public class DimCanalLoader : DimensionLoaderBase<DimCanal>
    {
        public DimCanalLoader(IConfiguration configuration, IOptions<CargaOptions> options, IDimensionWriter writer)
            : base(configuration, options, writer)
        {
        }

        public override string Nombre => "DimCanal";

        protected override string Consulta => @"
            SELECT IdCanal, Nombre
            FROM stg.Canales
            WHERE IdCanal > 0";

        protected override string Procedimiento => "dbo.usp_CargarDimCanal";

        protected override string TipoTabla => "dbo.CanalTipo";

        protected override DimCanal Transformar(SqlDataReader reader) => new(
            reader.GetInt32(0),
            Transformacion.SepararPalabras(reader.Texto(1)));

        protected override int Clave(DimCanal fila) => fila.IdCanal;
    }
}
