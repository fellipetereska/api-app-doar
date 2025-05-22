using Api.AppDoar.Classes;
using Api.AppDoar.Dtos;
using Api.AppDoar.PersistenciaDB;
using Dapper;
using Dapper.Contrib.Extensions;
using MySql.Data.MySqlClient;
using System.Text;

namespace Api.AppDoar.Repositories
{
    public class EntregasRepositorio : ICrud<Entregas>
    {
        private MySqlConnection conn;

        public EntregasRepositorio() { conn = ConnectionDB.GetConnection(); }

        public long Create(Entregas pEntidade)
        {
            return conn.Insert<Entregas>(pEntidade);
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Entregas> GetAll()
        {
            throw new NotImplementedException();
        }

        public Entregas? GetById(int id)
        {
            throw new NotImplementedException();
        }

        public Entregas? GetAllByInstituicao(int id)
        {
            throw new NotImplementedException();
        }

        public void Update(Entregas entidade)
        {
            throw new NotImplementedException();
        }

        public void UpdateStatus(int id)
        {
            string sql = "UPDATE entregas SET WHERE id = @id";
            conn.Execute(sql, new { id });
        }

        public void ToggleStatus(int id, string status)
        {
            string sql = "UPDATE entregas SET status = @status WHERE id = @id";
            conn.Execute(sql, new { id, status });
        }

        public IEnumerable<EntregaComItensDto> GetByInstituicao(int instituicaoId, string? status, int? assistidoId, DateTime? data)
        {
            var sql = new StringBuilder(@"SELECT * FROM vw_entregas WHERE instituicao_id = @instituicaoId");

            if (!string.IsNullOrEmpty(status))
                sql.Append(" AND status = @status");

            if (assistidoId.HasValue)
                sql.Append(" AND assistido_id = @assistidoId");

            if (data.HasValue)
                sql.Append(" AND DATE(data) = DATE(@data)");

            return conn.Query<EntregaComItensDto>(sql.ToString(), new
            {
                instituicaoId,
                status,
                assistidoId,
                data
            });
        }

    }
}
