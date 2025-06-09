using Api.AppDoar.Classes.doador;
using Api.AppDoar.Classes.instituicao;
using Api.AppDoar.Dtos.doacao;
using Api.AppDoar.Enum;
using Api.AppDoar.Repositories;
using Api.AppDoar.Repositories.doacao;
using Api.AppDoar.Repositories.doador;
using Api.AppDoar.Repositories.instituicao;
using System.Transactions;

namespace Api.AppDoar.Services.doacao
{
    public class DoacaoService
    {
        private readonly DoacaoRepositorio _doacaoRepo;
        private readonly EnderecoRepositorio _enderecoRepo;
        private readonly UsuarioRepositorio _usuarioRepo;
        private readonly InstituicaoRepositorio _instituicaoRepo;
        private readonly DoacaoCategoriaService _categoriaService;
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _config;

        public DoacaoService(
            DoacaoRepositorio doacaoRepo,
            EnderecoRepositorio enderecoRepo,
            UsuarioRepositorio usuarioRepo,
            InstituicaoRepositorio instituicaoRepositorio,
            DoacaoCategoriaService categoriaService,
            IWebHostEnvironment env,
            IConfiguration config)
        {
            _doacaoRepo = doacaoRepo;
            _enderecoRepo = enderecoRepo;
            _usuarioRepo = usuarioRepo;
            _instituicaoRepo = instituicaoRepositorio;
            _categoriaService = categoriaService;
            _env = env;
            _config = config;
        }

        public async Task<int> CriarDoacaoCompleta(
        CriarDoacaoDto dto,
        List<(ItemDoacaoDto item, List<IFormFile> imagens)> itensComArquivos,
        int instituicaoId,
        string baseUrl)
        {
            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            try
            {
                var instituicao = _doacaoRepo.GetInstituicaoById(dto.InstituicaoId);
                if (instituicao == null)
                    throw new Exception("Instituição não encontrada");

                if (dto.Itens == null || !dto.Itens.Any())
                    throw new Exception("Pelo menos um item é obrigatório");

                string enderecoCompleto = ProcessarEnderecoDoacao(dto);

                var doacao = new Doacao
                {
                    usuario_id = dto.DoadorId,
                    instituicao_id = dto.InstituicaoId,
                    horario_retirada = dto.HorarioRetirada,
                    endereco = enderecoCompleto,
                    tipo_entrega = dto.TipoEntrega,
                    status = StatusDoacao.pendente.ToString(),
                    status_entrega = StatusEntrega.pendente.ToString(),
                    data_status = DateTime.Now,
                    data_entrega = DateTime.Now
                };

                var doacaoId = (int)_doacaoRepo.Create(doacao);

                foreach (var (item, imagens) in itensComArquivos)
                {
                    if (!ValidarSubcategoria(dto.InstituicaoId, item.SubcategoriaId))
                        throw new Exception($"Subcategoria inválida para o item {item.Nome}");

                    var doacaoItem = new DoacaoItem
                    {
                        doacao_id = doacaoId,
                        nome = item.Nome,
                        descricao = item.Descricao ?? string.Empty,
                        estado = item.Estado,
                        quantidade = item.Quantidade,
                        subcategoria_id = item.SubcategoriaId 
                    };

                    var itemId = (int)_doacaoRepo.CreateItemDoacao(doacaoItem);

                    if (imagens != null && imagens.Any())
                    {
                        var urlsImagens = await ProcessarUploadImagensItem(
                         imagens,
                         doacaoId,
                         itemId,
                         instituicaoId,
                         baseUrl);

                        var imagensDoacao = urlsImagens.Select((url, index) => new DoacaoImagem
                        {
                            doacao_item_id = itemId,
                            url_imagem = url,
                            ordem = index
                        }).ToList();

                        _doacaoRepo.CreateImagensDoacao(imagensDoacao);
                    }
                }

                scope.Complete();
                return doacaoId;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao criar doação completa: {ex.Message}");
            }
        }

        private async Task<List<string>> ProcessarUploadImagensItem(
        IEnumerable<IFormFile> files,
        int doacaoId,
        int itemId,
        int instituicaoId,
        string baseUrl)
        {
            var urls = new List<string>();
            if (files == null || !files.Any())
                return urls;

            var basePath = Path.Combine(_env.ContentRootPath, "Uploads",
                $"instituicao_{instituicaoId}",
                $"doacao_{doacaoId}",
                $"item_{itemId}");

            try
            {
                Directory.CreateDirectory(basePath);

                foreach (var file in files)
                {
                    if (file.Length == 0) continue;

                    var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                    if (string.IsNullOrEmpty(extension) || !new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" }.Contains(extension))
                        continue;

                    var fileName = $"{Guid.NewGuid()}{extension}";
                    var filePath = Path.Combine(basePath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                        await file.CopyToAsync(stream);

                    var relativePath = $"instituicao_{instituicaoId}/doacao_{doacaoId}/item_{itemId}/{fileName}";
                    urls.Add($"{baseUrl.TrimEnd('/')}/api/doacao/imagem/{relativePath}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERRO AO CRIAR PASTAS/UPLOAD: {ex}");
                throw;
            }

            return urls;
        }

        private string ProcessarEnderecoDoacao(CriarDoacaoDto dto)
        {
            if (dto.TipoEntrega == "retirada")
            {
                var usuario = _usuarioRepo.GetById(dto.DoadorId);
                if (usuario == null) throw new Exception("Usuário não encontrado");

                return $"{usuario.logradouro}, {usuario.numero}, {usuario.bairro}, {usuario.cidade}-{usuario.uf}";
            }
            else 
            {
                var instituicao = _instituicaoRepo.GetById(dto.InstituicaoId);
                return $"{instituicao.logradouro}, {instituicao.numero}, {instituicao.bairro}, {instituicao.cidade}-{instituicao.uf}";
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
            WHERE s.id = @SubcategoriaId
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
    }
}