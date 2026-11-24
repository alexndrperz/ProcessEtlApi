using System.Text.RegularExpressions;

namespace AnalisisOpiniones.Etl.Infrastructure.Comun
{
    public static partial class Transformacion
    {
        public const string SinDato = "No definido";

        public static string Texto(string? valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
                return SinDato;

            return EspaciosRepetidos().Replace(valor.Trim(), " ");
        }

        public static string Email(string? valor) =>
            string.IsNullOrWhiteSpace(valor) ? SinDato : valor.Trim().ToLowerInvariant();

        public static string SepararPalabras(string? valor) =>
            Texto(MayusculaInterna().Replace(valor ?? string.Empty, "$1 $2"));

        [GeneratedRegex(@"\s+")]
        private static partial Regex EspaciosRepetidos();

        [GeneratedRegex(@"(\p{Ll})(\p{Lu})")]
        private static partial Regex MayusculaInterna();
    }
}
