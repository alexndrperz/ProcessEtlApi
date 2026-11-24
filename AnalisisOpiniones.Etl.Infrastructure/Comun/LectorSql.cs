using Microsoft.Data.SqlClient;

namespace AnalisisOpiniones.Etl.Infrastructure.Comun
{
    public static class LectorSql
    {
        public static string? Texto(this SqlDataReader reader, int i) => reader.IsDBNull(i) ? null : reader.GetString(i);

        public static DateTime? Fecha(this SqlDataReader reader, int i) => reader.IsDBNull(i) ? null : reader.GetDateTime(i);
    }
}
