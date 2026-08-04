using System.Globalization;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using PlanejadorCompras.Application.Common.Dtos.Reports;

namespace PlanejadorCompras.Infrastructure.Reports.Pdf;

public sealed class ShoppingListPdfDocumentBuilder
{
    private const int MaxSuppliersPerGroup = 4;
    private const int HeaderDescriptionLimit = 180;
    private const int TableTextLimit = 80;
    private static readonly CultureInfo BrazilianCulture =
        CultureInfo.GetCultureInfo("pt-BR");
    private static readonly Color DarkBlue = Color.FromRgb(24, 24, 27);
    private static readonly Color BorderBlue = Color.FromRgb(212, 212, 216);
    private static readonly Color LightBlue = Color.FromRgb(244, 244, 245);
    private static readonly Color BestPriceBackground = Color.FromRgb(226, 240, 217);
    private static readonly Color BestPriceForeground = Color.FromRgb(0, 97, 0);
    private static readonly Color MissingPriceBackground = Color.FromRgb(255, 242, 204);
    private static readonly Color MissingPriceForeground = Color.FromRgb(156, 101, 0);

    public Document Build(ShoppingListReportDataDto reportData)
    {
        ArgumentNullException.ThrowIfNull(reportData);

        var document = CreateDocument(reportData);
        var supplierGroups = CreateSupplierGroups(reportData.Suppliers);
        for (var groupIndex = 0; groupIndex < supplierGroups.Count; groupIndex++)
        {
            var section = document.AddSection();
            ConfigureSection(section);
            AddHeader(section, reportData);
            AddFooter(section);

            if (groupIndex == 0)
            {
                AddReportIntroduction(section, reportData);
                AddSummary(section, reportData);
            }

            AddPriceMapTitle(
                section,
                groupIndex,
                supplierGroups.Count,
                reportData.Suppliers.Count);
            AddPriceMapTable(section, reportData, supplierGroups[groupIndex]);
        }

        return document;
    }

    private static Document CreateDocument(ShoppingListReportDataDto reportData)
    {
        var document = new Document();
        document.Info.Title = $"Equalização - {LimitText(reportData.Name, TableTextLimit)}";
        document.Info.Subject = "Relatório de equalização de preços";

        var normalStyle = document.Styles[StyleNames.Normal]!;
        normalStyle.Font.Name = EmbeddedPdfFontResolver.FamilyName;
        normalStyle.Font.Size = Unit.FromPoint(8.5);
        normalStyle.ParagraphFormat.SpaceAfter = Unit.FromPoint(2);

        var headingStyle = document.Styles[StyleNames.Heading1]!;
        headingStyle.Font.Name = EmbeddedPdfFontResolver.FamilyName;
        headingStyle.Font.Size = Unit.FromPoint(13);
        headingStyle.Font.Bold = true;
        headingStyle.Font.Color = DarkBlue;
        headingStyle.ParagraphFormat.SpaceBefore = Unit.FromPoint(7);
        headingStyle.ParagraphFormat.SpaceAfter = Unit.FromPoint(5);
        headingStyle.ParagraphFormat.KeepWithNext = true;

        return document;
    }

    private static IReadOnlyList<IReadOnlyList<ShoppingListReportSupplierDto>>
        CreateSupplierGroups(
            IReadOnlyCollection<ShoppingListReportSupplierDto> suppliers)
    {
        if (suppliers.Count == 0)
        {
            return new[] { Array.Empty<ShoppingListReportSupplierDto>() };
        }

        return suppliers
            .Chunk(MaxSuppliersPerGroup)
            .Select(group => (IReadOnlyList<ShoppingListReportSupplierDto>)group)
            .ToList();
    }

    private static void ConfigureSection(Section section)
    {
        section.PageSetup.PageFormat = PageFormat.A4;
        section.PageSetup.Orientation = Orientation.Landscape;
        section.PageSetup.LeftMargin = Unit.FromCentimeter(1.2);
        section.PageSetup.RightMargin = Unit.FromCentimeter(1.2);
        section.PageSetup.TopMargin = Unit.FromCentimeter(3.1);
        section.PageSetup.BottomMargin = Unit.FromCentimeter(1.6);
        section.PageSetup.HeaderDistance = Unit.FromCentimeter(0.6);
        section.PageSetup.FooterDistance = Unit.FromCentimeter(0.7);
        section.PageSetup.DifferentFirstPageHeaderFooter = false;
        section.PageSetup.OddAndEvenPagesHeaderFooter = false;
    }

    private static void AddHeader(
        Section section,
        ShoppingListReportDataDto reportData)
    {
        var headerTable = section.Headers.Primary.AddTable();
        headerTable.AddColumn(Unit.FromCentimeter(26.9));
        headerTable.Borders.Bottom.Width = Unit.FromPoint(0.8);
        headerTable.Borders.Bottom.Color = BorderBlue;

        var row = headerTable.AddRow();
        row.VerticalAlignment = VerticalAlignment.Center;

        var listParagraph = row.Cells[0].AddParagraph();
        listParagraph.Format.SpaceAfter = Unit.FromPoint(1);
        listParagraph.AddFormattedText("Lista: ", TextFormat.Bold);
        listParagraph.AddText(LimitText(reportData.Name, TableTextLimit));

        if (!string.IsNullOrWhiteSpace(reportData.Description))
        {
            var descriptionParagraph = row.Cells[0].AddParagraph();
            descriptionParagraph.Format.Font.Size = Unit.FromPoint(7.2);
            descriptionParagraph.Format.Font.Color = Colors.DimGray;
            descriptionParagraph.AddText(
                LimitText(reportData.Description, HeaderDescriptionLimit));
        }

        var generatedParagraph = row.Cells[0].AddParagraph();
        generatedParagraph.Format.Font.Size = Unit.FromPoint(7.2);
        generatedParagraph.Format.Font.Color = Colors.DimGray;
        generatedParagraph.AddText(
            $"Gerado em {reportData.GeneratedAt.ToString("dd/MM/yyyy HH:mm", BrazilianCulture)}");
    }

    private static void AddFooter(Section section)
    {
        var footer = section.Footers.Primary.AddParagraph();
        footer.Format.Font.Size = Unit.FromPoint(7);
        footer.Format.Font.Color = Colors.DimGray;
        footer.Format.Alignment = ParagraphAlignment.Right;
        footer.Format.Borders.Top.Width = Unit.FromPoint(0.5);
        footer.Format.Borders.Top.Color = BorderBlue;
        footer.Format.SpaceBefore = Unit.FromPoint(3);
        footer.AddText("Página ");
        footer.AddPageField();
        footer.AddText(" de ");
        footer.AddNumPagesField();
    }

    private static void AddReportIntroduction(
        Section section,
        ShoppingListReportDataDto reportData)
    {
        var title = section.AddParagraph("Equalização de preços");
        title.Format.Font.Size = Unit.FromPoint(20);
        title.Format.Font.Bold = true;
        title.Format.Font.Color = DarkBlue;
        title.Format.SpaceAfter = Unit.FromPoint(3);
        title.Format.KeepWithNext = true;

        var subtitle = section.AddParagraph(
            "Comparativo consolidado dos preços informados pelos fornecedores.");
        subtitle.Format.Font.Size = Unit.FromPoint(8);
        subtitle.Format.Font.Color = Colors.DimGray;
        subtitle.Format.SpaceAfter = Unit.FromPoint(7);
        subtitle.Format.KeepWithNext = true;

        if (reportData.Items.Count == 0)
        {
            var warning = section.AddParagraph(
                "Esta lista ainda não possui itens cadastrados.");
            warning.Format.Shading.Color = MissingPriceBackground;
            warning.Format.Font.Color = MissingPriceForeground;
            warning.Format.LeftIndent = Unit.FromPoint(5);
            warning.Format.RightIndent = Unit.FromPoint(5);
            warning.Format.SpaceBefore = Unit.FromPoint(3);
            warning.Format.SpaceAfter = Unit.FromPoint(7);
        }
    }

    private static void AddSummary(
        Section section,
        ShoppingListReportDataDto reportData)
    {
        section.AddParagraph("Resumo da decisão", StyleNames.Heading1);

        var table = section.AddTable();
        table.Borders.Width = Unit.FromPoint(0.4);
        table.Borders.Color = BorderBlue;

        for (var column = 0; column < 4; column++)
        {
            table.AddColumn(Unit.FromCentimeter(6.68));
        }

        var firstRow = table.AddRow();
        AddSummaryCell(
            firstRow.Cells[0],
            "Menores preços por item",
            FormatCurrency(reportData.Summary.BestChoiceTotal));
        AddSummaryCell(
            firstRow.Cells[1],
            "Melhor fornecedor completo",
            LimitText(
                reportData.Summary.BestCompleteSupplierName ?? "Não disponível",
                42));
        AddSummaryCell(
            firstRow.Cells[2],
            "Total do fornecedor",
            FormatOptionalCurrency(reportData.Summary.BestCompleteSupplierTotal));
        AddSummaryCell(
            firstRow.Cells[3],
            "Economia estimada",
            FormatOptionalCurrency(reportData.Summary.PotentialSavings));
    }

    private static void AddSummaryCell(Cell cell, string label, string value)
    {
        cell.VerticalAlignment = VerticalAlignment.Center;
        cell.Format.Alignment = ParagraphAlignment.Center;
        cell.Shading.Color = LightBlue;

        var labelParagraph = cell.AddParagraph();
        labelParagraph.Format.Font.Size = Unit.FromPoint(7);
        labelParagraph.Format.Font.Color = Colors.DimGray;
        labelParagraph.AddText(label);

        var valueParagraph = cell.AddParagraph();
        valueParagraph.Format.Font.Size = Unit.FromPoint(9);
        valueParagraph.Format.Font.Bold = true;
        valueParagraph.Format.Font.Color = DarkBlue;
        valueParagraph.AddText(value);
    }

    private static void AddPriceMapTitle(
        Section section,
        int groupIndex,
        int groupCount,
        int supplierCount)
    {
        var title = section.AddParagraph("Mapa comparativo", StyleNames.Heading1);

        if (groupCount <= 1)
        {
            return;
        }

        var firstSupplier = (groupIndex * MaxSuppliersPerGroup) + 1;
        var lastSupplier = Math.Min(
            firstSupplier + MaxSuppliersPerGroup - 1,
            supplierCount);
        var context = section.AddParagraph(
            $"Fornecedores {firstSupplier} a {lastSupplier} de {supplierCount}");
        context.Format.Font.Size = Unit.FromPoint(7.5);
        context.Format.Font.Color = Colors.DimGray;
        context.Format.SpaceAfter = Unit.FromPoint(4);
        context.Format.KeepWithNext = true;
    }

    private static void AddPriceMapTable(
        Section section,
        ShoppingListReportDataDto reportData,
        IReadOnlyList<ShoppingListReportSupplierDto> suppliers)
    {
        var table = section.AddTable();
        table.Borders.Width = Unit.FromPoint(0.35);
        table.Borders.Color = BorderBlue;
        table.Format.Font.Size = Unit.FromPoint(7.2);
        table.AddColumn(Unit.FromCentimeter(5.5));
        table.AddColumn(Unit.FromCentimeter(1.4));
        table.AddColumn(Unit.FromCentimeter(1.3));

        if (suppliers.Count == 0)
        {
            table.AddColumn(Unit.FromCentimeter(18.5));
        }
        else
        {
            var supplierColumnWidth = 18.4 / suppliers.Count;

            foreach (var _ in suppliers)
            {
                table.AddColumn(Unit.FromCentimeter(supplierColumnWidth));
            }
        }

        var header = table.AddRow();
        header.HeadingFormat = true;
        header.VerticalAlignment = VerticalAlignment.Center;
        header.Shading.Color = DarkBlue;
        header.Format.Font.Bold = true;
        header.Format.Font.Color = Colors.White;
        header.Format.Alignment = ParagraphAlignment.Center;
        header.TopPadding = Unit.FromPoint(4);
        header.BottomPadding = Unit.FromPoint(4);

        AddCellText(header.Cells[0], "Item", ParagraphAlignment.Left);
        AddCellText(header.Cells[1], "Qtd.");
        AddCellText(header.Cells[2], "Un.");

        if (suppliers.Count == 0)
        {
            AddCellText(header.Cells[3], "Situação");
        }
        else
        {
            for (var supplierIndex = 0; supplierIndex < suppliers.Count; supplierIndex++)
            {
                AddCellText(
                    header.Cells[supplierIndex + 3],
                    LimitText(suppliers[supplierIndex].Name, 42));
            }
        }

        var itemIndex = 0;

        foreach (var item in reportData.Items)
        {
            var row = table.AddRow();
            row.VerticalAlignment = VerticalAlignment.Center;
            row.TopPadding = Unit.FromPoint(3);
            row.BottomPadding = Unit.FromPoint(3);

            if (itemIndex % 2 == 1)
            {
                row.Shading.Color = LightBlue;
            }

            AddCellText(
                row.Cells[0],
                LimitText(item.Name, TableTextLimit),
                ParagraphAlignment.Left);
            AddCellText(
                row.Cells[1],
                item.Quantity.ToString("0.###", BrazilianCulture));
            AddCellText(
                row.Cells[2],
                LimitText(item.Unit, 16));

            if (suppliers.Count == 0)
            {
                AddMissingPriceCell(
                    row.Cells[3],
                    "Nenhum fornecedor cadastrado");
            }
            else
            {
                AddSupplierPrices(row, item, suppliers);
            }

            itemIndex++;
        }

        AddSupplierTotals(table, reportData, suppliers);
    }

    private static void AddSupplierPrices(
        Row row,
        ShoppingListReportItemDto item,
        IReadOnlyList<ShoppingListReportSupplierDto> suppliers)
    {
        var quotesBySupplier = item.Quotes.ToDictionary(quote => quote.SupplierId);

        for (var supplierIndex = 0; supplierIndex < suppliers.Count; supplierIndex++)
        {
            var supplier = suppliers[supplierIndex];
            var cell = row.Cells[supplierIndex + 3];

            if (!quotesBySupplier.TryGetValue(supplier.SupplierId, out var quote))
            {
                AddMissingPriceCell(cell, "Preço não informado");
                continue;
            }

            cell.Format.Alignment = ParagraphAlignment.Center;

            var unitPrice = cell.AddParagraph();
            unitPrice.Format.Font.Bold = true;
            unitPrice.AddText(FormatCurrency(quote.UnitPrice));

            var totalPrice = cell.AddParagraph();
            totalPrice.Format.Font.Size = Unit.FromPoint(6.7);
            totalPrice.AddText($"Total: {FormatCurrency(quote.TotalPrice)}");

            if (quote.IsLowestPrice)
            {
                cell.Shading.Color = BestPriceBackground;
                cell.Format.Font.Color = BestPriceForeground;

                var bestPrice = cell.AddParagraph();
                bestPrice.Format.Font.Size = Unit.FromPoint(6.2);
                bestPrice.Format.Font.Bold = true;
                bestPrice.AddText("Melhor preço");
            }
        }
    }

    private static void AddSupplierTotals(
        Table table,
        ShoppingListReportDataDto reportData,
        IReadOnlyList<ShoppingListReportSupplierDto> suppliers)
    {
        var totalRow = table.AddRow();
        totalRow.VerticalAlignment = VerticalAlignment.Center;
        totalRow.Format.Font.Bold = true;
        totalRow.TopPadding = Unit.FromPoint(4);
        totalRow.BottomPadding = Unit.FromPoint(4);
        totalRow.Borders.Top.Width = Unit.FromPoint(0.9);
        totalRow.Cells[0].MergeRight = 2;
        AddCellText(
            totalRow.Cells[0],
            "Total cotado por fornecedor",
            ParagraphAlignment.Left);

        if (suppliers.Count == 0)
        {
            AddMissingPriceCell(totalRow.Cells[3], "Sem preços");
            return;
        }

        for (var supplierIndex = 0; supplierIndex < suppliers.Count; supplierIndex++)
        {
            var supplier = suppliers[supplierIndex];
            var cell = totalRow.Cells[supplierIndex + 3];
            cell.Format.Alignment = ParagraphAlignment.Center;

            var total = cell.AddParagraph();
            total.AddText(FormatCurrency(supplier.QuotedTotal));

            var status = cell.AddParagraph();
            status.Format.Font.Size = Unit.FromPoint(6.3);
            status.AddText(
                supplier.HasCompleteCoverage
                    ? "Cobertura completa"
                    : $"{supplier.MissingItemCount} pendente(s)");

            if (supplier.SupplierId == reportData.Summary.BestCompleteSupplierId)
            {
                cell.Shading.Color = BestPriceBackground;
                cell.Format.Font.Color = BestPriceForeground;
            }
            else if (!supplier.HasCompleteCoverage)
            {
                cell.Shading.Color = MissingPriceBackground;
                cell.Format.Font.Color = MissingPriceForeground;
            }
        }
    }

    private static void AddMissingPriceCell(Cell cell, string message)
    {
        cell.Shading.Color = MissingPriceBackground;
        cell.Format.Font.Color = MissingPriceForeground;
        cell.Format.Alignment = ParagraphAlignment.Center;

        var paragraph = cell.AddParagraph();
        paragraph.Format.Font.Size = Unit.FromPoint(6.8);
        paragraph.AddText(message);
    }

    private static void AddCellText(
        Cell cell,
        string text,
        ParagraphAlignment alignment = ParagraphAlignment.Center)
    {
        cell.Format.Alignment = alignment;
        cell.AddParagraph(text);
    }

    private static string FormatCurrency(decimal value)
    {
        return value.ToString("C2", BrazilianCulture);
    }

    private static string FormatOptionalCurrency(decimal? value)
    {
        return value.HasValue ? FormatCurrency(value.Value) : "Não disponível";
    }

    private static string LimitText(string value, int maximumLength)
    {
        if (value.Length <= maximumLength)
        {
            return value;
        }

        return $"{value[..(maximumLength - 3)]}...";
    }
}
