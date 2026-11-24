using System.Data;

namespace AnalisisOpiniones.Etl.Infrastructure.Comun
{
    public static class TablaDatos
    {
        public static DataTable Crear<T>(IEnumerable<T> filas)
        {
            var propiedades = typeof(T).GetProperties();
            var tabla = new DataTable();

            foreach (var p in propiedades)
                tabla.Columns.Add(p.Name, Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType);

            foreach (var fila in filas)
                tabla.Rows.Add(propiedades.Select(p => p.GetValue(fila) ?? DBNull.Value).ToArray());

            return tabla;
        }
    }
}
