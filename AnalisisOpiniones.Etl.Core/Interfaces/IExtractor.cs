namespace AnalisisOpiniones.Etl.Core.Interfaces
{
    public interface IExtractor
    {
        string Nombre { get; }

        int Orden { get; }

        Task<int> ExtraerAsync(CancellationToken cancellationToken);
    }
}
