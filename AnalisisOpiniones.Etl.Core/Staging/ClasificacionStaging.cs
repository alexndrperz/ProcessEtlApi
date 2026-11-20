namespace AnalisisOpiniones.Etl.Core.Staging
{
    public class ClasificacionStaging
    {
        public int IdClasificacion { get; set; }
        public string? Nombre { get; set; }
        public DateTime FechaExtraccion { get; set; }
    }
}
