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

    public List<SubCategoria> GetSubcategoriasByInstituicao(int instituicaoId)
    {
        string sql = @"
        SELECT s.*
        FROM subcategoria s
        INNER JOIN categoria c ON s.categoria_id = c.id
        WHERE c.instituicao_id = @instituicaoId";

        return conn.Query<SubCategoria>(sql, new { instituicaoId }).ToList();
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

    public void AtualizarNomeCategoria(int categoriaId, string novoNome)
    {
        string update = "UPDATE categoria SET nome = @nome WHERE id = @id";
        conn.Execute(update, new { nome = novoNome, id = categoriaId });
    }
}
