namespace AnalisisOpiniones.Etl.Core.Staging
{
    public class CanalStaging
    {
        public int IdCanal { get; set; }
        public string? Nombre { get; set; }
        public DateTime FechaExtraccion { get; set; }
    }
}
