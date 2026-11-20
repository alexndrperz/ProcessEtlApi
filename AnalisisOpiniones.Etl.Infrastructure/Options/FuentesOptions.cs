namespace AnalisisOpiniones.Etl.Infrastructure.Options
{
    public class FuentesOptions
    {
        public const string Seccion = "Fuentes";

        public CsvOptions Csv { get; set; } = new();
        public BaseDatosOptions BaseDatos { get; set; } = new();
        public ApiOptions Api { get; set; } = new();
    }

    public class CsvOptions
    {
        public bool Habilitado { get; set; } = true;
        public string RutaArchivo { get; set; } = string.Empty;
    }

    public class BaseDatosOptions
    {
        public bool Habilitado { get; set; } = true;
        public int TimeoutSegundos { get; set; } = 60;
    }

    public class ApiOptions
    {
        public bool Habilitado { get; set; } = true;
        public string UrlBase { get; set; } = string.Empty;
        public string Endpoint { get; set; } = "/api/social-comments";
        public int TamanoPagina { get; set; } = 100;
        public int TimeoutSegundos { get; set; } = 30;
    }

    public class StagingOptions
    {
        public const string Seccion = "Staging";

        public int TamanoLote { get; set; } = 5000;
    }
}
