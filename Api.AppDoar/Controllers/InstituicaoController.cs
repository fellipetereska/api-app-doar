using Api.AppDoar.Classes;
using Api.AppDoar.Dtos;
using Api.AppDoar.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Api.AppDoar.Controllers
{
    [ApiController]
    [Route("/instituicao")]
    public class InstituicaoController : Controller
    {
        [HttpPost("registrar")]
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
                numero = dto.instituicao.numero,
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

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
