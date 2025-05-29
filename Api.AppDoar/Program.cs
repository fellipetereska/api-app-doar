using Api.AppDoar.Repositories;
using Api.AppDoar.Services;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;

namespace Api.AppDoar
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();


            // Repositórios
            builder.Services.AddScoped<DoacaoRepositorio>();
            builder.Services.AddScoped<CategoriaRepositorio>();
            builder.Services.AddScoped<EnderecoRepositorio>();

            // Serviços
            builder.Services.AddScoped<DoacaoService>();
            builder.Services.AddScoped<DoacaoCategoriaService>();


            // Configuração para upload de arquivos grandes
            builder.Services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = 52428800; // 50MB
                options.MultipartHeadersLengthLimit = 52428800; // 50MB
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

             
                c.CustomSchemaIds(x => x.FullName);
            });

            // Força o ASP.NET a sempre gerar as URLs minúsculas
            builder.Services.AddRouting(options =>
            {
                options.LowercaseUrls = true;
            });

            // Permitindo o acesso do localhost:3000
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", policy =>
                {
                    policy
                        .WithOrigins("http://localhost:3000")
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });

            var app = builder.Build();

            // Criar pasta de uploads se não existir
            var uploadsPath = Path.Combine(app.Environment.ContentRootPath, "Uploads");
            if (!Directory.Exists(uploadsPath))
            {
                Directory.CreateDirectory(uploadsPath);
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

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}