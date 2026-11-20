namespace AnalisisOpiniones.Etl.Core.Staging
{
    public class FuenteDatosStaging
    {
        public int IdFuente { get; set; }
        public string? TipoFuente { get; set; }
        public DateTime? FechaCarga { get; set; }
        public DateTime FechaExtraccion { get; set; }
    }
}
