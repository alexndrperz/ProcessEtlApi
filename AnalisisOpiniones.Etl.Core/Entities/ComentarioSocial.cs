namespace AnalisisOpiniones.Etl.Core.Entities
{
    public record ComentarioSocial(int IdComment, DateTime? Fecha, string? Comentario,
        Cliente Cliente, Producto Producto, Canal? Canal, Fuente? Fuente);
}
