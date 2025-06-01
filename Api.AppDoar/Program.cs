using Api.AppDoar.Repositories.doacao;
using Api.AppDoar.Repositories.doador;
using Api.AppDoar.Services.doacao;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

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

                c.OperationFilter<SwaggerFileUploadOperationFilter>();

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

            app.UseCors("AllowAll"); 


            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }

    public class SwaggerFileUploadOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (context.MethodInfo.Name == "CriarDoacao")
            {
                operation.RequestBody.Content["multipart/form-data"].Schema = new OpenApiSchema
                {
                    Type = "object",
                    Properties = new Dictionary<string, OpenApiSchema>
                    {
                        ["DoadorId"] = new() { Type = "integer", Format = "int32" },
                        ["InstituicaoId"] = new() { Type = "integer", Format = "int32" },
                        ["TipoEntrega"] = new() { Type = "string" },
                        ["HorarioRetirada"] = new() { Type = "string" },
                        ["EnderecoId"] = new() { Type = "integer", Format = "int32" },
                        ["NovoEndereco.UsuarioId"] = new() { Type = "integer", Format = "int32" },
                        ["NovoEndereco.Logradouro"] = new() { Type = "string" },
                        ["NovoEndereco.Numero"] = new() { Type = "string" },
                        ["NovoEndereco.Complemento"] = new() { Type = "string" },
                        ["NovoEndereco.Bairro"] = new() { Type = "string" },
                        ["NovoEndereco.Cidade"] = new() { Type = "string" },
                        ["NovoEndereco.Uf"] = new() { Type = "string" },
                        ["NovoEndereco.Cep"] = new() { Type = "string" },
                        ["NovoEndereco.Principal"] = new() { Type = "boolean" },
                        ["Itens[0].Nome"] = new() { Type = "string" },
                        ["Itens[0].Descricao"] = new() { Type = "string" },
                        ["Itens[0].Estado"] = new() { Type = "string" },
                        ["Itens[0].Quantidade"] = new() { Type = "integer", Format = "int32" },
                        ["Itens[0].SubcategoriaId"] = new() { Type = "integer", Format = "int32" },
                        ["Itens[0].ImagensItem"] = new()
                        {
                            Type = "array",
                            Items = new OpenApiSchema
                            {
                                Type = "string",
                                Format = "binary"
                            }
                        }
                    },
                    Required = new HashSet<string> { "DoadorId", "InstituicaoId", "TipoEntrega" }
                };
            }
        }
    }
}