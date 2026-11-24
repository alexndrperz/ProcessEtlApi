namespace AnalisisOpiniones.Etl.Core.Dimensiones
{
    public record ResultadoCarga(int Leidas, int Insertadas, int Actualizadas)
    {
        public int SinCambios => Math.Max(Leidas - Insertadas - Actualizadas, 0);
    }
}
