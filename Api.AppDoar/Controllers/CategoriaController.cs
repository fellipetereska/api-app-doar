using Api.AppDoar.Dtos;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class CategoriaController : ControllerBase
{
    private readonly CategoriaRepositorio _repo = new();

    [HttpGet]
    public IActionResult BuscarCategorias([FromQuery] int id)
    {
        try
        {
            var categorias = _repo.GetAllByInstituicao(id);
            var subcategorias = _repo.GetSubcategoriasByInstituicao(id);

            var resultado = categorias.Select(cat => new
            {
                Nome = cat.nome,
                subcategorias = subcategorias
                    .Where(s => s.categoriaId == cat.id)
                    .Select(s => s.nome)
                    .ToList()
            }).ToList();

            return Ok(resultado);
        }
        catch (Exception ex)
        {
            return BadRequest(new { erro = ex.Message });
        }
    }


    [HttpPost]
    public IActionResult CriarComSubcategorias([FromBody] CategoriaDto dto)
    {
        try
        {
            var id = _repo.CriarCategoriaComSubcategorias(dto);
            return Ok(new { message = "Categoria criada com sucesso!", categoriaId = id });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public IActionResult AtualizarCategoria(int id, [FromBody] CategoriaDto dto)
    {
        try
        {
            _repo.AtualizarNomeCategoria(id, dto.nome);
            _repo.AdicionarSubcategorias(id, dto.subcategorias);

            return Ok(new { message = "Categoria atualizada com sucesso!" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
