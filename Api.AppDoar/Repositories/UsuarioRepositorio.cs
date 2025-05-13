using Api.AppDoar.Classes;
using Api.AppDoar.PersistenciaDB;
using Dapper;
using Dapper.Contrib.Extensions;
using MySql.Data.MySqlClient;

namespace Api.AppDoar.Repositories
{
    public class UsuarioRepositorio : ICrud<Usuario>
    {
        private MySqlConnection conn;

        public UsuarioRepositorio() { conn = ConnectionDB.GetConnection(); }

        public Usuario? BuscarPorEmail(string email)
        {
            string query = "SELECT * FROM usuario WHERE email = @Email";
            return conn.QueryFirstOrDefault<Usuario>(query, new { Email = email });
        }

        public long Create(Usuario pUsuario)
        {
            return conn.Insert<Usuario>(pUsuario);
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Usuario> GetAll()
        {
            throw new NotImplementedException();
        }

        public Usuario? GetById(int id)
        {
            throw new NotImplementedException();
        }

        public void Update(Usuario entidade)
        {
            throw new NotImplementedException();
        }

        public void UpdateStatus(int id)
        {
            throw new NotImplementedException();
        }
    }
}
