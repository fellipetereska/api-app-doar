using Api.AppDoar.Classes;
using Api.AppDoar.Classes.instituicao;
using Api.AppDoar.Dtos.instituicao;
using Api.AppDoar.PersistenciaDB;
using Api.AppDoar.Repositories.assistido;
using Dapper;
using Dapper.Contrib.Extensions;
using MySql.Data.MySqlClient;
using System.Text;

namespace Api.AppDoar.Repositories.instituicao
{
    public class EntregasRepositorio : ICrud<Entregas>
    {
        private MySqlConnection conn;
        private readonly AssistidoRepositorio AssistidoRepo = new AssistidoRepositorio();
        private readonly ItensEntregaRepositorio ItensEntregaRepo = new ItensEntregaRepositorio();

        public EntregasRepositorio() { conn = ConnectionDB.GetConnection(); }

        public long Create(Entregas pEntidade)
        {
            return conn.Insert(pEntidade);
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Entregas> GetAll()
        {
            throw new NotImplementedException();
        }

        public Entregas? GetById(int id)
        {
            throw new NotImplementedException();
        }

        public Entregas? GetAllByInstituicao(int id)
        {
            throw new NotImplementedException();
        }

        public void Update(Entregas entidade)
        {
            throw new NotImplementedException();
        }

        public void UpdateStatus(int id)
        {
            string sql = "UPDATE entregas SET WHERE id = @id";
            conn.Execute(sql, new { id });
        }

        public void ToggleStatus(int id, string status)
        {
            string sql = "UPDATE entregas SET status = @status WHERE id = @id";
            conn.Execute(sql, new { id, status });
        }

        public IEnumerable<EntregaComItensDto> GetByInstituicao(int instituicaoId, string? status, string? tipo_entrega, int? assistidoId, DateTime? data)
        {
            var sql = new StringBuilder(@"SELECT * FROM vw_entregas WHERE instituicao_id = @instituicaoId");

            if (!string.IsNullOrEmpty(status))
                sql.Append(" AND status = @status");
            
            if (!string.IsNullOrEmpty(tipo_entrega))
                sql.Append(" AND tipo_entrega = @tipo_entrega");

            if (assistidoId.HasValue)
                sql.Append(" AND assistido_id = @assistidoId");

            if (data.HasValue)
                sql.Append(" AND DATE(data) = DATE(@data)");

            return conn.Query<EntregaComItensDto>(sql.ToString(), new
            {
                instituicaoId,
                status,
                tipo_entrega,
                assistidoId,
                data
            });
        }

        public void RegistrarEntregaComItensEAtualizarLista(
            Entregas entrega,
            List<ItensEntrega> itensEntrega,
            List<ItemEntregaDto> itensEntregues)
        {
            using var transaction = conn.BeginTransaction();

            try
            {
                var sqlEntrega = @"
                    INSERT INTO entregas (data, observacao, instituicao_id, assistido_id, tipo_entrega, status)
                    VALUES (@data, @observacao, @instituicao_id, @assistido_id, @tipo_entrega, @status);
                    SELECT LAST_INSERT_ID();";

                var entregaId = conn.ExecuteScalar<int>(sqlEntrega, entrega, transaction);

                foreach (var item in itensEntrega)
                {
                    item.entregas_id = entregaId;

                    // Verifica se já existe item da mesma categoria/subcategoria na entrega
                    var itemExistente = conn.QueryFirstOrDefault<int?>(@"
                        SELECT id
                        FROM entrega_itens
                        WHERE entregas_id = @EntregaId
                          AND categoria_id = @CategoriaId
                          AND subcategoria_id = @SubcategoriaId
                        LIMIT 1",
                        new
                        {
                            EntregaId = entregaId,
                            CategoriaId = item.categoria_id,
                            SubcategoriaId = item.subcategoria_id
                        },
                        transaction
                    );

                    if (itemExistente.HasValue)
                    {
                        // Já existe: atualizar quantidade
                        conn.Execute(@"
                            UPDATE entrega_itens
                            SET quantidade = quantidade + @Quantidade
                            WHERE id = @Id",
                            new
                            {
                                Quantidade = item.quantidade,
                                Id = itemExistente.Value
                            },
                            transaction
                        );
                    }
                    else
                    {
                        // Não existe: inserir novo
                        var sqlItem = @"
                            INSERT INTO entrega_itens 
                            (entregas_id, quantidade, estoque_id, categoria_id, subcategoria_id)
                            VALUES 
                            (@entregas_id, @quantidade, @estoque_id, @categoria_id, @subcategoria_id);";

                        conn.Execute(sqlItem, item, transaction);
                    }

                    var estoqueAtual = conn.ExecuteScalar<int>(
                        "SELECT quantidade FROM estoque WHERE id = @EstoqueId",
                        new { EstoqueId = item.estoque_id },
                        transaction
                    );

                    if (estoqueAtual < item.quantidade)
                    {
                        throw new Exception($"Estoque insuficiente para o item com estoque_id {item.estoque_id}. Disponível: {estoqueAtual}, Solicitado: {item.quantidade}");
                    }


                    conn.Execute(@"
                        UPDATE estoque
                        SET quantidade = GREATEST(0, quantidade - @Quantidade)
                        WHERE id = @EstoqueId",
                        new
                        {
                            Quantidade = item.quantidade,
                            EstoqueId = item.estoque_id
                        },
                        transaction
                    );
                }


                foreach (var item in itensEntregues)
                {
                    var itemLista = conn.QueryFirstOrDefault(
                        @"SELECT id_item, quantidade_solicitada, quantidade_atendida, status
                          FROM vw_lista_espera
                          WHERE id = @AssistidoId
                            AND categoria_id = @CategoriaId
                            AND subcategoria_id = @SubcategoriaId
                            AND status != 'atendida'
                          LIMIT 1",
                        new
                        {
                            AssistidoId = entrega.assistido_id,
                            CategoriaId = item.categoria_id,
                            SubcategoriaId = item.subcategoria_id
                        },
                        transaction: transaction
                    );

                    if (itemLista == null)
                    {
                        Console.WriteLine("Item Lista Vazio");
                        continue;
                    }

                    int novaQuantidade = itemLista.quantidade_atendida + item.quantidade;
                    string novoStatus = itemLista.status;

                    if (novaQuantidade >= itemLista.quantidade_solicitada)
                    {
                        novaQuantidade = itemLista.quantidade_solicitada;
                        novoStatus = "atendida";
                    }
                    else if (itemLista.status != "parcial")
                    {
                        novoStatus = "parcial";
                    }

                    conn.Execute(
                        @"UPDATE itens_espera_assistido
                          SET quantidade_atendida = @NovaQuantidade,
                              status = @NovoStatus
                          WHERE id = @Id",
                        new
                        {
                            NovaQuantidade = novaQuantidade,
                            NovoStatus = novoStatus,
                            Id = itemLista.id_item
                        },
                        transaction: transaction
                    );
                }

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public void CancelarDoacao(int entregaId)
        {
            using var transaction = conn.BeginTransaction();

            try
            {
                // Atualiza o status da doação
                conn.Execute(@"UPDATE entregas SET status = 'cancelada' WHERE id = @Id", new { Id = entregaId }, transaction);

                // Busca os itens da doação
                var itens = conn.Query(@"
                    SELECT categoria_id, subcategoria_id, quantidade, assistido_id, estoque_id
                    FROM vw_entregas
                    WHERE id = @Id",
                    new { Id = entregaId },
                    transaction
                ).ToList();

                foreach (var item in itens)
                {
                    var dadosLista = conn.QueryFirstOrDefault(@"
                        SELECT id, id_item, quantidade_solicitada, quantidade_atendida
                        FROM vw_lista_espera
                        WHERE assistido_id = @AssistidoId
                          AND categoria_id = @CategoriaId
                          AND subcategoria_id = @SubcategoriaId",
                         new
                         {
                             AssistidoId = item.assistido_id,
                             CategoriaId = item.categoria_id,
                             SubcategoriaId = item.subcategoria_id
                         },
                         transaction
                     );

                    if (dadosLista == null) continue;

                    int novaQuantidade = Math.Max(0, dadosLista.quantidade_atendida - item.quantidade);
                    string novoStatus = novaQuantidade == 0
                        ? "pendente"
                        : "parcial";

                    conn.Execute(@"
                        UPDATE itens_espera_assistido
                        SET quantidade_atendida = @NovaQuantidade,
                            status = @NovoStatus
                        WHERE id = @Id",
                        new
                        {
                            NovaQuantidade = novaQuantidade,
                            NovoStatus = novoStatus,
                            Id = dadosLista.id_item
                        },
                        transaction
                    );

                    conn.Execute(@"
                        UPDATE estoque
                        SET quantidade = quantidade + @Quantidade
                        WHERE id = @EstoqueId",
                       new
                       {
                           Quantidade = item.quantidade,
                           EstoqueId = item.estoque_id
                       },
                       transaction
                    );
                }
                transaction.Commit();
            }
            catch (Exception ex)
            {
                {
                    Console.Error.WriteLine(ex.ToString());
                    transaction.Rollback();
                    throw;
                }
            }
        }
    }
}
