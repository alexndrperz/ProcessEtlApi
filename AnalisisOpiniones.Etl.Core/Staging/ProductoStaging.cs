namespace AnalisisOpiniones.Etl.Core.Staging
{
    public class ProductoStaging
    {
        public int IdProducto { get; set; }
        public string? Nombre { get; set; }
        public string? Categoria { get; set; }
        public DateTime FechaExtraccion { get; set; }
    }
}
