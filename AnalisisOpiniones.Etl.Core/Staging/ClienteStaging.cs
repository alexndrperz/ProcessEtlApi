namespace AnalisisOpiniones.Etl.Core.Staging
{
    public class ClienteStaging
    {
        public int IdCliente { get; set; }
        public string? Nombre { get; set; }
        public string? Email { get; set; }
        public DateTime FechaExtraccion { get; set; }
    }
}
