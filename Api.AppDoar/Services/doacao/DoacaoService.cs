using Api.AppDoar.Classes;
using Api.AppDoar.Classes.doacao;
using Api.AppDoar.Classes.doador;
using Api.AppDoar.Classes.instituicao;
using Api.AppDoar.Dtos.doacao;
using Api.AppDoar.Repositories.doacao;
using Api.AppDoar.Repositories.doador;
using System.Transactions;

namespace Api.AppDoar.Services.doacao
{
    public class DoacaoService
    {
        private readonly DoacaoRepositorio _doacaoRepo;
        private readonly DoadorRepositorio _doadorRepo;
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _config;

        public DoacaoService(
            DoacaoRepositorio doacaoRepo,
            DoadorRepositorio doadorRepo,
            IWebHostEnvironment env,
            IConfiguration config)
        {
            _doacaoRepo = doacaoRepo;
            _doadorRepo = doadorRepo;
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

                var doacao = new Doacao
                {
                    instituicao_id = dto.InstituicaoId,
                    horario_retirada = dto.HorarioRetirada,
                    endereco = dto.Endereco, 
                    tipo_entrega = dto.TipoEntrega,
                    usuario_id = dto.DoadorId,
                    status = "pendente",
                    status_entrega = "pendente",
                };

                var doacaoId = (int)_doacaoRepo.Create(doacao);

                foreach (var (item, imagens) in itensComArquivos)
                {


                    var doacaoItem = new DoacaoItem
                    {
                        doacao_id = doacaoId,
                        nome = item.Nome,
                        descricao = item.Descricao ?? string.Empty,
                        estado = item.Estado,
                        quantidade = item.Quantidade,
                        subcategoria_id = item.SubcategoriaId
                    };

                    var itemId = (int)_doacaoRepo.CreateItemDoacao(doacaoItem, instituicaoId);

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
                    urls.Add($"{baseUrl.TrimEnd('/')}/Uploads/{relativePath}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao processar upload: {ex.Message}");
            }

            return urls;
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

    }
}