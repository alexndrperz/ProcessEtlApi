using AnalisisOpiniones.Etl.Core.Dimensiones;
using AnalisisOpiniones.Etl.Core.Interfaces;
using AnalisisOpiniones.Etl.Infrastructure.Comun;
using AnalisisOpiniones.Etl.Infrastructure.Options;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace AnalisisOpiniones.Etl.Infrastructure.Dimensiones
{
    public class DimFuenteLoader : DimensionLoaderBase<DimFuente>
    {
        public DimFuenteLoader(IConfiguration configuration, IOptions<CargaOptions> options, IDimensionWriter writer)
            : base(configuration, options, writer)
        {
        }

        public override string Nombre => "DimFuente";

        protected override string Consulta => @"
            SELECT IdFuente, TipoFuente, FechaCarga
            FROM stg.FuentesDatos
            WHERE IdFuente > 0";

        protected override string Procedimiento => "dbo.usp_CargarDimFuente";

        protected override string TipoTabla => "dbo.FuenteTipo";

        protected override DimFuente Transformar(SqlDataReader reader) => new(
            reader.GetInt32(0),
            Transformacion.Texto(reader.Texto(1)),
            reader.Fecha(2));

        protected override int Clave(DimFuente fila) => fila.IdFuente;
    }
}
