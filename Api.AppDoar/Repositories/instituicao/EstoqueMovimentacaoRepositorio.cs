using Dapper;
using Dapper.Contrib.Extensions;
using Api.AppDoar.PersistenciaDB;
using MySql.Data.MySqlClient;
using Api.AppDoar.Classes.instituicao;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Api.AppDoar.Repositories.instituicao
{
    public class EstoqueMovimentacaoRepositorio
    {
        public long Adicionar(EstoqueMovimentacao movimentacao)
        {
            using var conn = ConnectionDB.GetConnection();
            return conn.Insert(movimentacao);
        }

        public IEnumerable<RelatorioEntradaDto> ListarEntradasPorPeriodo(int instituicaoId, DateTime dataInicio, DateTime dataFim)
        {
            using var conn = ConnectionDB.GetConnection();

            string sql = @"
                SELECT 
                    m.id, m.data_movimentacao, m.quantidade, m.descricao,
                    c.nome AS categoria,
                    s.nome AS subcategoria
                FROM 
                    estoque_movimentacao m
                JOIN 
                    categoria c ON m.categoria_id = c.id
                JOIN 
                    subcategoria s ON m.subcategoria_id = s.id
                WHERE 
                    m.instituicao_id = @instituicaoId 
                    AND m.tipo_movimentacao = 'entrada'
                    AND m.data_movimentacao BETWEEN @dataInicio AND @dataFim
                ORDER BY
                    m.data_movimentacao DESC";

            return conn.Query<RelatorioEntradaDto>(sql, new { instituicaoId, dataInicio, dataFim }).ToList();
        }
    }
}