namespace AnalisisOpiniones.Etl.Core.Staging
{
    public class ResenaWebStaging
    {
        public int IdReview { get; set; }
        public int IdCliente { get; set; }
        public int IdProducto { get; set; }
        public DateTime? Fecha { get; set; }
        public string? Comentario { get; set; }
        public decimal? Rating { get; set; }
        public int? IdFuente { get; set; }
        public DateTime FechaExtraccion { get; set; }
    }
}
