using Api.AppDoar.Classes;
using Api.AppDoar.Classes.assistido;
using Api.AppDoar.Classes.instituicao;
using Api.AppDoar.Dtos.instituicao;
using Api.AppDoar.PersistenciaDB;
using Dapper;
using Dapper.Contrib.Extensions;
using MySql.Data.MySqlClient;
using System.Data;

namespace Api.AppDoar.Repositories.assistido
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
                return conn.Query<Assistido>(sql, new { idInstituicao = id }).ToList();
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

        public IEnumerable<ItemListaEsperaDto> GetItensListaEsperaPorAssistido(int assistidoId)
        {
            try
            {
                string sql = @"SELECT * FROM vw_lista_espera WHERE id = @assistidoId";
                return conn.Query<ItemListaEsperaDto>(sql, new { assistidoId }).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao buscar itens da lista de espera do assistido: {ex.Message}");
            }
        }


        public void AdicionarItemListaEspera(ListaEspera item)
        {
            try
            {
                string sql = @"
                    INSERT INTO itens_espera_assistido 
                    (status, data_solicitacao, quantidade_solicitada, quantidade_atendida, observacao, subcategoria_id, categoria_id, assistido_id)
                    VALUES
                    (@status, @data_solicitacao, @quantidade_solicitada, @quantidade_atendida, @observacao, @subcategoria_id, @categoria_id, @assistido_id)";

                conn.Execute(sql, item);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao adicionar item à lista de espera: {ex.Message}");
            }
        }

        public void AtualizarItemListaEspera(ListaEspera item)
        {
            try
            {
                string sql = @"
                    UPDATE itens_espera_assistido SET
                        status = @status,
                        quantidade_atendida = @quantidade_atendida,
                        quantidade_solicitada = @quantidade_solicitada,
                        observacao = @observacao
                    WHERE id = @id";

                conn.Execute(sql, item);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao atualizar item da lista de espera: {ex.Message}");
            }
        }

        public void RemoverItemListaEspera(int id)
        {
            try
            {
                string sql = "DELETE FROM itens_espera_assistido WHERE id = @id";
                conn.Execute(sql, new { id });
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao remover item da lista de espera: {ex.Message}");
            }
        }
    }
}