using Api.AppDoar.Classes;
using Api.AppDoar.Dtos.doacao;
using Api.AppDoar.Enum;
using Api.AppDoar.Repositories;
using Api.AppDoar.Repositories.doacao;
using Api.AppDoar.Repositories.doador;
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
        private readonly EnderecoRepositorio _enderecoRepo;
        private readonly UsuarioRepositorio _usuarioRepo;



        public DoacaoController(
    DoacaoService doacaoService,
    IWebHostEnvironment env,
    IConfiguration config,
    DoacaoCategoriaService doacaoCategoriaService,
    DoacaoRepositorio doacaoRepo,
    EnderecoRepositorio enderecoRepo,
    UsuarioRepositorio usuarioRepo
            ) 
        {
            _doacaoService = doacaoService;
            _env = env;
            _config = config;
            _doacaoCategoriaService = doacaoCategoriaService;
            _doacaoRepo = doacaoRepo;
            _enderecoRepo = enderecoRepo;
            _usuarioRepo = usuarioRepo;
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CriarDoacao([FromForm] CriarDoacaoDto doacaoDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                if (doacaoDto.TipoEntrega == "retirada")
                {
                    var usuario = _usuarioRepo.GetById(doacaoDto.DoadorId);
                    if (string.IsNullOrEmpty(usuario?.logradouro))
                        return BadRequest("Usuário não possui endereço cadastrado");
                }

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
        public IActionResult GetDoacoesPorInstituicao(int instituicaoId, [FromQuery] StatusDoacao status = StatusDoacao.pendente)
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

        [HttpPatch("{id}/status-pedido")]
        public IActionResult AtualizarStatusPedido(int id, [FromBody] AtualizarStatusConfirmacaoDto dto)
        {
            try
            {
                var sucesso = _doacaoRepo.AtualizarStatusPedido(id, dto.status);
                if (!sucesso) return NotFound();

                var doacaoAtualizada = _doacaoRepo.GetById(id);
                return Ok(doacaoAtualizada);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao atualizar status do pedido: {ex.Message}");
            }
        }

        [HttpPatch("{id}/status-entrega")]
        public IActionResult AtualizarStatusEntrega(int id, [FromBody] AtualizarStatusEntregaDto dto)
        {
            try
            {
                var sucesso = _doacaoRepo.AtualizarStatusEntrega(id, dto.status_entrega);
                if (!sucesso) return NotFound();

                var doacaoAtualizada = _doacaoRepo.GetById(id);
                return Ok(doacaoAtualizada);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao atualizar status de entrega: {ex.Message}");
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