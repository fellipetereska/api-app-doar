using Api.AppDoar.Classes.doador;
using Api.AppDoar.PersistenciaDB;
using Dapper;
using Dapper.Contrib.Extensions;
using MySql.Data.MySqlClient;

namespace Api.AppDoar.Repositories.doador
{
    public class EnderecoRepositorio
    {
        private readonly MySqlConnection _conn;

        public EnderecoRepositorio()
        {
            _conn = ConnectionDB.GetConnection();
        }

        public Endereco? GetById(int id)
        {
            try
            {
                return _conn.Get<Endereco>(id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao buscar endereço por ID: {ex.Message}");
            }
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
            try
            {
                if (endereco.principal)
                {
                    _conn.Execute(
                        "UPDATE endereco SET principal = false WHERE usuario_id = @UsuarioId AND id != @Id",
                        new { UsuarioId = endereco.usuario_id, Id = endereco.id });
                }

          
                return _conn.Update(endereco);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao atualizar endereço: {ex.Message}");
            }
        }

        public IEnumerable<Endereco> GetByUsuario(int usuarioId)
        {
            return _conn.Query<Endereco>(
                "SELECT * FROM endereco WHERE usuario_id = @UsuarioId ORDER BY principal DESC",
                new { UsuarioId = usuarioId });
        }

        public Endereco? GetPrincipalByUsuario(int usuarioId)
        {
            return _conn.QueryFirstOrDefault<Endereco>(
                "SELECT * FROM endereco WHERE usuario_id = @UsuarioId AND principal = true LIMIT 1",
                new { UsuarioId = usuarioId });
        }
    }
}