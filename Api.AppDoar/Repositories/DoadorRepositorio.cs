using Api.AppDoar.Classes;
using Api.AppDoar.PersistenciaDB;
using Dapper;
using Dapper.Contrib.Extensions;
using MySql.Data.MySqlClient;

namespace Api.AppDoar.Repositories
{
    public class DoadorRepositorio : ICrud<Doador>
    {
        private MySqlConnection conn;

        public DoadorRepositorio() { conn = ConnectionDB.GetConnection(); }

        public long Create(Doador pDoador)
        {
            try
            {
                return conn.Insert<Doador>(pDoador);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao cadastrar doador: {ex.Message}");
            }
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Doador> GetAll()
        {
            throw new NotImplementedException();
        }

        public Doador? GetById(int id)
        {
            try
            {
                return conn.QueryFirstOrDefault<Doador>("SELECT * FROM doador WHERE id = @Id", new { Id = id });
            }
            catch (MySqlException ex)
            {
                throw new Exception($"Erro no banco de dados. Satatus: {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao buscar doador: {ex.Message}");
            }
        }

        public void Update(Doador entidade)
        {
            throw new NotImplementedException();
        }

        public void UpdateStatus(int id)
        {
            throw new NotImplementedException();
        }

        public (Usuario usuario, Doador doador) CreateWithUsuario(Usuario usuario, Doador doador)
        {
            using var transaction = conn.BeginTransaction();

            try
            {
                var usuarioId = conn.Insert(usuario, transaction);
                usuario.id = (int)usuarioId;

                doador.id = usuario.id;
                conn.Insert(doador, transaction);

                transaction.Commit();

                return (usuario, doador);
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new Exception($"Erro ao cadastrar doador e usuário: {ex.Message}");
            }
        }

    }
}
