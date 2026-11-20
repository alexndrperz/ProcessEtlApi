namespace AnalisisOpiniones.Etl.Core.Staging
{
    public class ComentarioSocialStaging
    {
        public int IdComment { get; set; }
        public int IdCliente { get; set; }
        public int IdProducto { get; set; }
        public string? Canal { get; set; }
        public DateTime? Fecha { get; set; }
        public string? Comentario { get; set; }
        public DateTime FechaExtraccion { get; set; }
    }
}
