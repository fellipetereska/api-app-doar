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
            var resultado = _repo.GetCategoriasComSubcategorias(id);
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
            _repo.AtualizarCategoriaComSubcategorias(id, dto);
            return Ok(new { message = "Categoria atualizada com sucesso!" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

}
