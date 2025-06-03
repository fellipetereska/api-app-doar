using Api.AppDoar.Classes;
using Api.AppDoar.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Api.AppDoar.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjetoController : Controller
    {
        private readonly ProjetoRepositorio ProjetoRepo = new();

        [HttpGet]
        public IActionResult BuscarProjetos([FromQuery] int id)
        {
            try
            {
                var resultado = ProjetoRepo.GetAllByInstituicao(id);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(new { erro = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult CadastrarProjeto([FromBody] Projeto projeto)
        {
            try
            {
                var idCriado = ProjetoRepo.Create(projeto);
                return Ok(new { id = idCriado });
            }
            catch (Exception ex)
            {
                return BadRequest(new { erro = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public IActionResult AtualizarProjeto(int id, [FromBody] Projeto projeto)
        {
            try
            {
                projeto.id = id;
                ProjetoRepo.Update(projeto);
                return Ok(projeto);
            }
            catch (Exception ex)
            {
                return BadRequest(new { erro = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeletarProjeto(int id)
        {
            try
            {
                ProjetoRepo.Delete(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { erro = ex.Message });
            }
        }
    }
}
