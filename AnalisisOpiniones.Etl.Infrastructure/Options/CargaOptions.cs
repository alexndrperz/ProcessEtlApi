namespace AnalisisOpiniones.Etl.Infrastructure.Options
{
    public class CargaOptions
    {
        public const string Seccion = "Carga";

        public bool Habilitado { get; set; } = true;
        public int TimeoutSegundos { get; set; } = 60;
    }
}
