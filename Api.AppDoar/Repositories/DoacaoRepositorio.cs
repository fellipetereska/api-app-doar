using Api.AppDoar.Classes;
using Api.AppDoar.PersistenciaDB;
using Dapper;
using Dapper.Contrib.Extensions;
using MySql.Data.MySqlClient;

namespace Api.AppDoar.Repositories
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

                doacao.id = (int)doacaoId;

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
                var doacao = conn.Get<Doacao>(id);

                if (doacao != null)
                {
                    var itens = conn.Query<DoacaoItem>(
                        "SELECT * FROM doacao_item WHERE doacao_id = @DoacaoId",
                        new { DoacaoId = id });

                }

                return doacao;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao buscar doação: {ex.Message}");
            }
        }

        public IEnumerable<Doacao> GetAll()
        {
            throw new NotImplementedException();
        }

        public void Update(Doacao entidade)
        {
            throw new NotImplementedException();
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }

        public void UpdateStatus(int id)
        {
            throw new NotImplementedException();
        }

        public void CreateImagensDoacao(IEnumerable<DoacaoImagem> imagens)
        {
            try
            {
                conn.Insert(imagens);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao criar imagens da doação: {ex.Message}");
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
    }
}