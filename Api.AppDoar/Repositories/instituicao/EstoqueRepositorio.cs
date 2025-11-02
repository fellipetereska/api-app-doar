using Dapper;
using Dapper.Contrib.Extensions;
using Api.AppDoar.PersistenciaDB;
using MySql.Data.MySqlClient;
using Api.AppDoar.Classes.instituicao;
using System; // Adicionado
using System.Collections.Generic;
using System.Linq;

namespace Api.AppDoar.Repositories.instituicao
{
    public class EstoqueRepositorio
    {
        public long Adicionar(Estoque estoque)
        {
            using var conn = ConnectionDB.GetConnection();
            return conn.Insert(estoque);
        }

        public IEnumerable<Estoque> ListarPorInstituicao(int instituicaoId)
        {
            using var conn = ConnectionDB.GetConnection();
            string sql = "SELECT * FROM vw_estoque WHERE instituicao_id = @instituicaoId";
            return conn.Query<Estoque>(sql, new { instituicaoId }).ToList();
        }

        public Estoque? GetByCategoria(int categoria_id, int subcategoria_id, int instituicao_id)
        {
            using var conn = ConnectionDB.GetConnection();
            string sql = "SELECT * FROM estoque WHERE categoria_id = @categoria_id AND subcategoria_id = @subcategoria_id AND instituicao_id = @instituicao_id";
            return conn.QueryFirstOrDefault<Estoque>(sql, new { categoria_id, subcategoria_id, instituicao_id });
        }

        public void AtualizarQuantidadeEData(int id, int novaQuantidade, DateTime dataEntrada)
        {
            using var conn = ConnectionDB.GetConnection();
            string sql = "UPDATE estoque SET quantidade = @quantidade, data_ultima_entrada = @dataEntrada WHERE id = @id";
            conn.Execute(sql, new { id, quantidade = novaQuantidade, dataEntrada });
        }

        public Estoque? GetById(int id)
        {
            using var conn = ConnectionDB.GetConnection();
            string sql = "SELECT * FROM estoque WHERE id = @id";
            return conn.QueryFirstOrDefault<Estoque>(sql, new { id });
        }

        public void Atualizar(Estoque item)
        {
            using var conn = ConnectionDB.GetConnection();
            string sql = @"
                UPDATE estoque SET
                    descricao = @descricao,
                    quantidade = @quantidade,
                    categoria_id = @categoria_id,
                    subcategoria_id = @subcategoria_id,
                    data_ultima_entrada = @data_ultima_entrada 
                WHERE id = @id";

            conn.Execute(sql, item);
        }
    }
}