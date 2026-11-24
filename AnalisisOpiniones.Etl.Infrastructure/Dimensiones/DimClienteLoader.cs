using AnalisisOpiniones.Etl.Core.Dimensiones;
using AnalisisOpiniones.Etl.Core.Interfaces;
using AnalisisOpiniones.Etl.Infrastructure.Comun;
using AnalisisOpiniones.Etl.Infrastructure.Options;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace AnalisisOpiniones.Etl.Infrastructure.Dimensiones
{
    public class DimClienteLoader : DimensionLoaderBase<DimCliente>
    {
        public DimClienteLoader(IConfiguration configuration, IOptions<CargaOptions> options, IDimensionWriter writer)
            : base(configuration, options, writer)
        {
        }

        public override string Nombre => "DimCliente";

        protected override string Consulta => @"
            SELECT IdCliente, Nombre, Email
            FROM stg.Clientes
            WHERE IdCliente > 0";

        protected override string Procedimiento => "dbo.usp_CargarDimCliente";

        protected override string TipoTabla => "dbo.ClienteTipo";

        protected override DimCliente Transformar(SqlDataReader reader) => new(
            reader.GetInt32(0),
            Transformacion.Texto(reader.Texto(1)),
            Transformacion.Email(reader.Texto(2)));

        protected override int Clave(DimCliente fila) => fila.IdCliente;
    }
}
