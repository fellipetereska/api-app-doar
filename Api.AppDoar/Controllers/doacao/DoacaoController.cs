using Api.AppDoar.Classes;
using Api.AppDoar.Dtos.doacao;
using Api.AppDoar.Repositories.assistido;
using Api.AppDoar.Repositories.doacao;
using Api.AppDoar.Repositories.instituicao;
using Api.AppDoar.Services.doacao;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using System.Diagnostics;
using System.IO;
using System.Text;

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
        private readonly AssistidoRepositorio _assistidoRepo;
        private readonly EntregasRepositorio _entregaRepo;
        private readonly InstituicaoRepositorio _instituicaoRepo;
        private readonly ILogger<DoacaoController> _logger;

        public DoacaoController(
            DoacaoService doacaoService,
            IWebHostEnvironment env,
            IConfiguration config,
            DoacaoCategoriaService doacaoCategoriaService,
            DoacaoRepositorio doacaoRepo,
            AssistidoRepositorio assistidoRepo,
            EntregasRepositorio entregaRepo,
            InstituicaoRepositorio instituicaoRepo,
            ILogger<DoacaoController> logger)
        {
            _doacaoService = doacaoService;
            _env = env;
            _config = config;
            _doacaoCategoriaService = doacaoCategoriaService;
            _doacaoRepo = doacaoRepo;
            _assistidoRepo = assistidoRepo;
            _entregaRepo = entregaRepo;
            _instituicaoRepo = instituicaoRepo;
            _logger = logger;

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
                {
                    var itemDto = new ItemDoacaoDto
                    {
                        Nome = item.Nome,
                        Descricao = item.Descricao,
                        Estado = item.Estado,
                        Quantidade = item.Quantidade,
                        SubcategoriaId = item.SubcategoriaId,
                        ImagensItem = item.ImagensItem
                    };

                    return (itemDto, item.ImagensItem ?? new List<IFormFile>());
                }).ToList();

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
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("todas/{instituicaoId}")]
        public IActionResult GetTodasDoacoes(int instituicaoId)
        {
            try
            {
                var recebidas = _doacaoRepo.GetByInstituicaoId(instituicaoId, "recebidas");
                var aguardando = _doacaoRepo.GetByInstituicaoId(instituicaoId, "aguardando");
                var gerais = _doacaoRepo.GetDoacoesSemInstituicao();

                var anoAtual = DateTime.Now.Year;
                var mesAtual = DateTime.Now.Month;

                var totalRecebidasAno = _doacaoRepo.CountDoacoesRecebidasPorAno(instituicaoId, anoAtual);
                var totalRecebidasMes = _doacaoRepo.CountDoacoesRecebidasPorMes(instituicaoId, anoAtual, mesAtual);

                var totalEntregasAno = _entregaRepo.CountEntregasPorAno(instituicaoId, anoAtual);
                var totalEntregasMes = _entregaRepo.CountEntregasPorMes(instituicaoId, anoAtual, mesAtual);

                var totalListaEspera = _assistidoRepo.CountListaEspera(instituicaoId);

                return Ok(new
                {
                    recebidas,
                    aguardando,
                    gerais,
                    indicadores = new
                    {
                        recebidasAno = totalRecebidasAno,
                        recebidasMes = totalRecebidasMes,
                        entregasAno = totalEntregasAno,
                        entregasMes = totalEntregasMes,
                        listaEspera = totalListaEspera
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // Código sem Automação
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
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // Código com Automação
        /*
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> AtualizarStatus(int id, [FromBody] AtualizarStatusDto dto)
        {
            try
            {
                if (dto == null || string.IsNullOrEmpty(dto.Status))
                {
                    return BadRequest(new { message = "Status é obrigatório" });
                }

                var doacao = _doacaoRepo.GetById(id);
                if (doacao == null)
                {
                    return NotFound(new { message = "Doação não encontrada" });
                }

                var updateSuccess = _doacaoRepo.UpdateStatus(id, dto.Status);
                if (!updateSuccess)
                {
                    _logger.LogError($"Falha ao atualizar status da doação {id}");
                    return StatusCode(500, new { message = "Falha ao atualizar status no banco de dados" });
                }

                if (dto.Status.Equals("Aceita", StringComparison.OrdinalIgnoreCase))
                {
                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            var instituicao = _instituicaoRepo.GetById(doacao.instituicao_id.Value);
                            if (instituicao != null)
                            {
                                var mensagem = $"Olá! Informamos que sua doação foi {dto.Status.ToLower()} pela nossa instituição ({instituicao.nome}). Agradecemos imensamente o seu apoio.\n\nCaso você tenha selecionado a opção de retirada da doação, entraremos em contato em breve para agendar.\n\n*Mensagem automática*";

                                await EnviarMensagemWhatsApp("5543991052073", mensagem);
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, $"Erro no envio WhatsApp para doação {id}");
                        }
                    });
                }

                return Ok(new { message = "Status atualizado com sucesso" });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao atualizar status da doação {id}");
                return StatusCode(500, new { message = "Erro interno no servidor" });
            }
        }
        private async Task EnviarMensagemWhatsApp(string numero, string mensagem)
        {
            var scriptPath = Path.Combine(Directory.GetCurrentDirectory(),
                                       "Automation", "Whatsapp", "bot.py");

            var psi = new ProcessStartInfo
            {
                FileName = "python",
                Arguments = $"\"{scriptPath}\" \"{numero}\" \"{mensagem}\"",
                WorkingDirectory = Path.GetDirectoryName(scriptPath),
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (var process = new Process { StartInfo = psi })
            {
                process.Start();
                await process.WaitForExitAsync();

                if (process.ExitCode != 0)
                {
                    var error = await process.StandardError.ReadToEndAsync();
                    throw new Exception($"Erro no envio WhatsApp: {error}");
                }
            }
        }
        */

        [HttpPost("{id}/aceitar")]
        public IActionResult AceitarDoacao(int id, [FromBody] int instituicaoId)
        {
            try
            {
                var success = _doacaoRepo.AceitarDoacao(id, instituicaoId);
                if (!success) return NotFound();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }


        [HttpPatch("{id}/status_entrega")]
        public IActionResult AtualizarStatusEntrega(int id, [FromBody] AtualizarStatusEntregaDto dto)
        {
            try
            {
                var success = _doacaoRepo.UpdateStatusEntrega(id, dto.Status, dto.Itens);
                if (!success) return NotFound();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
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