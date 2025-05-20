using Api.AppDoar.Classes;
using Api.AppDoar.Dtos;
using Api.AppDoar.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using Microsoft.AspNetCore.StaticFiles;

namespace Api.AppDoar.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoacaoController : ControllerBase
    {
        private readonly DoacaoRepositorio _doacaoRepo;
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _config;

        public DoacaoController(
            DoacaoRepositorio doacaoRepo,
            IWebHostEnvironment env,
            IConfiguration config)
        {
            _doacaoRepo = doacaoRepo;
            _env = env;
            _config = config;
        }

        [HttpPost]
        public async Task<IActionResult> CriarDoacao([FromForm] CriarDoacaoDto doacaoDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (doacaoDto.Itens == null || !doacaoDto.Itens.Any())
                {
                    ModelState.AddModelError("Itens", "Pelo menos um item é obrigatório");
                    return BadRequest(ModelState);
                }

                var doacao = new Doacao
                {
                    doador_id = doacaoDto.doador_id,
                    instituicao_id = doacaoDto.instituicao_id,
                    horario_retirada = doacaoDto.horario_retirada,
                    endereco = doacaoDto.endereco,
                    tipo_entrega = doacaoDto.tipo_entrega,
                    usuario_id = doacaoDto.usuario_id,
                    created_at = DateTime.Now
                };

                var doacaoId = _doacaoRepo.Create(doacao);
                var imagensDoacaoUrls = new List<string>();
                if (doacaoDto.ImagensDoacao != null && doacaoDto.ImagensDoacao.Count > 0)
                {
                    // Converter List<IFormFile> para FormFileCollection
                    var formFileCollection = new FormFileCollection();
                    foreach (var file in doacaoDto.ImagensDoacao)
                    {
                        formFileCollection.Add(file);
                    }

                    imagensDoacaoUrls = await ProcessarUploadImagens(formFileCollection, "doacoes");

                    var imagensDoacao = imagensDoacaoUrls.Select((url, index) => new DoacaoImagem
                    {
                        doacao_id = (int)doacaoId,
                        url_imagem = url,
                        ordem = index,
                    }).ToList();

                    _doacaoRepo.CreateImagensDoacao(imagensDoacao);
                }

                var itensProcessados = new List<DoacaoItem>();
                foreach (var itemDto in doacaoDto.Itens.Where(i => i != null)) // Filtra itens nulos
                {
                    if (string.IsNullOrWhiteSpace(itemDto.Nome) ||
                        string.IsNullOrWhiteSpace(itemDto.Estado) ||
                        itemDto.Quantidade <= 0 ||
                        itemDto.SubcategoriaId <= 0)
                    {
                        continue; 
                    }

                    var item = new DoacaoItem
                    {
                        doacao_id = (int)doacaoId,
                        nome = itemDto.Nome,
                        descricao = itemDto.Descricao ?? string.Empty,
                        estado = itemDto.Estado,
                        quantidade = itemDto.Quantidade,
                        subcategoria_idsubcategoria = itemDto.SubcategoriaId
                    };

                    if (itemDto.ImagensItem != null && itemDto.ImagensItem.Count > 0)
                    {
                        var itemFormFileCollection = new FormFileCollection();
                        foreach (var file in itemDto.ImagensItem)
                        {
                            itemFormFileCollection.Add(file);
                        }

                        var imagensItemUrls = await ProcessarUploadImagens(itemFormFileCollection, "itens");
                        item.descricao += $"\n\nImagens do item:\n{string.Join("\n", imagensItemUrls)}";
                    }

                    itensProcessados.Add(item);
                }

                if (!itensProcessados.Any())
                {
                    return BadRequest("Nenhum item válido foi enviado");
                }

                _doacaoRepo.CreateItensDoacao(itensProcessados);

                return Ok(new
                {
                    id = doacaoId,
                    message = "Doação criada com sucesso!",
                    imagens = imagensDoacaoUrls
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao criar doação: {ex.Message}");
            }
        }

        [HttpGet("{id}/imagens")]
        public IActionResult GetImagensDoacao(int id)
        {
            try
            {
                var imagens = _doacaoRepo.GetImagensByDoacaoId(id);
                return Ok(imagens);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao buscar imagens: {ex.Message}");
            }
        }

        [HttpGet("imagem/{nomeArquivo}")]
        public IActionResult GetImagem(string nomeArquivo)
        {
            try
            {
                var uploadsPath = Path.Combine(_env.ContentRootPath, "Uploads");
                var filePath = Path.Combine(uploadsPath, nomeArquivo);

                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound();
                }

                var provider = new FileExtensionContentTypeProvider();
                if (!provider.TryGetContentType(filePath, out var contentType))
                {
                    contentType = "application/octet-stream";
                }

                var fileStream = System.IO.File.OpenRead(filePath);
                return File(fileStream, contentType);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao recuperar imagem: {ex.Message}");
            }
        }

        private async Task<List<string>> ProcessarUploadImagens(IFormFileCollection files, string subpasta)
        {
            var urls = new List<string>();

            if (files == null || !files.Any())
            {
                return urls;
            }

            var uploadsPath = Path.Combine(_env.ContentRootPath, "Uploads", subpasta);

            if (!Directory.Exists(uploadsPath))
            {
                Directory.CreateDirectory(uploadsPath);
            }

            foreach (var file in files)
            {
                if (file.Length == 0) continue;

                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

                if (string.IsNullOrEmpty(extension)) continue;
                if (!allowedExtensions.Contains(extension)) continue;

                var fileName = $"{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(uploadsPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var baseUrl = _config["BaseUrl"] ?? $"{Request.Scheme}://{Request.Host}";
                var relativePath = Path.Combine("Uploads", subpasta, fileName).Replace("\\", "/");
                urls.Add($"{baseUrl}/api/doacao/imagem/{relativePath}");
            }

            return urls;
        }
    }
}