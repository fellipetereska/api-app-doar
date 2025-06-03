using Api.AppDoar.Classes;
using Api.AppDoar.PersistenciaDB;
using Dapper.Contrib.Extensions;
using Dapper;
using MySql.Data.MySqlClient;
using Api.AppDoar.Dtos;

namespace Api.AppDoar.Repositories
{
    public class ProjetoRepositorio : ICrud<Projeto>
    {
        private MySqlConnection conn;

        public ProjetoRepositorio() { conn = ConnectionDB.GetConnection(); }

        public long Create(Projeto pProjeto)
        {
            return conn.Insert(pProjeto);
        }

        public void Delete(int id)
        {
            var projeto = conn.Get<Projeto>(id);
            if (projeto is not null)
            {
                conn.Delete(projeto);
            }
            else
            {
                throw new Exception("Projeto não encontrado.");
            }
        }

        public IEnumerable<Projeto> GetAll()
        {
            throw new NotImplementedException();
        }

        public Projeto? GetById(int id)
        {
            try
            {
                string sql = "SELECT * FROM projeto WHERE id = @id";
                return conn.QueryFirstOrDefault<Projeto>(sql, new { id });
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao buscar projetos: {ex.Message}");
            }
        }

        public void Update(Projeto pProjeto)
        {
            conn.Update(pProjeto);
        }

        public void UpdateStatus(int id)
        {
            var projeto = conn.Get<Projeto>(id);
            if (projeto is not null)
            {
                projeto.status = !projeto.status;
                conn.Update(projeto);
            }
        }

        public IEnumerable<Projeto> GetAllByInstituicao(int id)
        {
            try
            {
                string sql = "SELECT * FROM projeto WHERE instituicao_id = @idInstituicao";
                return conn.Query<Projeto>(sql, new { idInstituicao = id }).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao buscar projetos: {ex.Message}");
            }
        }
    }
}
