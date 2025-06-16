using Api.AppDoar.Dtos.doador;
using Api.AppDoar.Repositories.doador;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class EnderecoController : ControllerBase
{
    private readonly DoadorRepositorio _doadorRepo;

    public EnderecoController(DoadorRepositorio doadorRepo)
    {
        _doadorRepo = doadorRepo;
    }

    [HttpGet("usuario/{usuarioId}")]
    public IActionResult GetByUsuario(int usuarioId)
    {
        try
        {
            var usuario = _doadorRepo.GetUsuarioCompleto(usuarioId);
            if (usuario == null || usuario.role != "doador")
            {
                return NotFound(new
                {
                    success = false,
                    message = "Usuário não encontrado ou não é um doador"
                });
            }

            return Ok(new
            {
                success = true,
                data = new
                {
                    logradouro = usuario.logradouro,
                    numero = usuario.numero,
                    complemento = usuario.complemento,
                    endereco = usuario.endereco,
                    bairro = usuario.bairro,
                    cidade = usuario.cidade,
                    uf = usuario.uf,
                    cep = usuario.cep
                }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                success = false,
                message = $"Erro ao buscar endereço: {ex.Message}"
            });
        }
    }

    [HttpPut("usuario/{usuarioId}")]
    public IActionResult Update(int usuarioId, [FromBody] AtualizarEnderecoDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var usuario = _doadorRepo.GetUsuarioCompleto(usuarioId);
            if (usuario == null || usuario.role != "doador")
                return NotFound();

            usuario.logradouro = dto.Logradouro; 
            usuario.numero = dto.Numero;
            usuario.complemento = dto.Complemento;
            usuario.bairro = dto.Bairro;
            usuario.cidade = dto.Cidade;
            usuario.uf = dto.Uf;
            usuario.cep = dto.Cep;

            var success = _doadorRepo.UpdateUsuario(usuario);

            if (!success)
                return StatusCode(500, new { message = "Erro ao atualizar endereço" });

            return Ok(new
            {
                success = true,
                message = "Endereço atualizado com sucesso"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"Erro ao atualizar endereço: {ex.Message}" });
        }
    }
}