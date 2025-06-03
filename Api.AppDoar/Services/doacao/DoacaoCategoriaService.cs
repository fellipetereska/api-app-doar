using Api.AppDoar.Classes;
using Api.AppDoar.Dtos.doacao;
using Api.AppDoar.PersistenciaDB;
using Dapper;
using MySql.Data.MySqlClient;

namespace Api.AppDoar.Services.doacao
{
    public class DoacaoCategoriaService
    {
        private readonly MySqlConnection _conn;

        public DoacaoCategoriaService()
        {
            _conn = ConnectionDB.GetConnection();
        }

        public IEnumerable<DoacaoCategoriaDto> GetCategoriasComSubcategorias(int instituicaoId)
        {
            var query = @"
                SELECT 
                    c.id AS Id,
                    c.nome AS Nome
                FROM categoria c
                WHERE c.instituicao_id = @InstituicaoId
                ORDER BY c.nome";

            var categorias = _conn.Query<DoacaoCategoriaDto>(query, new { InstituicaoId = instituicaoId });

            foreach (var categoria in categorias)
            {
                categoria.Subcategorias = GetSubcategoriasPorCategoria(categoria.Id).ToList();
            }

            return categorias;
        }

        public IEnumerable<DoacaoSubcategoriaDto> GetSubcategoriasPorCategoria(int categoriaId)
        {
            var query = @"
                SELECT 
                    s.idsubcategoria AS Id,
                    s.nome AS Nome
                FROM subcategoria s
                WHERE s.categoria_id = @CategoriaId
                ORDER BY s.nome";

            return _conn.Query<DoacaoSubcategoriaDto>(query, new { CategoriaId = categoriaId });
        }
    }
}