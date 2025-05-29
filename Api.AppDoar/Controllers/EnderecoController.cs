using Api.AppDoar.Classes;
using Api.AppDoar.Dtos;
using Api.AppDoar.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Api.AppDoar.Controllers
{
    
        [Route("api/[controller]")]
        [ApiController]
        public class EnderecoController : ControllerBase
        {
            private readonly EnderecoRepositorio _enderecoRepo;

            public EnderecoController(EnderecoRepositorio enderecoRepo)
            {
                _enderecoRepo = enderecoRepo;
            }

            [HttpGet]
            public IActionResult Get([FromQuery] int usuarioId)
            {
                try
                {
                    var enderecos = _enderecoRepo.GetByUsuario(usuarioId);
                    return Ok(enderecos);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, ex.Message);
                }
            }

            [HttpPost]
            public IActionResult Post([FromBody] AdicionarEnderecoDto dto)
            {
                try
                {
                    if (!ModelState.IsValid) return BadRequest(ModelState);

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
                        principal = dto.Principal
                    };

                    var id = _enderecoRepo.Create(endereco);
                    return CreatedAtAction(nameof(Get), new { usuarioId = dto.UsuarioId }, endereco);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, ex.Message);
                }
            }

            [HttpPut("{id}")]
            public IActionResult Put(int id, [FromBody] AdicionarEnderecoDto dto)
            {
                try
                {
                    if (!ModelState.IsValid) return BadRequest(ModelState);

                    var endereco = _enderecoRepo.GetById(id);
                    if (endereco == null) return NotFound();

                    endereco.logradouro = dto.Logradouro;
                    endereco.numero = dto.Numero;
                    endereco.complemento = dto.Complemento;
                    endereco.bairro = dto.Bairro;
                    endereco.cidade = dto.Cidade;
                    endereco.uf = dto.Uf;
                    endereco.cep = dto.Cep;
                    endereco.principal = dto.Principal;

                    var success = _enderecoRepo.Update(endereco);
                    if (!success) return NotFound();

                    return NoContent();
                }
                catch (Exception ex)
                {
                    return StatusCode(500, ex.Message);
                }
            }

            [HttpDelete("{id}")]
            public IActionResult Delete(int id)
            {
                try
                {
                    var success = _enderecoRepo.Delete(id);
                    if (!success) return NotFound();

                    return NoContent();
                }
                catch (Exception ex)
                {
                    return StatusCode(500, ex.Message);
                }
            }
        }
}

