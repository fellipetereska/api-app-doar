using Api.AppDoar.Classes;
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
            var DoadorRepo = new DoadorRepositorio();
            var InstituicaoRepo = new InstituicaoRepositorio();

            var objUsuario = UserRepo.BuscarPorEmail(login.email);

            // Verificar usuário e senha
            if (objUsuario == null || objUsuario.senha != login.senha)
                return Unauthorized("Usuário ou senha inválidos!");

            Object res;

            // Verificar a role do usuario
            if (objUsuario.role == "doador")
            {
                // Buscar dados do Doador
                var objDodador = DoadorRepo.GetById(objUsuario.id);

                if (objDodador == null)
                    return Unauthorized("Doador não encontrado!");

                res = objDodador;
            } else
            {
                // Buscar dados da Instituição
                var objInstituicao = InstituicaoRepo.GetById(Convert.ToInt32(objUsuario.instituicao_id));

                if (objInstituicao == null)
                    return Unauthorized("Instituição não encontrada!");
                
                res = objInstituicao;
            }

            var resToken = JwtHelper.GenerateToken(objUsuario);

            return Ok(new { usuario = objUsuario, dados = res, token = resToken });
        }

        [HttpPost("registrar/doador")]
        public IActionResult Register([FromBody] CadastroDoadorDto register)
        {
            var userRepo = new UsuarioRepositorio();
            var doadorRepo = new DoadorRepositorio();

            var usuarioExistente = userRepo.BuscarPorEmail(register.email);

            if (usuarioExistente != null)
                return Unauthorized(new { message = "Usuário já existe!" });

            var novoUsuario = new Usuario
            {
                email = register.email,
                senha = register.senha,
                role = "doador",
                instituicao_id = null,
                status = 1
            };

            var novoDoador = new Doador
            {
                nome = register.nome,
                telefone = register.telefone,
                endereco = register.endereco,
                documento = register.documento
            };

            try
            {
                var (objUsuario, doador) = doadorRepo.CreateWithUsuario(novoUsuario, novoDoador);
                return Ok(new { usuario = objUsuario, dados = doador });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        private string GerarToken(Usuario usuario)
        {
            var token = JwtHelper.GenerateToken(usuario);
            return token;
        }

    }
}
