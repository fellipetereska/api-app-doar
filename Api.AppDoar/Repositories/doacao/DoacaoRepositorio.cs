using Api.AppDoar.Classes;
using Api.AppDoar.Classes.doacao;
using Api.AppDoar.Classes.instituicao;
using Api.AppDoar.Dtos.doacao;
using Api.AppDoar.PersistenciaDB;
using Dapper;
using Dapper.Contrib.Extensions;
using MySql.Data.MySqlClient;
using System.Text;

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

        public IEnumerable<DoacaoComItensDto> GetByInstituicaoId(int instituicaoId, string status)
        {
            try
            {
                var sql = new StringBuilder("SELECT * FROM vw_doacao WHERE instituicao_id = @instituicaoId");
                if (!string.IsNullOrEmpty(status))
                {
                    switch (status.ToLower())
                    {
                        case "recebidas":
                            sql.Append(" AND status = 'pendente'");
                            break;
                        case "aguardando":
                            sql.Append(" AND status = 'aceita' AND status_entrega = 'pendente'");
                            break;
                        default:
                            sql.Append(" AND status = @status");
                            break;
                    }
                }

                var rows = conn.Query<DoacaoItemImagemRowDto>(sql.ToString(), new { instituicaoId });

                var agrupado = rows
                    .GroupBy(r => r.doacao_id)
                    .Select(g =>
                    {
                        var d = g.First();

                        return new DoacaoComItensDto
                        {
                            doacao_id = d.doacao_id,
                            horario_retirada = d.horario_retirada,
                            endereco = d.endereco,
                            tipo_entrega = d.tipo_entrega,
                            status = d.status,
                            status_entrega = d.status_entrega,
                            data_status = d.data_status,
                            data_entrega = d.data_entrega,
                            instituicao_id = d.instituicao_id,
                            usuario_id = d.usuario_id,
                            nome_usuario = d.nome_usuario,
                            tipo_documento = d.tipo_documento,
                            documento = d.documento,
                            telefone = d.telefone,

                            itens = g
                                .GroupBy(i => i.item_id)
                                .Select(itemGroup =>
                                {
                                    var i = itemGroup.First();

                                    return new ItemDoacao
                                    {
                                        item_id = i.item_id,
                                        item_nome = i.item_nome,
                                        item_descricao = i.item_descricao,
                                        item_estado = i.item_estado,
                                        item_quantidade = i.item_quantidade,
                                        categoria_id = i.categoria_id,
                                        subcategoria_id = i.subcategoria_id,
                                        imagens = itemGroup
                                            .Where(img => img.imagem_id != null)
                                            .Select(img => new ImagemItem
                                            {
                                                imagem_id = img.imagem_id ?? 0,
                                                url_imagem = img.url_imagem ?? "",
                                                ordem = img.ordem ?? 0
                                            }).ToList()
                                    };
                                }).ToList()
                        };
                    });

                return agrupado ?? Enumerable.Empty<DoacaoComItensDto>();
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao buscar doações com itens e imagens: {ex.Message}");
            }
        }

        public IEnumerable<DoacaoComItensDto> GetDoacoesSemInstituicao()
        {
            var sql = "SELECT * FROM vw_doacao WHERE instituicao_id IS NULL AND status = 'pendente'";

            var rows = conn.Query<DoacaoItemImagemRowDto>(sql);

            var agrupado = rows
                .GroupBy(r => r.doacao_id)
                .Select(g =>
                {
                    var d = g.First();
                    return new DoacaoComItensDto
                    {
                        doacao_id = d.doacao_id,
                        endereco = d.endereco,
                        tipo_entrega = d.tipo_entrega,
                        status = d.status,
                        status_entrega = d.status_entrega,
                        instituicao_id = d.instituicao_id,
                        usuario_id = d.usuario_id,
                        nome_usuario = d.nome_usuario,
                        tipo_documento = d.tipo_documento,
                        documento = d.documento,
                        telefone = d.telefone,
                        horario_retirada = d.horario_retirada,
                        itens = g.GroupBy(i => i.item_id).Select(gi => new ItemDoacao
                        {
                            item_id = gi.First().item_id,
                            item_nome = gi.First().item_nome,
                            item_descricao = gi.First().item_descricao,
                            item_estado = gi.First().item_estado,
                            item_quantidade = gi.First().item_quantidade,
                            categoria_id = gi.First().categoria_id,
                            subcategoria_id = gi.First().subcategoria_id,
                            imagens = gi
                                .Where(img => img.imagem_id != null)
                                .Select(img => new ImagemItem
                                {
                                    imagem_id = img.imagem_id ?? 0,
                                    url_imagem = img.url_imagem ?? "",
                                    ordem = img.ordem ?? 0
                                }).ToList()
                        }).ToList()
                    };
                });

            return agrupado ?? Enumerable.Empty<DoacaoComItensDto>();
        }
        public bool AceitarDoacao(int doacaoId, int instituicaoId)
        {
            var sql = @"
        UPDATE doacao 
        SET status = 'aceita',
            instituicao_id = @InstituicaoId
        WHERE id = @Id AND instituicao_id IS NULL";

            var rows = conn.Execute(sql, new
            {
                Id = doacaoId,
                InstituicaoId = instituicaoId
            });

            return rows > 0;
        }


        public bool UpdateStatus(int id, string status)
        {
            string sql;

            if (status == "recusada")
            {
                sql = @"
                    UPDATE doacao 
                    SET instituicao_id = NULL
                    WHERE id = @Id";
            }
            else
            {
                sql = @"
                    UPDATE doacao 
                    SET status = @Status
                    WHERE id = @Id";
            }

            var rowsAffected = conn.Execute(sql, new
            {
                Id = id,
                Status = status,
            });

            return rowsAffected > 0;
        }


        public bool UpdateStatusEntrega(int doacaoId, string status, List<ItemDoacaoEntregaDto> itens)
        {
            using var transaction = conn.BeginTransaction();

            try
            {
                var updateSql = @"
                    UPDATE doacao 
                    SET status_entrega = @Status
                    WHERE id = @Id";

                var rowsAffected = conn.Execute(updateSql, new
                {
                    Id = doacaoId,
                    Status = status
                }, transaction);

                if (status == "confirmada")
                {
                    foreach (var item in itens)
                    {
                        var estoqueExistente = conn.QueryFirstOrDefault<int?>(@"
                            SELECT id FROM estoque
                            WHERE instituicao_id = (SELECT instituicao_id FROM doacao WHERE id = @Id)
                            AND subcategoria_id = @SubcategoriaId", new
                        {
                            Id = doacaoId,
                            SubcategoriaId = item.subcategoria_id
                        }, transaction);

                        if (estoqueExistente.HasValue)
                        {
                            conn.Execute(@"
                                UPDATE estoque
                                SET quantidade = quantidade + @Quantidade
                                WHERE id = @EstoqueId", new
                            {
                                Quantidade = item.item_quantidade,
                                EstoqueId = estoqueExistente.Value
                            }, transaction);
                        }
                        else
                        {
                            conn.Execute(@"
                                INSERT INTO estoque (instituicao_id, descricao, quantidade, categoria_id, subcategoria_id)
                                VALUES ((SELECT instituicao_id FROM doacao WHERE id = @Id), @Descricao, @Quantidade, @CategoriaId, @SubcategoriaId)",
                                new
                                {
                                    Id = doacaoId,
                                    Descricao = item.descricao,
                                    Quantidade = item.item_quantidade,
                                    CategoriaId = item.categoria_id,
                                    SubcategoriaId = item.subcategoria_id
                                }, transaction);
                        }
                    }
                }

                transaction.Commit();
                return rowsAffected > 0;
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
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

        public long CreateItemDoacao(DoacaoItem item, int instituicaoId)
        {
            try
            {
                var categoriaId = conn.QueryFirstOrDefault<int?>(
                    @"SELECT s.categoria_id 
              FROM subcategoria s
              JOIN categoria c ON s.categoria_id = c.id
              WHERE s.id = @SubcategoriaId AND c.instituicao_id = @InstituicaoId",
                    new
                    {
                        SubcategoriaId = item.subcategoria_id,
                        InstituicaoId = instituicaoId
                    });

                if (!categoriaId.HasValue)
                    throw new Exception($"Subcategoria {item.subcategoria_id} não encontrada");

                item.categoria_id = categoriaId.Value;

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

        public int CountDoacoesRecebidasPorAno(int instituicaoId, int ano)
        {
            var sql = @"SELECT COUNT(*) FROM doacao 
                WHERE instituicao_id = @instituicaoId AND YEAR(data_status) = @ano";
            return conn.ExecuteScalar<int>(sql, new { instituicaoId, ano });
        }

        public int CountDoacoesRecebidasPorMes(int instituicaoId, int ano, int mes)
        {
            var sql = @"SELECT COUNT(*) FROM doacao 
                WHERE instituicao_id = @instituicaoId AND YEAR(data_status) = @ano AND MONTH(data_status) = @mes";
            return conn.ExecuteScalar<int>(sql, new { instituicaoId, ano, mes });
        }



        public IEnumerable<Doacao> GetAll() => throw new NotImplementedException();
        public void Update(Doacao entidade) => throw new NotImplementedException();
        public void Delete(int id) => throw new NotImplementedException();
        public void UpdateStatus(int id) => throw new NotImplementedException();
    }
}