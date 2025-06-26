using Api.AppDoar.Repositories;
using Api.AppDoar.Repositories.assistido;
using Api.AppDoar.Repositories.doacao;
using Api.AppDoar.Repositories.doador;
using Api.AppDoar.Repositories.instituicao;
using Api.AppDoar.Services;
using Api.AppDoar.Services.doacao;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;
using QuestPDF.Infrastructure;
using System.Net;
using System.Net.Http.Headers;

namespace Api.AppDoar
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Licença QuestPDF
            QuestPDF.Settings.License = LicenseType.Community;
            QuestPDF.Settings.EnableDebugging = true;

            // Controllers
            builder.Services.AddControllers();

            // Repositórios
            // Repositórios
            builder.Services.AddScoped<UsuarioRepositorio>();
            builder.Services.AddScoped<InstituicaoRepositorio>();
            builder.Services.AddScoped<DoacaoRepositorio>();
            builder.Services.AddScoped<CategoriaRepositorio>();
            builder.Services.AddScoped<AssistidoRepositorio>();
            builder.Services.AddScoped<EntregasRepositorio>();
            builder.Services.AddScoped<DoadorRepositorio>();

            // Serviços
            builder.Services.AddScoped<DoacaoService>();


            // Geocodificação
            builder.Services.AddHttpClient<IGeocodificacaoService, GeocodificacaoService>(client =>
            {
                client.Timeout = TimeSpan.FromSeconds(30);
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
            })
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            });

            // Formulários (uploads)
            builder.Services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = 52428800;
                options.MultipartHeadersLengthLimit = 52428800;
            });

            // Swagger
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "API AppDoar",
                    Version = "v1",
                    Description = "API para gerenciamento de doações"
                });

                c.CustomSchemaIds(x => x.Name);
            });

            // Roteamento e CORS
            builder.Services.AddRouting(options =>
            {
                options.LowercaseUrls = true;
            });

            builder.Services.AddCors(); // Configurado globalmente no app

            // Kestrel na porta 5005
            builder.WebHost.ConfigureKestrel(options =>
            {
                options.ListenAnyIP(5005);
            });

            var app = builder.Build();

            // CORS global
            app.UseCors(policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader();
            });

            // Swagger disponível sempre
            app.UseSwagger();
            app.UseSwaggerUI();

            // Pasta de uploads
            var uploadsPath = Path.Combine(app.Environment.ContentRootPath, "Uploads");
            if (!Directory.Exists(uploadsPath))
                Directory.CreateDirectory(uploadsPath);

            var logosPath = Path.Combine(uploadsPath, "logos");
            if (!Directory.Exists(logosPath))
                Directory.CreateDirectory(logosPath);

            // Servir arquivos estáticos (imagens, PDFs, etc.)
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(uploadsPath),
                RequestPath = "/Uploads",
                ContentTypeProvider = new FileExtensionContentTypeProvider()
            });

            // Middleware de autorização
            app.UseAuthorization();

            // Endpoints
            app.MapControllers();

            app.Run();
        }
    }
}
