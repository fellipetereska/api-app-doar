using Dapper;
using Dapper.Contrib.Extensions;
using Api.AppDoar.PersistenciaDB;
using MySql.Data.MySqlClient;
using Api.AppDoar.Classes.instituicao;

namespace Api.AppDoar.Repositories.instituicao
{
    public class EstoqueRepositorio
    {
        private MySqlConnection conn;

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

        public void AtualizarQuantidade(int id, int novaQuantidade)
        {
            using var conn = ConnectionDB.GetConnection();
            string sql = "UPDATE estoque SET quantidade = @quantidade WHERE id = @id";
            conn.Execute(sql, new { id, quantidade = novaQuantidade });
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
                    subcategoria_id = @subcategoria_id
                WHERE id = @id";

            conn.Execute(sql, item);
        }
    }
}