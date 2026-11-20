namespace AnalisisOpiniones.Etl.Core.Staging
{
    public class EncuestaStaging
    {
        public string IdOpinion { get; set; } = string.Empty;
        public string IdCliente { get; set; } = string.Empty;
        public string IdProducto { get; set; } = string.Empty;
        public string Fecha { get; set; } = string.Empty;
        public string Comentario { get; set; } = string.Empty;
        public string Clasificacion { get; set; } = string.Empty;
        public string PuntajeSatisfaccion { get; set; } = string.Empty;
        public string Fuente { get; set; } = string.Empty;
        public DateTime FechaExtraccion { get; set; }
    }
}
