using Api.AppDoar.Classes;
using Api.AppDoar.PersistenciaDB;
using MySql.Data.MySqlClient;
using Dapper;
using Dapper.Contrib.Extensions;

namespace Api.AppDoar.Repositories
{
    public class EnderecoRepositorio
    {
        private readonly MySqlConnection _conn;

        public EnderecoRepositorio()
        {
            _conn = ConnectionDB.GetConnection();
        }

        public IEnumerable<Endereco> GetByUsuario(int usuarioId)
        {
            return _conn.Query<Endereco>(
                "SELECT * FROM endereco WHERE usuario_id = @UsuarioId ORDER BY principal DESC",
                new { UsuarioId = usuarioId });
        }

        public Endereco GetById(int id)
        {
            return _conn.Get<Endereco>(id);
        }

        public long Create(Endereco endereco)
        {
            if (endereco.principal)
            {
                _conn.Execute(
                    "UPDATE endereco SET principal = false WHERE usuario_id = @UsuarioId",
                    new { UsuarioId = endereco.usuario_id });
            }

            return _conn.Insert(endereco);
        }

        public bool Update(Endereco endereco)
        {
            if (endereco.principal)
            {
                _conn.Execute(
                    "UPDATE endereco SET principal = false WHERE usuario_id = @UsuarioId AND id != @Id",
                    new { UsuarioId = endereco.usuario_id, Id = endereco.id });
            }

            return _conn.Update(endereco);
        }

        public bool Delete(int id)
        {
            var endereco = GetById(id);
            if (endereco == null) return false;

            return _conn.Delete(endereco);
        }
    }
}

