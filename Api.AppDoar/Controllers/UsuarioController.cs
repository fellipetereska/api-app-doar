using Api.AppDoar.Dtos;
using Api.AppDoar.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Api.AppDoar.Controllers   
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController : Controller
    {
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto login)
        {
            var UserRepo = new UsuarioRepositorio();
            var objUsuario = UserRepo.BuscarPorEmail(login.email);

            // Verificar usuário e senha
            if (objUsuario == null || objUsuario.senha != login.senha)
                return Unauthorized("Usuário ou senha inválidos!");

            return Ok(new
            {
                objUsuario.id,
                objUsuario.email,
                objUsuario.role,
                objUsuario.status
            });
        }
    }
}
