using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Application.DTOs;

namespace Application.PdfGenerator
{
    public class InvoiceDocument : IDocument
    {
        private readonly SalesResponseDto _sales;

        public InvoiceDocument(SalesResponseDto sales)
        {
            _sales = sales;
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Header()
                    .AlignCenter()
                    .Column(column =>
                    {
                        column.Item().Text($"Sales Invoice - #{_sales.InvoiceNumber}")
                            .Bold().FontSize(16);

                        column.Item().PaddingBottom(5).LineHorizontal(1).LineColor(Colors.Grey.Medium);
                    });

                page.Content().Column(col =>
                {
                    // ── Customer Details ──
                    col.Item().PaddingVertical(10).Background(Colors.Grey.Lighten3).Padding(10).Column(info =>
                    {
                        info.Item().Text($"Date: {_sales.SaleDate:d}").FontSize(10);
                        info.Item().Text($"Customer: {_sales.CustomerName}").FontSize(10);
                    });

                    // ── Item Table ──
                    col.Item().PaddingTop(10).Table(table =>
                    {
                        // Columns
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(1); // SRNO
                            columns.RelativeColumn(3); // Product
                            columns.RelativeColumn(1); // Qty
                            columns.RelativeColumn(2); // UnitPrice
                            columns.RelativeColumn(2); // SubTotal
                            columns.RelativeColumn(2); // Discount

                            if (_sales.TotalSGST != 0 || _sales.TotalUGST != 0)
                            {
                                columns.RelativeColumn(2); // SGST/UGST
                                columns.RelativeColumn(2); // CGST
                            }
                            else if (_sales.TotalIGST != 0)
                            {
                                columns.RelativeColumn(2); // IGST
                            }

                            columns.RelativeColumn(2); // Total
                        });

                        // Header Row
                        table.Header(header =>
                        {
                            header.Cell().Element(CellHeaderStyle).Text("SRNO");
                            header.Cell().Element(CellHeaderStyle).Text("Product");
                            header.Cell().Element(CellHeaderStyle).Text("Qty");
                            header.Cell().Element(CellHeaderStyle).Text("Unit Price");
                            header.Cell().Element(CellHeaderStyle).Text("SubTotal");
                            header.Cell().Element(CellHeaderStyle).Text("Discount");

                            if (_sales.TotalSGST != 0)
                            {
                                header.Cell().Element(CellHeaderStyle).Text("SGST");
                                header.Cell().Element(CellHeaderStyle).Text("CGST");
                            }
                            else if (_sales.TotalUGST != 0)
                            {
                                header.Cell().Element(CellHeaderStyle).Text("UGST");
                                header.Cell().Element(CellHeaderStyle).Text("CGST");
                            }
                            else if (_sales.TotalIGST != 0)
                            {
                                header.Cell().Element(CellHeaderStyle).Text("IGST");
                            }

                            header.Cell().Element(CellHeaderStyle).Text("Total");
                        });

                        // Data Rows
                        int count = 0;
                        foreach (var item in _sales.Items)
                        {
                            count++;

                            table.Cell().Element(CellStyle).Text(count.ToString());
                            table.Cell().Element(CellStyle).Text(item.ProductName);
                            table.Cell().Element(CellStyle).Text($"₹{item.Quantity:F2}");
                            table.Cell().Element(CellStyle).Text($"₹{item.UnitPrice:F2}");
                            table.Cell().Element(CellStyle).Text($"₹{item.TaxableAmount:F2}");
                            table.Cell().Element(CellStyle).Text($"₹{item.DiscountAmount:F2}");

                            if (_sales.TotalSGST != 0)
                            {
                                table.Cell().Element(CellStyle).Text($"₹{item.SGST:F2}");
                                table.Cell().Element(CellStyle).Text($"₹{item.CGST:F2}");
                            }
                            else if (_sales.TotalUGST != 0)
                            {
                                table.Cell().Element(CellStyle).Text($"₹{item.UGST:F2}");
                                table.Cell().Element(CellStyle).Text($"₹{item.CGST:F2}");
                            }
                            else if (_sales.TotalIGST != 0)
                            {
                                table.Cell().Element(CellStyle).Text($"₹{item.IGST:F2}");
                            }

                            table.Cell().Element(CellStyle).Text($"₹{item.TotalAmount:F2}");
                        }
                    });

                    // ── Totals ──
                    col.Item().PaddingTop(15).AlignRight().Column(totals =>
                    {
                        totals.Item().PaddingBottom(5).Text($"SubTotal: ₹{_sales.TaxableAmount:F2}").Bold();
                        totals.Item().PaddingBottom(5).Text($"Discount Saved: ₹{_sales.TotalDiscount:F2}").Bold();

                        if (_sales.TotalSGST != 0)
                        {
                            totals.Item().PaddingBottom(2).Text($"Total SGST: ₹{_sales.TotalSGST:F2}").FontSize(9);
                            totals.Item().PaddingBottom(2).Text($"Total CGST: ₹{_sales.TotalCGST:F2}").FontSize(9);
                        }
                        else if (_sales.TotalUGST != 0)
                        {
                            totals.Item().PaddingBottom(2).Text($"Total UGST: ₹{_sales.TotalUGST:F2}").FontSize(9);
                            totals.Item().PaddingBottom(2).Text($"Total CGST: ₹{_sales.TotalCGST:F2}").FontSize(9);
                        }
                        else if (_sales.TotalIGST != 0)
                        {
                            totals.Item().PaddingBottom(2).Text($"Total IGST: ₹{_sales.TotalIGST:F2}").FontSize(9);
                        }

                        totals.Item().PaddingTop(5).PaddingBottom(5)
                            .BorderTop(1).BorderColor(Colors.Grey.Medium)
                            .Text($"Grand Total: ₹{_sales.TotalAmount:F2}")
                            .Bold().FontSize(14).FontColor(Colors.Black);
                    });
                });

                page.Footer().AlignCenter().Column(col =>
                {
                    col.Item().PaddingTop(10).LineHorizontal(1).LineColor(Colors.Grey.Medium);
                    col.Item().PaddingTop(5).Text("Thank you for your business!").Italic();
                    col.Item().Text("Come again!").Italic();
                });
            });
        }

        private IContainer CellStyle(IContainer container)
        {
            return container
                .Border(1)
                .BorderColor(Colors.Grey.Lighten1)
                .PaddingVertical(5)
                .PaddingHorizontal(2)
                .AlignMiddle()
                .AlignCenter()
                .ShowOnce();
        }

        private IContainer CellHeaderStyle(IContainer container)
        {
            return container
                .Border(1)
                .BorderColor(Colors.Grey.Darken1)
                .Background(Colors.Grey.Lighten3)
                .PaddingVertical(5)
                .PaddingHorizontal(2)
                .AlignMiddle()
                .AlignCenter()
                
                .ShowOnce();
        }
    }
}