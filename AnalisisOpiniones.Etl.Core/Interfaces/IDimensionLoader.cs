using AnalisisOpiniones.Etl.Core.Dimensiones;

namespace AnalisisOpiniones.Etl.Core.Interfaces
{
    public interface IDimensionLoader
    {
        string Nombre { get; }

        Task<ResultadoCarga> CargarAsync(CancellationToken cancellationToken);
    }
}
