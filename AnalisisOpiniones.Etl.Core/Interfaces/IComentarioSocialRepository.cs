using AnalisisOpiniones.Etl.Core.Entities;

namespace AnalisisOpiniones.Etl.Core.Interfaces
{
    public interface IComentarioSocialRepository
    {
        Task<IReadOnlyList<ComentarioSocial>> ObtenerPaginaAsync(int pagina, int tamano, CancellationToken cancellationToken);
    }
}
