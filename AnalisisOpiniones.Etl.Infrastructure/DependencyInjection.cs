using System.Net.Http.Headers;
using AnalisisOpiniones.Etl.Core.Interfaces;
using AnalisisOpiniones.Etl.Infrastructure.Extractors;
using AnalisisOpiniones.Etl.Infrastructure.Options;
using AnalisisOpiniones.Etl.Infrastructure.Repositories;
using AnalisisOpiniones.Etl.Infrastructure.Staging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AnalisisOpiniones.Etl.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddExtraccion(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<FuentesOptions>(configuration.GetSection(FuentesOptions.Seccion));
            services.Configure<StagingOptions>(configuration.GetSection(StagingOptions.Seccion));

            services.AddSingleton<IStagingWriter, SqlStagingWriter>();

            var fuentes = configuration.GetSection(FuentesOptions.Seccion).Get<FuentesOptions>() ?? new FuentesOptions();

            if (fuentes.Csv.Habilitado)
                services.AddSingleton<IExtractor, CsvExtractor>();

            if (fuentes.BaseDatos.Habilitado)
            {
                services.AddSingleton<IExtractor, DatabaseExtractor>();
                services.AddSingleton<IExtractor, ClienteExtractor>();
                services.AddSingleton<IExtractor, ProductoExtractor>();
                services.AddSingleton<IExtractor, FuenteDatosExtractor>();
                services.AddSingleton<IExtractor, ClasificacionExtractor>();
                services.AddSingleton<IExtractor, CanalExtractor>();
            }

            if (fuentes.Api.Habilitado)
            {
                services.AddHttpClient(ApiExtractor.NombreCliente, client =>
                {
                    client.BaseAddress = new Uri(fuentes.Api.UrlBase);
                    client.Timeout = TimeSpan.FromSeconds(fuentes.Api.TimeoutSegundos);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                });
                services.AddSingleton<IExtractor, ApiExtractor>();
            }

            return services;
        }

        public static IServiceCollection AddComentariosSociales(this IServiceCollection services, string connectionString)
        {
            services.AddSingleton<IComentarioSocialRepository>(new SqlComentarioSocialRepository(connectionString));
            return services;
        }
    }
}
