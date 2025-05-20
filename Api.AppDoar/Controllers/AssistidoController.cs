using Api.AppDoar.Classes;
using Api.AppDoar.Dtos;
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
        public IActionResult Listar([FromQuery] int instituicaId)
        {
            try
            {
                var assistidos = AssistidoRepo.GetAllByInstituicao(instituicaId);
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

        [HttpPost("{id}/itens_espera")]
        public IActionResult AdicionarItensListaEspera(int id, [FromBody] ListaEsperaPostDto dto)
        {
            try
            {
                var assistido = AssistidoRepo.GetById(id);
                if (assistido == null)
                    return NotFound(new { message = "Assistido não encontrado." });

                if (dto.itens == null || dto.itens.Count == 0)
                    return BadRequest(new { message = "Nenhum item enviado." });

                foreach (var item in dto.itens)
                {
                    var novoItem = new ListaEspera
                    {
                        assistido_id = id,
                        categoria_id = item.categoria_id,
                        subcategoria_id = item.subcategoria_id,
                        quantidade_solicitada = item.quantidade_solicitada,
                        quantidade_atendida = 0,
                        status = "pendente",
                        data_solicitacao = DateTime.Now,
                        observacao = item.observacao
                    };

                    AssistidoRepo.AdicionarItemListaEspera(novoItem);
                }

                return Ok(new { message = "Itens adicionados à lista de espera com sucesso." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPatch("{id}/itens_espera")]
        public IActionResult AtualizarItensListaEspera(int id, [FromBody] ListaEsperaPostDto dto)
        {
            try
            {
                foreach (var item in dto.itens)
                {
                    if (item.id_item > 0)
                    {
                        // Atualizar
                        var atualizado = new ListaEspera
                        {
                            id = item.id_item,
                            quantidade_atendida = item.quantidade_atendida,
                            quantidade_solicitada = item.quantidade_solicitada,
                            observacao = item.observacao,
                            status = item.status
                        };
                        AssistidoRepo.AtualizarItemListaEspera(atualizado);
                    }
                    else
                    {
                        // Inserir
                        var novo = new ListaEspera
                        {
                            assistido_id = id,
                            categoria_id = item.categoria_id,
                            subcategoria_id = item.subcategoria_id,
                            quantidade_solicitada = item.quantidade_solicitada,
                            quantidade_atendida = 0,
                            observacao = item.observacao,
                            status = "pendente",
                            data_solicitacao = DateTime.Now
                        };
                        AssistidoRepo.AdicionarItemListaEspera(novo);
                    }
                }

                return Ok(new { message = "Itens atualizados com sucesso." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }


        [HttpGet("itens_lista_espera/{id}")]
        public IActionResult BuscarItensDoAssistido(int id)
        {
            try
            {
                var itens = AssistidoRepo.GetItensListaEsperaPorAssistido(id);
                return Ok(itens);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpDelete("item_lista_espera/{id}")]
        public IActionResult RemoverItemListaEspera(int id)
        {
            try
            {
                AssistidoRepo.RemoverItemListaEspera(id);
                return Ok(new { message = "Item removido com sucesso." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

    }
}
