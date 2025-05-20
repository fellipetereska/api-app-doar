using Api.AppDoar.Classes;
using Api.AppDoar.Dtos;
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
            // Atualiza nome
            string updateCategoria = "UPDATE categoria SET nome = @nome WHERE id = @id";
            conn.Execute(updateCategoria, new { Nome = dto.nome, id = categoriaId }, transaction);

            // Apaga todas as subcategorias antigas
            string deleteSubcategorias = "DELETE FROM subcategoria WHERE categoria_id = @categoriaId";
            conn.Execute(deleteSubcategorias, new { categoriaId }, transaction);

            // Insere as novas
            string insertSub = "INSERT INTO subcategoria (nome, categoria_id) VALUES (@nome, @categoriaId)";
            foreach (var sub in dto.subcategorias)
            {
                conn.Execute(insertSub, new { nome = sub, categoriaId }, transaction);
            }

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }


}
