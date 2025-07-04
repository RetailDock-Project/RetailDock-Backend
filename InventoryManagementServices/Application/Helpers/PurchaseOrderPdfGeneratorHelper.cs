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
        public static byte[] GeneratePdf(PurchaseOrderDto order)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(1, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontSize(11));

                    // Header with basic info
                    page.Header().Column(column =>
                    {
                        column.Item().Text($"Purchase Order #{order.PurchaseOrderNumber}")
                            .FontSize(16)
                            .Bold();

                        column.Item().Text(text =>
                        {
                            text.Span("Date: ").SemiBold();
                            text.Span($"{DateTime.Now:yyyy-MM-dd}");
                        });
                    });

                    // Main content
                    page.Content().PaddingVertical(10).Column(column =>
                    {
                        // Supplier information
                        column.Item().PaddingBottom(10).Column(col =>
                        {
                            col.Item().Text("Supplier").FontSize(12).Bold();
                            col.Item().Text(order.Supplier.Name);
                            col.Item().Text(order.Supplier.ContactNumber);
                            col.Item().Text(order.Supplier.Address);

                            col.Item().Text(order.Supplier.City);
                            col.Item().Text(order.Supplier.Pincode);

                            col.Item().Text(order.Supplier.GSTNumber);

                        });


                        // Created by
                        column.Item().PaddingBottom(15).Text(text =>
                        {
                            text.Span("Created by: ").SemiBold();
                            text.Span(order.CreatedBy.ToString());
                        });

                        // Products table
                        column.Item().Table(table =>
                        {
                            // Define columns
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(3);  // Product
                                columns.ConstantColumn(60); // Qty
                                columns.ConstantColumn(80); // Unit Price
                                columns.ConstantColumn(80); // Total
                            });

                            // Header row
                            table.Header(header =>
                            {
                                header.Cell().Text("Product").Bold();
                                header.Cell().AlignRight().Text("Qty").Bold();
                                header.Cell().AlignRight().Text("Price").Bold();
                                header.Cell().AlignRight().Text("Total").Bold();
                            });

                            // Product rows
                            foreach (var item in order.Items)
                            {
                                table.Cell().Text(item.ProductName);
                                table.Cell().AlignRight().Text(item.Quantity.ToString());
                                table.Cell().AlignRight().Text(item.RatePerPiece.ToString("C"));
                                table.Cell().AlignRight().Text(item.TotalAmount.ToString("C"));
                            }

                            // Total row
                            table.Footer(footer =>
                            {
                                footer.Cell().ColumnSpan(3).Text("Total").Bold();
                                footer.Cell().AlignRight().Text(order.GrossTotalAmount.ToString("C")).Bold();
                            });
                        });
                    });

                    // Simple footer
                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Generated on ");
                        x.Span($"{DateTime.Now:yyyy-MM-dd HH:mm}");
                    });
                });
            });

            return document.GeneratePdf();
        }
    }
}