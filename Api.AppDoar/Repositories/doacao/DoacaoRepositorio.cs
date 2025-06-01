using Api.AppDoar.Classes;
using Api.AppDoar.Classes.doacao;
using Api.AppDoar.Classes.instituicao;
using Api.AppDoar.PersistenciaDB;
using Dapper;
using Dapper.Contrib.Extensions;
using MySql.Data.MySqlClient;

namespace Api.AppDoar.Repositories.doacao
{
    public class DoacaoRepositorio : ICrud<Doacao>
    {
        private MySqlConnection conn;

        public DoacaoRepositorio()
        {
            conn = ConnectionDB.GetConnection();
        }

        public long Create(Doacao doacao)
        {
            try
            {
                var doacaoId = conn.Insert(doacao);
                return doacaoId;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao criar doação: {ex.Message}");
            }
        }

        public void CreateItensDoacao(IEnumerable<DoacaoItem> itens)
        {
            try
            {
                conn.Insert(itens);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao criar itens da doação: {ex.Message}");
            }
        }

        public Doacao? GetById(int id)
        {
            try
            {
                return conn.Get<Doacao>(id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao buscar doação: {ex.Message}");
            }
        }

        public IEnumerable<DoacaoItem> GetItensByDoacaoId(int doacaoId)
        {
            try
            {
                return conn.Query<DoacaoItem>(
                    "SELECT * FROM doacao_item WHERE doacao_id = @DoacaoId",
                    new { DoacaoId = doacaoId });
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao buscar itens da doação: {ex.Message}");
            }
        }

        public IEnumerable<Doacao> GetByInstituicaoId(int instituicaoId, string status)
        {
            var sql = @"
        SELECT * FROM doacao 
        WHERE instituicao_id = @InstituicaoId 
        AND status = @Status
        ORDER BY created_at DESC";

            return conn.Query<Doacao>(sql, new
            {
                InstituicaoId = instituicaoId,
                Status = status
            });
        }

        public bool UpdateStatus(int id, string status)
        {
            var sql = @"
        UPDATE doacao 
        SET status = @Status, 
            updated_at = @UpdatedAt 
        WHERE id = @Id";

            var rowsAffected = conn.Execute(sql, new
            {
                Id = id,
                Status = status,
                UpdatedAt = DateTime.Now
            });

            return rowsAffected > 0;
        }

        public void CreateImagensDoacao(IEnumerable<DoacaoImagem> imagens)
        {
            try
            {
                var imagensValidas = imagens.Where(i => i.doacao_item_id > 0).ToList();

                if (!imagensValidas.Any())
                    throw new Exception("Nenhuma imagem com item válido encontrada");

                conn.Insert(imagensValidas);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao criar imagens da doação: {ex.Message}");
            }
        }

        public long CreateItemDoacao(DoacaoItem item)
        {
            try
            {
                return conn.Insert(item);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao criar item da doação: {ex.Message}");
            }
        }

        public IEnumerable<DoacaoImagem> GetImagensByDoacaoId(int doacaoId)
        {
            try
            {
                return conn.Query<DoacaoImagem>(
                    "SELECT * FROM doacao_imagem WHERE doacao_id = @DoacaoId ORDER BY ordem",
                    new { DoacaoId = doacaoId });
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao buscar imagens da doação: {ex.Message}");
            }
        }

        public Instituicao GetInstituicaoById(int id)
        {
            return conn.Get<Instituicao>(id);
        }

        public T ExecuteScalar<T>(string sql, object parameters = null)
        {
            return conn.ExecuteScalar<T>(sql, parameters);
        }

        public IEnumerable<Doacao> GetAll() => throw new NotImplementedException();
        public void Update(Doacao entidade) => throw new NotImplementedException();
        public void Delete(int id) => throw new NotImplementedException();
        public void UpdateStatus(int id) => throw new NotImplementedException();
    }
}