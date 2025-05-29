using Api.AppDoar.Classes;
using Api.AppDoar.Dtos;
using Api.AppDoar.Repositories;
using Microsoft.AspNetCore.Http;
using System.Transactions;

namespace Api.AppDoar.Services
{
    public class DoacaoService
    {
        private readonly DoacaoRepositorio _doacaoRepo;
        private readonly EnderecoRepositorio _enderecoRepo;
        private readonly DoacaoCategoriaService _categoriaService;

        public DoacaoService(
            DoacaoRepositorio doacaoRepo,
            EnderecoRepositorio enderecoRepo,
            DoacaoCategoriaService categoriaService)
        {
            _doacaoRepo = doacaoRepo;
            _enderecoRepo = enderecoRepo;
            _categoriaService = categoriaService;
        }

        public int CriarDoacaoCompleta(CriarDoacaoDto dto, List<string> imagensUrls)
        {
            using var scope = new TransactionScope();

            try
            {
                var instituicao = _doacaoRepo.GetInstituicaoById(dto.InstituicaoId);
                if (instituicao == null)
                    throw new Exception("Instituição não encontrada");

                foreach (var item in dto.Itens)
                {
                    if (!ValidarSubcategoria(dto.InstituicaoId, item.SubcategoriaId))
                        throw new Exception($"Subcategoria inválida para o item {item.Nome}");
                }

                string enderecoCompleto = ProcessarEnderecoDoacao(dto);

                var doacao = new Doacao
                {
                    doador_id = dto.DoadorId,
                    instituicao_id = dto.InstituicaoId,
                    horario_retirada = dto.HorarioRetirada,
                    endereco = enderecoCompleto,
                    tipo_entrega = dto.TipoEntrega,
                    usuario_id = dto.DoadorId,
                    status = "pendente",
                    created_at = DateTime.Now
                };

                var doacaoId = (int)_doacaoRepo.Create(doacao);

                var itensDoacao = dto.Itens.Select(item => new DoacaoItem
                {
                    doacao_id = doacaoId,
                    nome = item.Nome,
                    descricao = item.Descricao ?? string.Empty,
                    estado = item.Estado,
                    quantidade = item.Quantidade,
                    subcategoria_idsubcategoria = item.SubcategoriaId
                }).ToList();

                _doacaoRepo.CreateItensDoacao(itensDoacao);

                if (imagensUrls != null && imagensUrls.Any())
                {
                    var imagensDoacao = imagensUrls.Select((url, index) => new DoacaoImagem
                    {
                        doacao_id = doacaoId,
                        url_imagem = url,
                        ordem = index
                    }).ToList();

                    _doacaoRepo.CreateImagensDoacao(imagensDoacao);
                }

                scope.Complete();
                return doacaoId;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao criar doação: {ex.Message}");
            }
        }

        private string ProcessarEnderecoDoacao(CriarDoacaoDto dto)
        {
            if (dto.TipoEntrega == "retirada")
            {
                if (dto.EnderecoId.HasValue)
                {
                    var endereco = _enderecoRepo.GetById(dto.EnderecoId.Value);
                    if (endereco == null || endereco.usuario_id != dto.DoadorId)
                        throw new Exception("Endereço inválido");

                    return FormatarEndereco(endereco);
                }

                if (dto.NovoEndereco != null)
                {
                    if (string.IsNullOrWhiteSpace(dto.NovoEndereco.Logradouro) ||
                        string.IsNullOrWhiteSpace(dto.NovoEndereco.Numero) ||
                        string.IsNullOrWhiteSpace(dto.NovoEndereco.Bairro) ||
                        string.IsNullOrWhiteSpace(dto.NovoEndereco.Cidade) ||
                        string.IsNullOrWhiteSpace(dto.NovoEndereco.Uf) ||
                        string.IsNullOrWhiteSpace(dto.NovoEndereco.Cep))
                    {
                        throw new Exception("Todos os campos obrigatórios do endereço devem ser preenchidos");
                    }

                    var novoEndereco = new Endereco
                    {
                        usuario_id = dto.DoadorId,
                        logradouro = dto.NovoEndereco.Logradouro,
                        numero = dto.NovoEndereco.Numero,
                        complemento = dto.NovoEndereco.Complemento,
                        bairro = dto.NovoEndereco.Bairro,
                        cidade = dto.NovoEndereco.Cidade,
                        uf = dto.NovoEndereco.Uf,
                        cep = dto.NovoEndereco.Cep,
                        principal = dto.NovoEndereco.Principal
                    };

                    _enderecoRepo.Create(novoEndereco);
                    return FormatarEndereco(novoEndereco);
                }

                throw new Exception("Endereço é obrigatório para retirada");
            }
            else
            {
                var instituicao = _doacaoRepo.GetInstituicaoById(dto.InstituicaoId);
                return FormatarEndereco(instituicao);
            }
        }

        public Doacao ObterDoacaoCompleta(int doacaoId)
        {
            var doacao = _doacaoRepo.GetById(doacaoId);
            if (doacao == null)
                throw new Exception("Doação não encontrada");

            doacao.Itens = _doacaoRepo.GetItensByDoacaoId(doacaoId).ToList();
            doacao.Imagens = _doacaoRepo.GetImagensByDoacaoId(doacaoId).ToList();

            return doacao;
        }

        public IEnumerable<DoacaoCategoriaDto> GetCategoriasComSubcategorias(int instituicaoId)
        {
            try
            {
                return _categoriaService.GetCategoriasComSubcategorias(instituicaoId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao buscar categorias: {ex.Message}");
            }
        }

        public bool ValidarSubcategoria(int instituicaoId, int subcategoriaId)
        {
            try
            {
                var query = @"
                    SELECT COUNT(1) 
                    FROM subcategoria s
                    JOIN categoria c ON s.categoria_id = c.id
                    WHERE s.idsubcategoria = @SubcategoriaId
                    AND c.instituicao_id = @InstituicaoId";

                var count = _doacaoRepo.ExecuteScalar<int>(query,
                    new { SubcategoriaId = subcategoriaId, InstituicaoId = instituicaoId });

                return count > 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao validar subcategoria: {ex.Message}");
            }
        }

        private string FormatarEndereco(Endereco endereco)
        {
            return $"{endereco.logradouro}, {endereco.numero}, {endereco.bairro}, {endereco.cidade}-{endereco.uf}";
        }

        private string FormatarEndereco(Instituicao instituicao)
        {
            return $"{instituicao.logradouro}, {instituicao.numero}, {instituicao.bairro}, {instituicao.cidade}-{instituicao.uf}";
        }
    }
}