using Api.AppDoar.Classes;
using Api.AppDoar.Dtos.doacao;
using Api.AppDoar.Repositories.doacao;
using Api.AppDoar.Services.doacao;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using System.IO;

namespace Api.AppDoar.Controllers.doacao
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
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CriarDoacao([FromForm] CriarDoacaoDto doacaoDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var itensComArquivos = doacaoDto.Itens.Select(item =>
                    (new ItemDoacaoDto
                    {
                        Nome = item.Nome,
                        Descricao = item.Descricao,
                        Estado = item.Estado,
                        Quantidade = item.Quantidade,
                        SubcategoriaId = item.SubcategoriaId
                    },
                    item.ImagensItem?.ToList() ?? new List<IFormFile>())).ToList();

                var baseUrl = _config["BaseUrl"] ?? $"{Request.Scheme}://{Request.Host}";
                var doacaoId = await _doacaoService.CriarDoacaoCompleta(
                    doacaoDto,
                    itensComArquivos,
                    doacaoDto.InstituicaoId,
                    baseUrl);

                return Ok(new { Id = doacaoId, Message = "Doação criada com sucesso!" });
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

        [HttpGet("instituicao/{instituicaoId}")]
        public IActionResult GetDoacoesPorInstituicao(int instituicaoId, [FromQuery] string status = "pendente")
        {
            try
            {
                var doacoes = _doacaoRepo.GetByInstituicaoId(instituicaoId, status);
                return Ok(doacoes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao buscar doações: {ex.Message}");
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

        [HttpPatch("{id}/status")]
        public IActionResult AtualizarStatus(int id, [FromBody] AtualizarStatusDto dto)
        {
            try
            {
                var success = _doacaoRepo.UpdateStatus(id, dto.Status);
                if (!success) return NotFound();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao atualizar status: {ex.Message}");
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

        private async Task<List<string>> ProcessarUploadImagensItem(
        IEnumerable<IFormFile> files, int doacaoId,int itemId)
        {
            var urls = new List<string>();
            if (files == null || !files.Any())
                return urls;

            var basePath = Path.Combine(_env.ContentRootPath, "Uploads", $"doacao_{doacaoId}", $"item_{itemId}");

            if (!Directory.Exists(basePath))
                Directory.CreateDirectory(basePath);

            foreach (var file in files)
            {
                if (file.Length == 0) continue;

                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (string.IsNullOrEmpty(extension) || !new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" }.Contains(extension))
                    continue;

                var fileName = $"{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(basePath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                    await file.CopyToAsync(stream);

                var relativePath = $"doacao_{doacaoId}/item_{itemId}/{fileName}";
                var baseUrl = _config["BaseUrl"] ?? $"{Request.Scheme}://{Request.Host}";
                urls.Add($"{baseUrl}/api/doacao/imagem/{relativePath}");
            }

            return urls;
        }
    }
}