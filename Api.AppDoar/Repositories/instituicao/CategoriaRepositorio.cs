using Api.AppDoar.Classes.instituicao;
using Api.AppDoar.Dtos.instituicao;
using Api.AppDoar.PersistenciaDB;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

public class CategoriaRepositorio
{
    private MySqlConnection conn;

    public CategoriaRepositorio()
    {
        conn = ConnectionDB.GetConnection();
    }

    public List<Categoria> GetAllByInstituicao(int instituicaoId)
    {
        string sql = "SELECT * FROM categoria WHERE instituicao_id = @instituicaoId";
        return conn.Query<Categoria>(sql, new { instituicaoId }).ToList();
    }

    public List<Subcategoria> GetSubcategoriasByCategoria(int categoriaId)
    {
        string sql = @"
        SELECT s.*
        FROM subcategoria s
        INNER JOIN categoria c ON s.categoria_id = c.id";

        return conn.Query<Subcategoria>(sql, new { categoriaId }).ToList();
    }

    public List<object> GetCategoriasComSubcategorias(int instituicaoId)
    {
        string sql = @"SELECT * FROM vw_categoria WHERE InstituicaoId = @instituicaoId";

        var dados = conn.Query<CategoriaComSubCategoriaDto>(sql, new { instituicaoId });

        var agrupado = dados
            .GroupBy(c => new { c.CategoriaId, c.CategoriaNome })
            .Select(g => new
            {
                id = g.Key.CategoriaId,
                nome = g.Key.CategoriaNome,
                subcategorias = g
                    .Where(x => x.SubcategoriaId.HasValue)
                    .Select(x => new { id = x.SubcategoriaId, nome = x.SubcategoriaNome })
                    .ToList()
            }).ToList<object>();

        return agrupado;
    }

    public int CriarCategoriaComSubcategorias(CategoriaDto dto)
    {
        using var transaction = conn.BeginTransaction();

        try
        {
            string insertCategoria = @"
                INSERT INTO categoria (nome, instituicao_id)
                VALUES (@nome, @instituicaoId);
                SELECT LAST_INSERT_ID();";

            var categoriaId = conn.ExecuteScalar<int>(insertCategoria, new
            {
                Nome = dto.nome,
                InstituicaoId = dto.instituicaoId
            }, transaction);

            string insertSubcategoria = @"
                INSERT INTO subcategoria (nome, categoria_id)
                VALUES (@nome, @categoriaId);";

            foreach (var subNome in dto.subcategorias)
            {
                conn.Execute(insertSubcategoria, new
                {
                    nome = subNome,
                    CategoriaId = categoriaId
                }, transaction);
            }

            transaction.Commit();
            return categoriaId;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public void AdicionarSubcategorias(int categoriaId, List<string> novasSubcategorias)
    {
        string insert = "INSERT INTO subcategoria (nome, categoria_id) VALUES (@nome, @categoriaId)";
        foreach (var nome in novasSubcategorias)
        {
            conn.Execute(insert, new { nome, categoriaId });
        }
    }

    public void AtualizarCategoriaComSubcategorias(int categoriaId, CategoriaDto dto)
    {
        using var transaction = conn.BeginTransaction();

        try
        {
            // Atualiza nome da categoria
            string updateCategoria = "UPDATE categoria SET nome = @nome WHERE id = @id";
            conn.Execute(updateCategoria, new { Nome = dto.nome, id = categoriaId }, transaction);

            // Busca subcategorias existentes dessa categoria
            string selectSubcategorias = "SELECT id, nome FROM subcategoria WHERE categoria_id = @categoriaId";
            var subcategoriasExistentes = conn.Query<(int id, string nome)>(selectSubcategorias, new { categoriaId }, transaction).ToList();

            var nomesNovos = dto.subcategorias.Select(n => n.Trim()).Distinct(StringComparer.OrdinalIgnoreCase).ToList();

            // Atualizar ou inserir subcategorias
            foreach (var nome in nomesNovos)
            {
                var existente = subcategoriasExistentes.FirstOrDefault(s => s.nome.Equals(nome, StringComparison.OrdinalIgnoreCase));
                if (existente.id > 0)
                {
                    // Atualiza nome (opcional, caso precise renomear - aqui seria redundante)
                    conn.Execute("UPDATE subcategoria SET nome = @nome WHERE id = @id",
                        new { nome, id = existente.id }, transaction);
                }
                else
                {
                    // Insere nova subcategoria
                    conn.Execute("INSERT INTO subcategoria (nome, categoria_id) VALUES (@nome, @categoriaId)",
                        new { nome, categoriaId }, transaction);
                }
            }

            // (Opcional) Excluir subcategorias que não estão mais na lista nova e não estão vinculadas a doações
            string excluirSubcategoriasOrfas = @"
            DELETE FROM subcategoria 
            WHERE categoria_id = @categoriaId
              AND nome NOT IN @nomes
              AND id NOT IN (SELECT DISTINCT subcategoria_id FROM doacao_item)";
            conn.Execute(excluirSubcategoriasOrfas, new { categoriaId, nomes = nomesNovos }, transaction);

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }
}
