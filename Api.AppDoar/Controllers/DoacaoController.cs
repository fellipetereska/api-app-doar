using Api.AppDoar.Classes;
using Api.AppDoar.Dtos;
using Api.AppDoar.Repositories;
using Api.AppDoar.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using System.IO;

namespace Api.AppDoar.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoacaoController : ControllerBase
    {
        private readonly DoacaoService _doacaoService;
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _config;
        private readonly DoacaoCategoriaService _doacaoCategoriaService;
        private readonly DoacaoRepositorio _doacaoRepo;

        public DoacaoController(
            DoacaoService doacaoService,
            IWebHostEnvironment env,
            IConfiguration config,
            DoacaoCategoriaService doacaoCategoriaService,
            DoacaoRepositorio doacaoRepo)
        {
            _doacaoService = doacaoService;
            _env = env;
            _config = config;
            _doacaoCategoriaService = doacaoCategoriaService;
            _doacaoRepo = doacaoRepo;
        }

        [HttpPost]
        public async Task<IActionResult> CriarDoacao([FromForm] CriarDoacaoDto doacaoDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                if (doacaoDto.Itens == null || !doacaoDto.Itens.Any())
                    return BadRequest("Pelo menos um item é obrigatório.");

                List<string> imagensUrls = new();
                if (doacaoDto.ImagensDoacao != null && doacaoDto.ImagensDoacao.Any())
                {
                    imagensUrls = await ProcessarUploadImagens(
                        doacaoDto.ImagensDoacao,
                        "doacao",
                        doacaoDto.InstituicaoId);
                }

                var doacaoId = _doacaoService.CriarDoacaoCompleta(doacaoDto, imagensUrls);

                return Ok(new
                {
                    Id = doacaoId,
                    Message = "Doação criada com sucesso!",
                    Imagens = imagensUrls
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao criar doação: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public IActionResult ObterDoacao(int id)
        {
            try
            {
                var doacao = _doacaoService.ObterDoacaoCompleta(id);
                return Ok(doacao);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao obter doação: {ex.Message}");
            }
        }

        [HttpGet("categorias/{instituicaoId}")]
        public IActionResult GetCategoriasPorInstituicao(int instituicaoId)
        {
            try
            {
                var categorias = _doacaoCategoriaService.GetCategoriasComSubcategorias(instituicaoId);
                return Ok(categorias);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao buscar categorias: {ex.Message}");
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

        [HttpGet("imagem/{*path}")]
        public IActionResult GetImagem(string path)
        {
            try
            {
                var uploadsPath = Path.Combine(_env.ContentRootPath, "Uploads");
                var filePath = Path.Combine(uploadsPath, path);

                if (!System.IO.File.Exists(filePath))
                    return NotFound();

                var provider = new FileExtensionContentTypeProvider();
                if (!provider.TryGetContentType(filePath, out var contentType))
                    contentType = "application/octet-stream";

                var fileStream = System.IO.File.OpenRead(filePath);
                return File(fileStream, contentType);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao recuperar imagem: {ex.Message}");
            }
        }

        private async Task<List<string>> ProcessarUploadImagens(
            IEnumerable<IFormFile> files,
            string subpasta,
            int? instituicaoId = null,
            int? doacaoId = null)
        {
            var urls = new List<string>();

            if (files == null || !files.Any())
                return urls;

            var basePath = Path.Combine(_env.ContentRootPath, "Uploads");

            if (instituicaoId.HasValue)
                basePath = Path.Combine(basePath, $"instituicao_{instituicaoId.Value}");

            if (doacaoId.HasValue)
                basePath = Path.Combine(basePath, $"doacao_{doacaoId.Value}");

            basePath = Path.Combine(basePath, subpasta);

            if (!Directory.Exists(basePath))
                Directory.CreateDirectory(basePath);

            foreach (var file in files)
            {
                if (file.Length == 0) continue;

                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

                if (string.IsNullOrEmpty(extension) || !allowedExtensions.Contains(extension))
                    continue;

                var fileName = $"{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(basePath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                    await file.CopyToAsync(stream);

                var pathSegments = new List<string> { "Uploads" };

                if (instituicaoId.HasValue)
                    pathSegments.Add($"instituicao_{instituicaoId.Value}");

                if (doacaoId.HasValue)
                    pathSegments.Add($"doacao_{doacaoId.Value}");

                pathSegments.Add(subpasta);
                pathSegments.Add(fileName);

                var relativePath = Path.Combine(pathSegments.ToArray()).Replace("\\", "/");
                var baseUrl = _config["BaseUrl"] ?? $"{Request.Scheme}://{Request.Host}";
                urls.Add($"{baseUrl}/api/doacao/imagem/{relativePath}");
            }

            return urls;
        }
    }
}