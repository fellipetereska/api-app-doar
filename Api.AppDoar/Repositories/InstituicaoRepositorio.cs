using Api.AppDoar.Classes;
using Api.AppDoar.PersistenciaDB;
using Dapper;
using Dapper.Contrib.Extensions;
using MySql.Data.MySqlClient;

namespace Api.AppDoar.Repositories
{
    public class InstituicaoRepositorio : ICrud<Instituicao>
    {
        private MySqlConnection conn;

        public InstituicaoRepositorio() { conn = ConnectionDB.GetConnection(); }

        public Instituicao? BuscarPorCnpj(string cnpj)
        {
            string query = "SELECT * FROM instituicao WHERE cnpj = @Cnpj";
            return conn.QueryFirstOrDefault<Instituicao>(query, new { Cnpj = cnpj });
        }

        public long Create(Instituicao pInstituicao)
        {
            return conn.Insert<Instituicao>(pInstituicao);
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Instituicao> GetAll()
        {
            throw new NotImplementedException();
        }

        public Instituicao? GetById(int id)
        {
            try
            {
                return conn.QueryFirstOrDefault<Instituicao>("SELECT * FROM instituicao WHERE id = @Id", new { Id = id });
            }
            catch (MySqlException ex)
            {
                throw new Exception($"Erro no banco de dados. Satatus: {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao buscar instituição: {ex.Message}");
            }
        }

        public void Update(Instituicao pInstituicao)
        {
            conn.Update<Instituicao>(pInstituicao);
        }

        public void UpdateStatus(int id)
        {
            throw new NotImplementedException();
        }
    }
}