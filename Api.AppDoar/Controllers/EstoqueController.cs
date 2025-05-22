
using Microsoft.AspNetCore.Mvc;
using Api.AppDoar.Repositories;
using Api.AppDoar.Classes;
using Api.AppDoar.Dtos;

namespace Api.AppDoar.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EstoqueController : Controller
    {
        private readonly EstoqueRepositorio EstoqueRepo = new EstoqueRepositorio();

        [HttpPost]
        public IActionResult Criar([FromBody] CadastroEstoqueDto dto)
        {
            try
            {

                var itemExistente = EstoqueRepo.GetByCategoria(dto.categoria_id, dto.subcategoria_id, dto.instituicao_id);
                // Se o item existir (categoria e subcategoria) ele apenas adiciona a quantidade
                if (itemExistente != null)
                {
                    var novaQuantidade = itemExistente.quantidade + dto.quantidade;
                    EstoqueRepo.AtualizarQuantidade(itemExistente.id, novaQuantidade);
                    return Ok(new { message = "Quantidade atualizada no estoque", itemExistente.id });
                }

                var estoque = new Estoque
                {
                    instituicao_id = dto.instituicao_id,
                    categoria_id = dto.categoria_id,
                    subcategoria_id = dto.subcategoria_id,
                    quantidade = dto.quantidade,
                    descricao = dto.descricao
                };

                var id = EstoqueRepo.Adicionar(estoque);
                return Ok(new { id });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public IActionResult Atualizar(int id, [FromBody] CadastroEstoqueDto dto)
        {
            try
            {
                var itemExistente = EstoqueRepo.GetById(id);
                if (itemExistente == null)
                    return NotFound(new { message = "Item não encontrado." });

                // Atualiza os campos principais
                itemExistente.descricao = dto.descricao;
                itemExistente.quantidade = dto.quantidade;
                itemExistente.categoria_id = dto.categoria_id;
                itemExistente.subcategoria_id = dto.subcategoria_id;

                EstoqueRepo.Atualizar(itemExistente);
                return Ok(new { message = "Item atualizado com sucesso." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }


        [HttpGet]
        public IActionResult Listar([FromQuery] int instituicaoId)
        {
            try
            {
                var dados = EstoqueRepo.ListarPorInstituicao(instituicaoId);
                return Ok(dados);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
