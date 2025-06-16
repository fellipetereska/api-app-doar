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
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using QuestPDF.Infrastructure;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Net;
using System.Net.Http.Headers;

namespace Api.AppDoar
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();

            // Defina a licença do QuestPDF
            QuestPDF.Settings.License = LicenseType.Community;
            QuestPDF.Settings.EnableDebugging = true;

            // Repositórios
            builder.Services.AddScoped<DoacaoRepositorio>();
            builder.Services.AddScoped<CategoriaRepositorio>();
            builder.Services.AddScoped<AssistidoRepositorio>();
            builder.Services.AddScoped<EntregasRepositorio>();
            builder.Services.AddScoped<DoadorRepositorio>();
            builder.Services.AddScoped<UsuarioRepositorio>();
            builder.Services.AddScoped<InstituicaoRepositorio>();


            // Serviços
            builder.Services.AddScoped<DoacaoService>();

            // Configuração do HttpClient para o serviço de geocodificação
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


            builder.Services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = 52428800; 
                options.MultipartHeadersLengthLimit = 52428800; 
            });

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

            // Força o ASP.NET a sempre gerar as URLs minúsculas
            builder.Services.AddRouting(options =>
            {
                options.LowercaseUrls = true;
            });

            // Permitindo o acesso do localhost:3000
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            var app = builder.Build();

            // Criar pasta de uploads se não existir
            var uploadsPath = Path.Combine(app.Environment.ContentRootPath, "Uploads");
            if (!Directory.Exists(uploadsPath))
            {
                Directory.CreateDirectory(uploadsPath);
            }

            // Criar pasta de logos se não existir
            var logosPath = Path.Combine(uploadsPath, "logos");
            if (!Directory.Exists(logosPath))
            {
                Directory.CreateDirectory(logosPath);
            }

            // Habilitar CORS
            app.UseCors("CorsPolicy");

            // Configurar serviço de arquivos estáticos
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(uploadsPath),
                RequestPath = "/Uploads",
                ContentTypeProvider = new FileExtensionContentTypeProvider()
            });

            string novoHash = BCrypt.Net.BCrypt.HashPassword("1234", workFactor: 11);
            Console.WriteLine(novoHash);

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors("AllowAll");

            //app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }

}