using AnalisisOpiniones.Etl.Core.Dimensiones;

namespace AnalisisOpiniones.Etl.Core.Interfaces
{
    public interface IDimensionWriter
    {
        Task<ResultadoCarga> CargarAsync<T>(string procedimiento, string tipoTabla, IReadOnlyCollection<T> filas, CancellationToken cancellationToken);

        Task<ResultadoCarga> EjecutarAsync(string procedimiento, IReadOnlyDictionary<string, object> parametros, CancellationToken cancellationToken);
    }
}
