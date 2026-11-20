namespace AnalisisOpiniones.Etl.Core.Interfaces
{
    public interface IStagingWriter
    {
        Task GuardarAsync<T>(string tabla, IReadOnlyCollection<T> filas, CancellationToken cancellationToken);
    }
}
