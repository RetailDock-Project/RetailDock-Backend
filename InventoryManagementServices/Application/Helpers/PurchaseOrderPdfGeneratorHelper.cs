using System;
using System.Linq;
using Application.Dto;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Application.Helpers
{
    public class PurchaseOrderPdfGeneratorHelper
    {
        public static byte[] GeneratePdf(PurchaseOrderDetailDto order, OrganizationDetailsDto organizationDatail)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(1, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontSize(11));

                    // Header
                    page.Header().Column(column =>
                    {
                        column.Item().Text($"Purchase Order #{order.PurchaseOrderNumber}")
                            .FontSize(18).Bold().FontColor(Colors.Blue.Medium);

                        column.Item().Text(text =>
                        {
                            text.Span("Date: ").SemiBold();
                            text.Span($"{DateTime.Now:yyyy-MM-dd}");
                        });
                    });

                    // Main content
                    page.Content().PaddingVertical(10).Column(column =>
                    {
                        // Organization Details Section
                        column.Item().Element(container =>
                        {
                            container
                                .Border(1)
                                .BorderColor(Colors.Grey.Lighten2)
                                .Padding(10)
                                .Column(orgCol =>
                                {
                                    orgCol.Item().PaddingBottom(5).Text("Organization Details")
                                        .FontSize(13).Bold().Underline();

                                    orgCol.Item().Text(text =>
                                    {
                                        text.Span("Name: ").SemiBold();
                                        text.Span(organizationDatail.OrganizationName);
                                    });

                                    orgCol.Item().Text(text =>
                                    {
                                        text.Span("Address: ").SemiBold();
                                        text.Span(organizationDatail.Address);
                                    });

                                    orgCol.Item().Text(text =>
                                    {
                                        text.Span("License No: ").SemiBold();
                                        text.Span(organizationDatail.LicenceNumber);
                                    });

                                    orgCol.Item().Text(text =>
                                    {
                                        text.Span("GST No: ").SemiBold();
                                        text.Span(organizationDatail.GSTNumber);
                                    });

                                    orgCol.Item().Text(text =>
                                    {
                                        text.Span("PAN No: ").SemiBold();
                                        text.Span(organizationDatail.PANNumber);
                                    });
                                });
                        });

                        // Supplier section
                        column.Item().PaddingTop(10).Element(container =>
                        {
                            container
                                .Border(1)
                                .BorderColor(Colors.Grey.Lighten2)
                                .Padding(10)
                                .Column(supplierCol =>
                                {
                                    supplierCol.Item().PaddingBottom(5).Text("Supplier Details")
                                        .FontSize(13).Bold().Underline();

                                    supplierCol.Item().Text(text =>
                                    {
                                        text.Span("Name: ").SemiBold();
                                        text.Span(order.Supplier.Name);
                                    });

                                    supplierCol.Item().Text(text =>
                                    {
                                        text.Span("Phone: ").SemiBold();
                                        text.Span(order.Supplier.ContactNumber);
                                    });

                                    supplierCol.Item().Text(text =>
                                    {
                                        text.Span("Address: ").SemiBold();
                                        text.Span(order.Supplier.Address);
                                    });

                                    supplierCol.Item().Text(text =>
                                    {
                                        text.Span("City: ").SemiBold();
                                        text.Span(order.Supplier.City);
                                    });

                                    supplierCol.Item().Text(text =>
                                    {
                                        text.Span("Pincode: ").SemiBold();
                                        text.Span(order.Supplier.Pincode);
                                    });

                                    supplierCol.Item().Text(text =>
                                    {
                                        text.Span("GST No: ").SemiBold();
                                        text.Span(order.Supplier.GSTNumber);
                                    });
                                });
                        });

                        // Product Table
                        column.Item().PaddingTop(15).Element(BuildProductTable(order));
                    });

                    // Footer
                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Generated on ");
                        x.Span($"{DateTime.Now:yyyy-MM-dd HH:mm}");
                    });
                });
            });

            return document.GeneratePdf();
        }

        private static Action<IContainer> BuildProductTable(PurchaseOrderDetailDto order)
        {
            return container => container
                .Border(1)
                .BorderColor(Colors.Grey.Lighten2)
                .Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(3);  // Product Name
                        columns.ConstantColumn(60); // Qty
                        columns.ConstantColumn(80); // Price
                        columns.ConstantColumn(80); // Total
                    });

                    // Header
                    table.Header(header =>
                    {
                        header.Cell().Element(CellStyle).Background(Colors.Grey.Lighten2).Text("Product").Bold();
                        header.Cell().Element(CellStyle).AlignRight().Background(Colors.Grey.Lighten2).Text("Qty").Bold();
                        header.Cell().Element(CellStyle).AlignRight().Background(Colors.Grey.Lighten2).Text("Price").Bold();
                        header.Cell().Element(CellStyle).AlignRight().Background(Colors.Grey.Lighten2).Text("Total").Bold();
                    });

                    // Rows
                    foreach (var item in order.Items)
                    {
                        table.Cell().Element(CellStyle).Text(item.ProductName);
                        table.Cell().Element(CellStyle).AlignRight().Text(item.Quantity.ToString("F2"));
                        table.Cell().Element(CellStyle).AlignRight().Text(item.RatePerPiece.ToString("F2"));
                        table.Cell().Element(CellStyle).AlignRight().Text(item.TotalAmount.ToString("F2"));
                    }

                    // Footer Total
                    table.Footer(footer =>
                    {
                        footer.Cell().ColumnSpan(3).Element(CellStyle).AlignRight().Text("Total").Bold();
                        footer.Cell().Element(CellStyle).AlignRight().Text(order.TotalAmount.ToString("F2")).Bold();
                    });
                });
        }

        private static IContainer CellStyle(IContainer container)
        {
            return container
                .PaddingVertical(5)
                .PaddingHorizontal(5)
                .BorderBottom(1)
                .BorderColor(Colors.Grey.Lighten3);
        }
    }
}
