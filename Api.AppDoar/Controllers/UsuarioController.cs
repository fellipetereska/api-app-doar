using Api.AppDoar.Classes;
using Api.AppDoar.Dtos;
using Api.AppDoar.Dtos.doador;
using Api.AppDoar.Repositories;
using Api.AppDoar.Repositories.instituicao;
using BCrypt.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Win32;
using System.Text.Json;

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
            var InstituicaoRepo = new InstituicaoRepositorio();

            var objUsuario = UserRepo.BuscarPorEmail(login.email);

            if (objUsuario == null)
                return Unauthorized(new { message = "Usuário não encontrado!" });

            if (!BCrypt.Net.BCrypt.Verify(login.senha, objUsuario.senha))
                return Unauthorized(new { message = "Senha inválida!" });

            Object? res = null;

            if (objUsuario.role == "instituicao")
            {
                var objInstituicao = InstituicaoRepo.GetById(Convert.ToInt32(objUsuario.instituicao_id));

                if (objInstituicao == null)
                    return Unauthorized("Instituição não encontrada!");

                res = objInstituicao;
            }

            var resToken = JwtHelper.GenerateToken(objUsuario);

            return Ok(new { usuario = objUsuario, instituicao = res, token = resToken });
        }

        [HttpPost("registrar")]
        public IActionResult Register([FromBody] CadastroUsuarioDto register)
        {
            var userRepo = new UsuarioRepositorio();

            var usuarioExistente = userRepo.BuscarPorEmail(register.email);

            if (usuarioExistente != null)
                return Unauthorized(new { message = "Usuário já existe!" });

            var novoUsuario = new Usuario
            {
                email = register.email,
                senha = BCrypt.Net.BCrypt.HashPassword(register.senha),
                nome = register.nome,
                role = register.role ?? "doador", 
                telefone = register.telefone,
                cep = register.cep,
                logradouro = register.logradouro,
                endereco = register.endereco,
                numero = register.numero,
                complemento = register.complemento,
                bairro = register.bairro,
                cidade = register.cidade,
                uf = register.uf,
                tipo_documento = register.tipo_documento,
                documento = register.documento,
                instituicao_id = null, 
                status = 1 
            };


            try
            {
                var objUsuario = userRepo.Create(novoUsuario);
                Console.WriteLine($"Dados recebidos: {JsonSerializer.Serialize(register)}");

                return Ok(new { usuario = objUsuario });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public IActionResult Editar(int id, [FromBody] Usuario usuario)
        {
            var userRepo = new UsuarioRepositorio();

            var usuarioExistente = userRepo.GetById(id);

            try
            {
                if (usuarioExistente == null)
                    return NotFound(new { message = "Usuário não encontrado." });

                usuario.id = id;
                userRepo.Update(usuario);
                return Ok(new { message = "Usuário atualizado com sucesso." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("doador/{doadorId}")]
        public IActionResult EditarDoador(int doadorId, [FromBody] UsuarioUpdateDto usuarioUpdate)
        {
            var userRepo = new UsuarioRepositorio();
            var usuarioExistente = userRepo.GetById(doadorId);

            try
            {
                if (usuarioExistente == null)
                    return NotFound(new { message = "Usuário não encontrado." });

                var usuarioParaAtualizar = new Usuario
                {
                    id = doadorId,
                    nome = usuarioUpdate.nome,
                    email = usuarioUpdate.email,
                    telefone = usuarioUpdate.telefone,
                    senha = usuarioExistente.senha 
                };

                if (!string.IsNullOrEmpty(usuarioUpdate.senhaAtual) && !string.IsNullOrEmpty(usuarioUpdate.novaSenha))
                {
                    if (!BCrypt.Net.BCrypt.Verify(usuarioUpdate.senhaAtual, usuarioExistente.senha))
                        return BadRequest(new { message = "Senha atual incorreta." });

                    usuarioParaAtualizar.senha = BCrypt.Net.BCrypt.HashPassword(usuarioUpdate.novaSenha);
                }

                userRepo.UpdateDoador(usuarioParaAtualizar);
                return Ok(new { message = "Usuário atualizado com sucesso." });
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

        [HttpGet("instituicao")]
        public IActionResult ListarUsuariosByInstituicao([FromQuery] int instituicaoId)
        {
            var userRepo = new UsuarioRepositorio();

            try
            {
                var usuarios = userRepo.GetAllByInstituicao(instituicaoId);
                return Ok(usuarios);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetUsuarioById(int id)
        {
            try
            {
                var usuarioRepo = new UsuarioRepositorio();
                var usuario = usuarioRepo.GetByIdComEndereco(id); 

                if (usuario == null)
                    return NotFound(new { message = "Usuário não encontrado" });

                return Ok(new
                {
                    usuario.id,
                    usuario.nome,
                    usuario.email,
                    usuario.telefone,
                    usuario.role,
                    endereco = new
                    {
                        usuario.logradouro,
                        usuario.numero,
                        usuario.complemento,
                        usuario.bairro,
                        usuario.cidade,
                        usuario.uf,
                        usuario.cep
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Erro ao obter usuário: {ex.Message}" });
            }
        }

    }
}
