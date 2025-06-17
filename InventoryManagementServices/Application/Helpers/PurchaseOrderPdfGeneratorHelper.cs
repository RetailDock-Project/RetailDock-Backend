using System;
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
                    page.Margin(50);

                    
                    page.Header().Text($"Purchase Order - {order.OrderDate:yyyy-MM-dd}")
                        .FontSize(20)
                        .Bold()
                        .AlignCenter();

                   
                    page.Content().Element(content =>
                    {
                        content.Column(column =>
                        {
                            column.Spacing(10);

                          
                            column.Item().Text($"Supplier Name: {order.SupplierName}");
                            column.Item().Text($"Created By: {order.CreatedBy}");

                            column.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                           
                            column.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(4); 
                                    columns.RelativeColumn(2); 
                                    columns.RelativeColumn(2); 
                                    columns.RelativeColumn(2); 
                                });

                                
                                table.Header(header =>
                                {
                                    header.Cell().Element(CellStyle).Text("Product Name").Bold();
                                    header.Cell().Element(CellStyle).AlignRight().Text("Qty").Bold();
                                    header.Cell().Element(CellStyle).AlignRight().Text("Rate").Bold();
                                    header.Cell().Element(CellStyle).AlignRight().Text("Total").Bold();
                                });

                               
                                foreach (var item in order.Items)
                                {
                                    table.Cell().Element(CellStyle).Text(item.ProductName);
                                    table.Cell().Element(CellStyle).AlignRight().Text(item.Quantity.ToString());
                                    table.Cell().Element(CellStyle).AlignRight().Text(item.RatePerPiece.ToString("C"));
                                    table.Cell().Element(CellStyle).AlignRight().Text(item.TotalAmount.ToString("C"));
                                }

                                static IContainer CellStyle(IContainer container)
                                {
                                    return container
                                        .PaddingVertical(5)
                                        .BorderBottom(1)
                                        .BorderColor(Colors.Grey.Lighten3);
                                }
                            });

                           
                            column.Item().Element(e =>
                                e.AlignRight().PaddingTop(15).Text(text =>
                                {
                                    text.Span($"Gross Total: {order.GrossTotalAmount:C}")
                                        .FontSize(14)
                                        .Bold();
                                })
                            );
                        });
                    });

                   
                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.DefaultTextStyle(ts => ts.FontSize(10));
                        x.Span("Generated on ");
                        x.Span($"{DateTime.Now:yyyy-MM-dd HH:mm}");
                    });
                });
            });

            return document.GeneratePdf();
        }
    }
}
