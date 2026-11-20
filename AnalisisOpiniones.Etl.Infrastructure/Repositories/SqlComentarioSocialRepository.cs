using AnalisisOpiniones.Etl.Core.Entities;
using AnalisisOpiniones.Etl.Core.Interfaces;
using Microsoft.Data.SqlClient;

namespace AnalisisOpiniones.Etl.Infrastructure.Repositories
{
    public class SqlComentarioSocialRepository : IComentarioSocialRepository
    {
        private const string Consulta = @"
            SELECT s.IdComment, s.Fecha, CAST(s.Comentario AS NVARCHAR(MAX)) AS Comentario,
                   s.IdCliente, cl.Nombre AS ClienteNombre, cl.Email AS ClienteEmail,
                   s.IdProducto, p.Nombre AS ProductoNombre, p.Categoria AS ProductoCategoria,
                   s.IdCanal, ca.Nombre AS CanalNombre,
                   s.IdFuente, f.TipoFuente AS FuenteTipo, f.FechaCarga AS FuenteFechaCarga
            FROM dbo.SocialComments s
            LEFT JOIN stg.Clientes cl ON cl.IdCliente = s.IdCliente
            LEFT JOIN stg.Productos p ON p.IdProducto = s.IdProducto
            LEFT JOIN stg.Canales ca ON ca.IdCanal = s.IdCanal
            LEFT JOIN stg.FuentesDatos f ON f.IdFuente = s.IdFuente
            ORDER BY s.IdComment
            OFFSET @Offset ROWS FETCH NEXT @Tamano ROWS ONLY";

        private readonly string _connectionString;

        public SqlComentarioSocialRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IReadOnlyList<ComentarioSocial>> ObtenerPaginaAsync(int pagina, int tamano, CancellationToken cancellationToken)
        {
            var comentarios = new List<ComentarioSocial>();

            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);
            await using var command = new SqlCommand(Consulta, connection);
            command.Parameters.AddWithValue("@Offset", (pagina - 1) * tamano);
            command.Parameters.AddWithValue("@Tamano", tamano);

            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
                comentarios.Add(Mapear(reader));

            return comentarios;
        }

        private static ComentarioSocial Mapear(SqlDataReader reader) => new(
            reader.GetInt32(0),
            Fecha(reader, 1),
            Texto(reader, 2),
            new Cliente(reader.GetInt32(3), Texto(reader, 4), Texto(reader, 5)),
            new Producto(reader.GetInt32(6), Texto(reader, 7), Texto(reader, 8)),
            reader.IsDBNull(9) ? null : new Canal(reader.GetInt32(9), Texto(reader, 10)),
            reader.IsDBNull(11) ? null : new Fuente(reader.GetInt32(11), Texto(reader, 12), Fecha(reader, 13)));

        private static string? Texto(SqlDataReader reader, int i) => reader.IsDBNull(i) ? null : reader.GetString(i);

        private static DateTime? Fecha(SqlDataReader reader, int i) => reader.IsDBNull(i) ? null : reader.GetDateTime(i);
    }
}
