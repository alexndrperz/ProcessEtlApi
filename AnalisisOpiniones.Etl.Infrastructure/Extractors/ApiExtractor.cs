using System.Net.Http.Json;
using AnalisisOpiniones.Etl.Core.Interfaces;
using AnalisisOpiniones.Etl.Core.Staging;
using AnalisisOpiniones.Etl.Infrastructure.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AnalisisOpiniones.Etl.Infrastructure.Extractors
{
    public class ApiExtractor : IExtractor
    {
        public const string NombreCliente = "ApiComentarios";
        private const string TablaStaging = "stg.ComentariosSociales";

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ApiOptions _options;
        private readonly IStagingWriter _staging;
        private readonly ILogger<ApiExtractor> _logger;

        public ApiExtractor(IHttpClientFactory httpClientFactory, IOptions<FuentesOptions> options,
            IStagingWriter staging, ILogger<ApiExtractor> logger)
        {
            _httpClientFactory = httpClientFactory;
            _options = options.Value.Api;
            _staging = staging;
            _logger = logger;
        }

        public string Nombre => "API REST (Comentarios sociales)";

        public int Orden => 2;

        public async Task<int> ExtraerAsync(CancellationToken cancellationToken)
        {
            var client = _httpClientFactory.CreateClient(NombreCliente);
            var fechaExtraccion = DateTime.Now;
            var filas = new List<ComentarioSocialStaging>();

            for (var pagina = 1; ; pagina++)
            {
                var url = $"{_options.Endpoint}?page={pagina}&pageSize={_options.TamanoPagina}";
                var respuesta = await client.GetFromJsonAsync<List<ComentarioApi>>(url, cancellationToken) ?? [];
                _logger.LogDebug("{Fuente}: página {Pagina} con {Cantidad} registros.", Nombre, pagina, respuesta.Count);

                filas.AddRange(respuesta.Select(c => new ComentarioSocialStaging
                {
                    IdComment = c.IdComment,
                    IdCliente = c.Cliente.IdCliente,
                    IdProducto = c.Producto.IdProducto,
                    Canal = c.Canal?.Nombre,
                    Fecha = c.Fecha,
                    Comentario = c.Comentario,
                    FechaExtraccion = fechaExtraccion
                }));

                if (respuesta.Count < _options.TamanoPagina)
                    break;
            }

            await _staging.GuardarAsync(TablaStaging, filas, cancellationToken);
            return filas.Count;
        }

        private record ComentarioApi(int IdComment, DateTime? Fecha, string? Comentario,
            ClienteApi Cliente, ProductoApi Producto, CanalApi? Canal);

        private record ClienteApi(int IdCliente);

        private record ProductoApi(int IdProducto);

        private record CanalApi(string? Nombre);
    }
}
