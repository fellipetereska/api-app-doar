using Api.AppDoar.Classes;
using Api.AppDoar.Dtos;
using Api.AppDoar.Repositories;
using Microsoft.AspNetCore.Mvc;
using BCrypt.Net;

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

            // Verificar usuário e senha
            if (objUsuario == null)
                return Unauthorized(new { message = "Usuário não encontrado!" });

            if (!BCrypt.Net.BCrypt.Verify(login.senha, objUsuario.senha))
                return Unauthorized(new { message = "Senha inválida!" });

            Object? res = null;

            // Verificar a role do usuario
            if (objUsuario.role == "instituicao")
            {
                // Buscar dados da Instituição
                var objInstituicao = InstituicaoRepo.GetById(Convert.ToInt32(objUsuario.instituicao_id));

                if (objInstituicao == null)
                    return Unauthorized("Instituição não encontrada!");

                res = objInstituicao;
            }

            var resToken = JwtHelper.GenerateToken(objUsuario);

            return Ok(new { usuario = objUsuario, instituicao = res, token = resToken });
        }

        [HttpPost("registrar/doador")]
        public IActionResult Register([FromBody] CadastroUsuarioDto register)
        {
            var userRepo = new UsuarioRepositorio();

            var usuarioExistente = userRepo.BuscarPorEmail(register.email);

            if (usuarioExistente != null)
                return Unauthorized(new { message = "Usuário já existe!" });

            var novoUsuario = new Usuario
            {
                email = register.email,
                // Criptografar a senha
                senha = BCrypt.Net.BCrypt.HashPassword(register.senha),
                nome = register.nome,
                role = "doador",
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
                return Ok(new { usuario = objUsuario });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost("registrar/instituicao")]
        public IActionResult RegistrarInstituicao([FromBody] CadastroInstituicaoDto dto)
        {
            var userRepo = new UsuarioRepositorio();
            var InstituicaoRepo = new InstituicaoRepositorio();
            var doadorRepo = new DoadorRepositorio();

            var instituicaoExiste = InstituicaoRepo.BuscarPorCnpj(dto.instituicao.cnpj);

            if (instituicaoExiste != null)
                return Conflict(new { message = "Instituição já existe!" });

            var usuarioExistente = userRepo.BuscarPorEmail(dto.usuario.email);

            if (usuarioExistente != null)
                return Conflict(new { message = "Usuário já existe!" });

            var novaInstituicao = new Instituicao
            {
                nome = dto.instituicao.nome,
                cnpj = dto.instituicao.cnpj,
                telefone = dto.instituicao.telefone,
                cep = dto.instituicao.cep,
                logradouro = dto.instituicao.logradouro,
                endereco = dto.instituicao.endereco,
                numero  = dto.instituicao.numero,
                complemento = dto.instituicao.complemento,
                bairro = dto.instituicao.bairro,
                cidade = dto.instituicao.cidade,
                uf = dto.usuario.uf,
                descricao = dto.instituicao.descricao,
                latitude = 0,
                longitude = 0,
            };

            object? instituicaoCriada;

            try
            {
                instituicaoCriada = InstituicaoRepo.Create(novaInstituicao);

                var novoUsuario = new Usuario
                {
                    email = dto.usuario.email,
                    // Criptografar a senha
                    senha = BCrypt.Net.BCrypt.HashPassword(dto.usuario.senha),
                    tipo_documento = dto.usuario.tipo_documento,
                    documento = dto.usuario.documento,
                    role = "instituicao",
                    instituicao_id = Convert.ToInt32(instituicaoCriada),
                    status = 1
                };

                var usuarioCriado = userRepo.Create(novoUsuario);
                return Ok(new { usuario = usuarioCriado, instituicao = instituicaoCriada });

            } catch (Exception ex)
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
