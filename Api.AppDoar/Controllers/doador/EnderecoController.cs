using Api.AppDoar.Classes.doador;
using Api.AppDoar.Dtos.doador;
using Api.AppDoar.Repositories.doador;
using Microsoft.AspNetCore.Mvc;

namespace Api.AppDoar.Controllers.doador
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnderecoController : ControllerBase
    {
        private readonly EnderecoRepositorio _enderecoRepo;
        private readonly DoadorRepositorio _doadorRepo;

        public EnderecoController(EnderecoRepositorio enderecoRepo, DoadorRepositorio doadorRepo)
        {
            _enderecoRepo = enderecoRepo;
            _doadorRepo = doadorRepo;
        }

        [HttpPost]
        public IActionResult Post([FromBody] AdicionarEnderecoDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var doador = _doadorRepo.GetById(dto.UsuarioId);
                if (doador == null)
                    return BadRequest("O usuário informado não é um doador válido");

                var endereco = new Endereco
                {
                    usuario_id = dto.UsuarioId,
                    logradouro = dto.Logradouro,
                    numero = dto.Numero,
                    complemento = dto.Complemento,
                    bairro = dto.Bairro,
                    cidade = dto.Cidade,
                    uf = dto.Uf,
                    cep = dto.Cep,
                    principal = dto.Principal,
                    latitude = dto.Latitude,
                    longitude = dto.Longitude
                };

                var id = _enderecoRepo.Create(endereco);
                return CreatedAtAction(nameof(GetByUsuario), new { usuarioId = dto.UsuarioId }, new { Id = id });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao cadastrar endereço: {ex.Message}");
            }
        }

        [HttpGet("usuario/{usuarioId}")]
        public IActionResult GetByUsuario(int usuarioId)
        {
            try
            {
                var enderecos = _enderecoRepo.GetByUsuario(usuarioId);
                return Ok(enderecos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao buscar endereços: {ex.Message}");
            }
        }

        [HttpGet("usuario/{usuarioId}/principal")]
        public IActionResult GetPrincipalByUsuario(int usuarioId)
        {
            try
            {
                var endereco = _enderecoRepo.GetPrincipalByUsuario(usuarioId);
                if (endereco == null)
                    return NotFound();

                return Ok(endereco);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao buscar endereço principal: {ex.Message}");
            }
        }
    }
}