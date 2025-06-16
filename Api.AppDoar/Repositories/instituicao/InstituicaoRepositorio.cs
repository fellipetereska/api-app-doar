using Api.AppDoar.Classes;
using Api.AppDoar.Classes.instituicao;
using Api.AppDoar.PersistenciaDB;
using Dapper;
using Dapper.Contrib.Extensions;
using MySql.Data.MySqlClient;

namespace Api.AppDoar.Repositories.instituicao
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
            return conn.Insert(pInstituicao);
        }


        public void Delete(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Instituicao> GetAll()
        {
            try
            {
                string query = "SELECT * FROM instituicao";
                return conn.Query<Instituicao>(query);
            }
            catch (MySqlException ex)
            {
                throw new Exception($"Erro no banco ao buscar instituições: {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro inesperado: {ex.Message}");
            }
        }

        public Instituicao? GetById(int id)
        {
            return conn.QueryFirstOrDefault<Instituicao>(
                "SELECT * FROM instituicao WHERE id = @Id",
                new { Id = id });
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