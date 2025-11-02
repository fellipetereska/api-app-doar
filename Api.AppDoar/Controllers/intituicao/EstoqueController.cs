using Microsoft.AspNetCore.Mvc;
using Api.AppDoar.Classes;
using Api.AppDoar.Dtos;
using Api.AppDoar.Repositories.instituicao;
using Api.AppDoar.Dtos.instituicao;
using Api.AppDoar.Classes.instituicao;
using System;
using System.Collections.Generic;

namespace Api.AppDoar.Controllers.instituicao
{
    [ApiController]
    [Route("api/[controller]")]
    public class EstoqueController : Controller
    {
        private readonly EstoqueRepositorio EstoqueRepo = new EstoqueRepositorio();
        private readonly EstoqueMovimentacaoRepositorio MovimentacaoRepo = new EstoqueMovimentacaoRepositorio();

        [HttpPost]
        public IActionResult Criar([FromBody] CadastroEstoqueDto dto)
        {
            try
            {
                int idDoItemDeEstoque = -1;
                var itemExistente = EstoqueRepo.GetByCategoria(dto.categoria_id, dto.subcategoria_id, dto.instituicao_id);

                if (itemExistente != null)
                {
                    var novaQuantidade = itemExistente.quantidade + dto.quantidade;
                    EstoqueRepo.AtualizarQuantidadeEData(itemExistente.id, novaQuantidade, dto.data_movimentacao);
                    idDoItemDeEstoque = itemExistente.id;
                }
                else
                {
                    var estoque = new Estoque
                    {
                        instituicao_id = dto.instituicao_id,
                        categoria_id = dto.categoria_id,
                        subcategoria_id = dto.subcategoria_id,
                        quantidade = dto.quantidade,
                        descricao = dto.descricao,
                        data_ultima_entrada = dto.data_movimentacao 
                    };

                    var id = EstoqueRepo.Adicionar(estoque);
                    idDoItemDeEstoque = (int)id;
                }

                var movimentacao = new EstoqueMovimentacao
                {
                    estoque_id = idDoItemDeEstoque,
                    instituicao_id = dto.instituicao_id,
                    categoria_id = dto.categoria_id,
                    subcategoria_id = dto.subcategoria_id,
                    quantidade = dto.quantidade,
                    descricao = dto.descricao,
                    data_movimentacao = dto.data_movimentacao, 
                    tipo_movimentacao = "entrada"
                };

                MovimentacaoRepo.Adicionar(movimentacao);

                return Ok(new { message = "Item salvo e movimentação registrada", id = idDoItemDeEstoque });
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

                int diferenca = 0; 
                if (dto.quantidade != itemExistente.quantidade)
                {
                    diferenca = dto.quantidade - itemExistente.quantidade;
                }

                if (diferenca > 0)
                {
                    var movimentacao = new EstoqueMovimentacao
                    {
                        estoque_id = itemExistente.id,
                        instituicao_id = itemExistente.instituicao_id,
                        categoria_id = dto.categoria_id,
                        subcategoria_id = dto.subcategoria_id,
                        quantidade = diferenca,
                        descricao = "Ajuste de estoque (Update)",
                        data_movimentacao = dto.data_movimentacao, 
                        tipo_movimentacao = "entrada"
                    };
                    MovimentacaoRepo.Adicionar(movimentacao);

                    itemExistente.data_ultima_entrada = dto.data_movimentacao;
                }

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

        [HttpGet("relatorio-entradas")]
        public IActionResult GetRelatorioEntradas([FromQuery] int instituicaoId, [FromQuery] DateTime dataInicio, [FromQuery] DateTime dataFim)
        {
            try
            {
                var dataFimAjustada = dataFim.Date.AddDays(1).AddTicks(-1);

                var dados = MovimentacaoRepo.ListarEntradasPorPeriodo(instituicaoId, dataInicio, dataFimAjustada);

                return Ok(dados);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}