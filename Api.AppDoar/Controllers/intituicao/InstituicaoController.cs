using Api.AppDoar.Classes;
using Api.AppDoar.Classes.instituicao;
using Api.AppDoar.Dtos.instituicao;
using Api.AppDoar.Repositories;
using Api.AppDoar.Repositories.instituicao;
using Api.AppDoar.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.AppDoar.Controllers.instituicao
{
    [ApiController]
    [Route("api/[controller]")]
    public class InstituicaoController : Controller
    {
        private readonly IGeocodificacaoService _geocodificacaoService;
        private readonly UsuarioRepositorio _userRepo;
        private readonly InstituicaoRepositorio _instituicaoRepo;


        public InstituicaoController(IGeocodificacaoService geocodificacaoService)
        {
            _geocodificacaoService = geocodificacaoService;
            _userRepo = new UsuarioRepositorio();
            _instituicaoRepo = new InstituicaoRepositorio();
        }

        [HttpPost("registrar")]
        public async Task<IActionResult> RegistrarInstituicao([FromForm] CadastroInstituicaoDto dto, IFormFile logo)
        {
            var userRepo = new UsuarioRepositorio();
            var InstituicaoRepo = new InstituicaoRepositorio();

            var instituicaoExiste = InstituicaoRepo.BuscarPorCnpj(dto.instituicao.cnpj);

            if (instituicaoExiste != null)
                return Conflict(new { message = "Instituição já existe!" });

            string logoPath = null;
            if (logo != null && logo.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", "logos");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var uniqueFileName = Guid.NewGuid().ToString() + "_" + logo.FileName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await logo.CopyToAsync(fileStream);
                }

                logoPath = Path.Combine("logos", uniqueFileName);
            }

            var (latitude, longitude) = await _geocodificacaoService.ObterCoordenadasPorCepAsync(dto.instituicao.cep);

            var novaInstituicao = new Instituicao
            {
                nome = dto.instituicao.nome,
                cnpj = dto.instituicao.cnpj,
                telefone = dto.instituicao.telefone,
                cep = dto.instituicao.cep,
                logradouro = dto.instituicao.logradouro,
                endereco = dto.instituicao.endereco,
                numero = dto.instituicao.numero,
                complemento = dto.instituicao.complemento,
                bairro = dto.instituicao.bairro,
                cidade = dto.instituicao.cidade,
                uf = dto.instituicao.uf,
                descricao = dto.instituicao.descricao,
                latitude = latitude,
                longitude = longitude,
                logo_path = logoPath 
            };

            try
            {
                var instituicaoId = _instituicaoRepo.Create(novaInstituicao);

                var instituicaoCriada = _instituicaoRepo.GetById((int)instituicaoId); 

                var novoUsuario = new Usuario
                {
                    nome = dto.usuario.nome,
                    email = dto.usuario.email,
                    senha = BCrypt.Net.BCrypt.HashPassword(dto.usuario.senha),
                    tipo_documento = dto.usuario.tipo_documento,
                    documento = dto.usuario.documento,
                    role = "instituicao",
                    instituicao_id = instituicaoCriada.id, 
                    status = 1
                };

                var usuarioCriado = userRepo.Create(novoUsuario);

                return Ok(new
                {
                    success = true,
                    message = "Cadastro realizado com sucesso",
                    instituicaoId = instituicaoCriada.id,
                    usuarioId = usuarioCriado
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpPut("{id}")]
        public IActionResult Editar(int id, [FromBody] Instituicao instituicao)
        {
            var instituicaoRepo = new InstituicaoRepositorio();

            var instituicaoExistente = instituicaoRepo.GetById(id);

            try
            {
                if (instituicaoExistente == null)
                    return NotFound(new { message = "Instituição não encontrada." });

                instituicao.id = id;
                instituicaoRepo.Update(instituicao);
                return Ok(new { message = "Instituição atualizada com sucesso." });

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public IActionResult BuscarPorId(int id)
        {
            var instituicaoRepo = new InstituicaoRepositorio();

            var instituicao = instituicaoRepo.GetById(id);
            try
            {
                if (instituicao == null)
                    return NotFound(new { message = "Instituição não encontrada." });

                return Ok(instituicao);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult ListarTodas()
        {
            var instituicaoRepo = new InstituicaoRepositorio();

            try
            {
                var instituicoes = instituicaoRepo.GetAll();
                return Ok(instituicoes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

    }
}
