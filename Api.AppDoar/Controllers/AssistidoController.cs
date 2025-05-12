using Api.AppDoar.Classes;
using Api.AppDoar.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Api.AppDoar.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AssistidoController : Controller
    {
        private readonly AssistidoRepositorio AssistidoRepo = new AssistidoRepositorio();

        [HttpPost]
        public IActionResult Criar([FromBody] Assistido assistido)
        {
            var AssistidoRepos = new AssistidoRepositorio();
            try
            {
                var objAssistido = AssistidoRepos.Create(assistido);
                return Ok(new { assistido = objAssistido });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult Listar()
        {
            try
            {
                var assistidos = AssistidoRepo.GetAll();
                return Ok(assistidos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public IActionResult BuscarPorId(int id)
        {
            try
            {
                var assistido = AssistidoRepo.GetById(id);
                if (assistido == null)
                    return NotFound(new { message = "Assistido não encontrado." });

                return Ok(assistido);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public IActionResult Editar(int id, [FromBody] Assistido assistido)
        {
            try
            {
                var existente = AssistidoRepo.GetById(id);
                if (existente == null)
                    return NotFound(new { message = "Assistido não encontrado." });

                assistido.id = id;
                AssistidoRepo.Update(assistido);
                return Ok(new { message = "Assistido atualizado com sucesso." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Excluir(int id)
        {
            try
            {
                var existente = AssistidoRepo.GetById(id);
                if (existente == null)
                    return NotFound(new { message = "Assistido não encontrado." });

                AssistidoRepo.Delete(id);
                return Ok(new { message = "Assistido excluído com sucesso." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPatch("{id}/status")]
        public IActionResult AlterarStatus(int id)
        {
            try
            {
                var existente = AssistidoRepo.GetById(id);
                if (existente == null)
                    return NotFound(new { message = "Assistido não encontrado." });

                AssistidoRepo.UpdateStatus(id);
                return Ok(new { message = "Status atualizado com sucesso." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
