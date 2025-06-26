using Api.AppDoar.Classes;
using Api.AppDoar.Classes.instituicao;
using Api.AppDoar.PersistenciaDB;
using Dapper;
using Dapper.Contrib.Extensions;
using MySql.Data.MySqlClient;

namespace Api.AppDoar.Repositories.instituicao
{
    public class ItensEntregaRepositorio : ICrud<ItensEntrega>
    {
        private MySqlConnection conn;

        public long Create(ItensEntrega item)
        {

            using var conn = ConnectionDB.GetConnection();
            string sql = @"
                INSERT INTO entrega_itens 
                (quantidade, entregas_id, estoque_id, categoria_id, subcategoria_id)
                VALUES
                (@quantidade, @entregas_id, @estoque_id, @categoria_id, @subcategoria_id);
                SELECT LAST_INSERT_ID();";

            return conn.ExecuteScalar<long>(sql, new
            {
                item.quantidade,
                item.entregas_id,
                item.estoque_id,
                item.categoria_id,
                item.subcategoria_id
            });
        }


        public void Delete(int id)
        {

            using var conn = ConnectionDB.GetConnection();
            conn.Execute("DELETE FROM entrega_itens WHERE id = @id", new { id });
        }

        public IEnumerable<ItensEntrega> GetAll()
        {
            throw new NotImplementedException();
        }

        public ItensEntrega? GetById(int id)
        {
            throw new NotImplementedException();
        }

        public void Update(ItensEntrega entidade)
        {
            throw new NotImplementedException();
        }

        public void UpdateStatus(int id)
        {
            throw new NotImplementedException();
        }
    }
}
