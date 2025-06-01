using Api.AppDoar.Classes;
using Api.AppDoar.Classes.instituicao;
using Api.AppDoar.Dtos;
using Api.AppDoar.Dtos.instituicao;
using Api.AppDoar.Repositories.assistido;
using Api.AppDoar.Repositories.instituicao;
using Microsoft.AspNetCore.Mvc;
using static Api.AppDoar.Dtos.instituicao.EntregasDto;

namespace Api.AppDoar.Controllers.instituicao
{
    [ApiController]
    [Route("api/[controller]")]
    public class EntregasController : Controller
    {
        private readonly EntregasRepositorio EntregasRepo = new EntregasRepositorio();
        private readonly ItensEntregaRepositorio ItensEntregaRepo = new ItensEntregaRepositorio();
        private readonly AssistidoRepositorio AssisitoRepo = new AssistidoRepositorio();

        [HttpGet]
        public IActionResult Listar([FromQuery] int instituicaoId)
        {
            try
            {
                var assistidos = EntregasRepo.GetAllByInstituicao(instituicaoId);
                return Ok(assistidos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult Adicionar([FromBody] EntregaPostDto dto)
        {
            try
            {
                var itensEntrega = dto.itens.Select(item => new ItensEntrega
                {
                    quantidade = item.quantidade,
                    estoque_id = item.estoque_id,
                    categoria_id = item.categoria_id,
                    subcategoria_id = item.subcategoria_id
                }).ToList();

                var itensEntregues = dto.itens.Select(item => new ItemEntregaDto
                {
                    assistido_id = dto.assistido_id,
                    categoria_id = item.categoria_id,
                    subcategoria_id = item.subcategoria_id,
                    quantidade = item.quantidade
                }).ToList();

                var entrega = new Entregas
                {
                    data = dto.data,
                    observacao = dto.observacao,
                    instituicao_id = dto.instituicao_id,
                    assistido_id = dto.assistido_id,
                    tipo_entrega = dto.tipo_entrega,
                    status = dto.status
                };

                EntregasRepo.RegistrarEntregaComItensEAtualizarLista(entrega, itensEntrega, itensEntregues);

                return Ok(new { message = "Entrega registrada com sucesso!" });
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Estoque insuficiente"))
                    return BadRequest(new { message = ex.Message });

                Console.Error.WriteLine(ex.ToString());
                return StatusCode(500, new { message = "Erro ao registrar entrega." });
            }
        }

        [HttpGet("instituicao/{instituicaoId}")]
        public IActionResult GetEntregasByInstituicao(
            int instituicaoId,
            [FromQuery] string? status,
            [FromQuery] string? tipo_entrega,
            [FromQuery] int? assistidoId,
            [FromQuery] DateTime? data)
        {
            try
            {
                var resultados = EntregasRepo.GetByInstituicao(instituicaoId, status, tipo_entrega, assistidoId, data);

                var agrupado = resultados
                    .GroupBy(e => new
                    {
                        e.id,
                        e.data,
                        e.observacao,
                        e.instituicao_id,
                        e.assistido_id,
                        e.tipo_entrega,
                        e.status,
                        e.nome_assistido,
                        e.latitude_assistido,
                        e.longitude_assistido,
                        e.endereco_completo,
                        e.cep_assistido,
                    })
                    .Select(g => new EntregaAgrupadaDto
                    {
                        id = g.Key.id,
                        data = g.Key.data,
                        observacao = g.Key.observacao,
                        instituicao_id = g.Key.instituicao_id,
                        assistido_id = g.Key.assistido_id,
                        tipo_entrega = g.Key.tipo_entrega,
                        status = g.Key.status,
                        nome_assistido = g.Key.nome_assistido,
                        latitude_assistido = g.Key.latitude_assistido,
                        longitude_assistido = g.Key.latitude_assistido,
                        endereco_completo = g.Key.endereco_completo,
                        cep_assistido = g.Key.cep_assistido,
                        itens = g
                            .Where(x => x.item_id != null)
                            .Select(x => new ItemEntregaDto
                            {
                                item_id = x.item_id ?? 0,
                                quantidade = x.quantidade ?? 0,
                                estoque_id = x.estoque_id ?? 0,
                                categoria_id = x.categoria_id ?? 0,
                                subcategoria_id = x.subcategoria_id ?? 0,
                                categoria = x.categoria,
                                subcategoria = x.subcategoria
                            })
                            .ToList()
                    });

                return Ok(agrupado);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.ToString());
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // Criar o controller

        public class StatusDto
        {
            public string status { get; set; } = string.Empty;
        }

        [HttpPut("{id}/status")]
        public IActionResult AtualizarStatus(int id, [FromBody] StatusDto dto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.status))
                    return BadRequest(new { message = "Status inválido." });

                EntregasRepo.ToggleStatus(id, dto.status);
                return Ok(new { message = "Status atualizado com sucesso!" });
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.ToString());
                return StatusCode(500, new { message = ex.Message });
            }
        }


        [HttpDelete("/remover_item/{id}")]
        public IActionResult RemoverItem(int id)
        {
            try
            {
                if (id == 0)
                    return BadRequest(new { message = "Parametros inválidos" });

                ItensEntregaRepo.Delete(id);
                return Ok(new { message = "Item removido com sucesso!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost("cancelar/{id}")]
        public IActionResult Cancelar(int id)
        {
            try
            {
                EntregasRepo.CancelarDoacao(id);
                return Ok(new { message = "Doação cancelada com sucesso" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
