using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Linq;
using System.Collections.Generic;

public class TermoDoacaoPdf : IDocument
{
    private readonly TermoDoacaoDto _termo;

    public TermoDoacaoPdf(TermoDoacaoDto termo)
    {
        QuestPDF.Settings.License = LicenseType.Community;
        _termo = termo;
    }

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.MarginTop(40);
            page.MarginLeft(40);
            page.MarginBottom(10);
            page.MarginRight(40);
            page.Size(PageSizes.A4);
            page.DefaultTextStyle(x => x.FontSize(11));
            page.Header().Element(ComposeHeader);
            page.Content().Element(ComposeContent);
            page.Footer().Element(ComposeFooter);
        });
    }

    void ComposeHeader(IContainer container)
    {
        container.Row(row =>
        {
            row.RelativeColumn().Column(col =>
            {
                string tipoEntrega = string.IsNullOrWhiteSpace(_termo.TipoEntrega)
                ? ""
                : char.ToUpper(_termo.TipoEntrega[0]) + _termo.TipoEntrega[1..];

                col.Item().Text($"Termo de Doação #{_termo.Id}").FontSize(20).Bold();
                col.Item().Text($"Data: {_termo.DataEntrega:dd/MM/yyyy} - {tipoEntrega}").FontSize(10).FontColor(Colors.Grey.Darken1);
            });
        });
    }

    void ComposeContent(IContainer container)
    {
        container.PaddingVertical(10).Column(coluna =>
        {
            coluna.Item().Column(col =>
            {
                col.Item().PaddingBottom(10).Column(instituicao =>
                {
                    instituicao.Item().Text(_termo.NomeInstituicao);
                    instituicao.Item().Text($"CNPJ: {_termo.CnpjInstituicao}");
                });
            });

            coluna.Item().Background(Colors.Grey.Lighten4).Padding(10).Column(col =>
            {
                col.Item().Column(assistido =>
                {
                    assistido.Item().Text(_termo.NomeAssistido);
                    assistido.Item().Text($"{_termo.TipoDocumento.ToUpper()}: {_termo.DocumentoAssistido}");
                    assistido.Item().Text($"Endereço: {_termo.EnderecoAssistido}");
                });
            });


            coluna.Item().PaddingTop(20).Text("Itens Entregues").Bold().FontSize(13);

            coluna.Item().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(30);
                    columns.RelativeColumn(3);
                    columns.RelativeColumn(3);
                    columns.RelativeColumn(1);
                });

                table.Header(header =>
                {
                    header.Cell().Element(CellHeaderStyle).Text("#").AlignCenter().Bold();
                    header.Cell().Element(CellHeaderStyle).Text("Categoria").AlignCenter().Bold();
                    header.Cell().Element(CellHeaderStyle).Text("Subcategoria").AlignCenter().Bold();
                    header.Cell().Element(CellHeaderStyle).AlignRight().Text("Quantidade").AlignCenter().Bold();
                });

                for (int i = 0; i < _termo.Itens.Count; i++)
                {
                    var item = _termo.Itens[i];
                    table.Cell().Element(CellStyle).Text($"{i + 1}").AlignCenter();
                    table.Cell().Element(CellStyle).Text(item.Categoria).AlignCenter();
                    table.Cell().Element(CellStyle).Text(item.Subcategoria).AlignCenter();
                    table.Cell().Element(CellStyle).AlignRight().Text($"{item.Quantidade}").AlignCenter();
                }

                table.Cell().ColumnSpan(3).PaddingTop(10).AlignRight().Text("Total").Bold();
                table.Cell().PaddingTop(10).AlignRight().Text($"{_termo.Itens.Sum(i => i.Quantidade)}").Bold();
            });

            if (!string.IsNullOrWhiteSpace(_termo.Observacao))
                coluna.Item().PaddingTop(10).Text($"Observações: {_termo.Observacao}").Italic();

            coluna.Item().PaddingTop(20).Text("Declaro que recebi os itens acima descritos da instituição informada, para meu uso pessoal.").LineHeight(1.5f);

            coluna.Item().PaddingTop(60).Row(row =>
            {
                row.RelativeColumn().AlignCenter().Column(col =>
                {
                    col.Item().Width(200).LineHorizontal(1);
                    col.Item().PaddingTop(5).AlignCenter().Text("Assinatura do Assistido")
                        .FontSize(10)
                        .FontColor(Colors.Grey.Darken1);
                });

                row.RelativeColumn().AlignCenter().Column(col =>
                {
                    col.Item().Width(200).LineHorizontal(1);
                    col.Item().PaddingTop(5).AlignCenter().Text("Assinatura do Agente")
                        .FontSize(10)
                        .FontColor(Colors.Grey.Darken1);
                });
            });

        });
    }

    void ComposeFooter(IContainer container)
    {
        container.Row(row =>
        {
            row.RelativeColumn().AlignCenter().Column(col =>
            {
                col.Item().Text("Desenvolvido por AppDoar").FontSize(10).FontColor(Colors.Grey.Darken1);
            });
        });
    }

    IContainer CellStyle(IContainer container)
    {
        return container
            .BorderBottom(1)
            .BorderColor(Colors.Grey.Lighten2)
            .PaddingVertical(5)
            .PaddingHorizontal(2);
    }
    
    IContainer CellHeaderStyle(IContainer container)
    {
        return container
            .BorderBottom(1)
            .BorderColor(Colors.Black)
            .PaddingVertical(5)
            .PaddingHorizontal(2);
    }
}
