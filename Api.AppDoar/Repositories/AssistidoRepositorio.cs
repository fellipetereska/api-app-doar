using Api.AppDoar.Classes;
using Api.AppDoar.PersistenciaDB;
using Dapper;
using Dapper.Contrib.Extensions;
using MySql.Data.MySqlClient;

namespace Api.AppDoar.Repositories
{
    public class AssistidoRepositorio : ICrud<Assistido>
    {
        private MySqlConnection conn;

        public AssistidoRepositorio() { conn = ConnectionDB.GetConnection(); }

        public long Create(Assistido pAssistido)
        {
            try
            {
                var existe = conn.QueryFirstOrDefault<Assistido>(
                    "SELECT * FROM assistido WHERE documento = @cpf",
                    new { cpf = pAssistido.documento });

                if (existe != null)
                    throw new Exception("Já existe um assistido com esse CPF.");

                return conn.Insert(pAssistido);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao cadastrar assistido: {ex.Message}");
            }
        }

        public void Delete(int id)
        {
            try
            {
                var assistido = conn.Get<Assistido>(id);
                if (assistido != null)
                    conn.Delete(assistido);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao deletar assistido: {ex.Message}");
            }
        }

        public IEnumerable<Assistido> GetAll()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Assistido> GetAllByInstituicao(int id)
        {
            try
            {
                string sql = "SELECT * FROM assistido WHERE instituicao_id = @idInstituicao";
                return conn.Query<Assistido>(sql, new { idInstituicao = id });
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao buscar assistidos: {ex.Message}");
            }
        }

        public Assistido? GetById(int id)
        {
            try
            {
                return conn.Get<Assistido>(id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao buscar assistido: {ex.Message}");
            }
        }

        public void Update(Assistido pAssistido)
        {
            try
            {
                conn.Update(pAssistido);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao cadastrar assistido: {ex.Message}");
            }
        }

        public void UpdateStatus(int id)
        {
            try
            {
                var assistido = conn.Get<Assistido>(id);
                if (assistido != null)
                {
                    assistido.status_lista_espera = 1;
                    conn.Update(assistido);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao adicionar assistido a lista de espera: {ex.Message}");
            }
        }
    }
}
