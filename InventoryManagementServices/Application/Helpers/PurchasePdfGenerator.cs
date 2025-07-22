using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Dto;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Application.Helpers
{
    public class PurchasePdfGenerator
    {
        public static byte[] GeneratePurchasePdf(GetPurchaseDetailsDto purchase, OrganizationDetailsDto orgData)
        {
            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(30);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(12).FontFamily("Arial"));

                    // Header
                    page.Header().Column(headerCol =>
                    {
                        headerCol.Item().Text("Purchase Invoice")
                            .SemiBold().FontSize(20).FontColor(Colors.Blue.Medium);

                        headerCol.Item().PaddingTop(5).Column(orgCol =>
                        {
                            orgCol.Item().Text(orgData.OrganizationName).Bold();
                            orgCol.Item().Text(orgData.Address);
                            orgCol.Item().Text($"License: {orgData.LicenceNumber}");
                            orgCol.Item().Text($"GST: {orgData.GSTNumber} | PAN: {orgData.PANNumber}");
                        });
                    });

                    // Content
                    page.Content().Element(content =>
                    {
                        content.Column(col =>
                        {
                            // Purchase + Supplier Details
                            col.Item().Row(row =>
                            {
                                row.RelativeItem().Column(colLeft =>
                                {
                                    colLeft.Item().Text($"Purchase ID: {purchase.PurchaseId}");
                                    colLeft.Item().Text($"Purchase Date: {purchase.Purchasedate:dd MMMM yyyy}");
                                    colLeft.Item().Text($"Purchase Order No: {purchase.PurchaseOrderNumber ?? "N/A"}");
                                    colLeft.Item().Text($"Supplier Invoice No: {purchase.SupplierInvoiceNumber ?? "N/A"}");
                                });

                                row.RelativeItem().Column(colRight =>
                                {
                                    colRight.Item().Text("Supplier Details").Bold();
                                    colRight.Item().Text($"Name: {purchase.SupplierDetails.Name}");
                                    colRight.Item().Text($"GST: {purchase.SupplierDetails.GSTNumber}");
                                    colRight.Item().Text($"Phone: {purchase.SupplierDetails.ContactNumber}");
                                    colRight.Item().Text($"Address: {purchase.SupplierDetails.Address}, {purchase.SupplierDetails.City}");
                                });
                            });

                            // Table Header
                            col.Item().PaddingTop(15).Text("Items Purchased").Bold().FontSize(14);
                            col.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn();      // Product
                                    columns.ConstantColumn(50);    // Qty
                                    columns.ConstantColumn(70);    // Rate
                                    columns.ConstantColumn(70);    // Tax
                                    columns.ConstantColumn(80);    // Total
                                });

                                // Header row
                                table.Header(header =>
                                {
                                    header.Cell().Element(CellStyle).Text("Product");
                                    header.Cell().Element(CellStyle).AlignCenter().Text("Qty");
                                    header.Cell().Element(CellStyle).AlignRight().Text("Rate");
                                    header.Cell().Element(CellStyle).AlignRight().Text("Tax");
                                    header.Cell().Element(CellStyle).AlignRight().Text("Total");

                                    static IContainer CellStyle(IContainer container)
                                    {
                                        return container.DefaultTextStyle(x => x.SemiBold()).Padding(5).Background(Colors.Grey.Lighten2);
                                    }
                                });

                                // Rows
                                foreach (var item in purchase.Items)
                                {
                                    table.Cell().Element(cell => cell.Padding(5)).Text(item.ProductName);
                                    table.Cell().Element(cell => cell.Padding(5)).AlignCenter().Text($"{item.Quantity:F2}");
                                    table.Cell().Element(cell => cell.Padding(5)).AlignRight().Text($"₹{item.RatePerPiece:F2}");
                                    table.Cell().Element(cell => cell.Padding(5)).AlignRight().Text($"₹{item.TaxAmount:F2}");
                                    table.Cell().Element(cell => cell.Padding(5)).AlignRight().Text($"₹{item.TotalAmount:F2}");
                                }
                            });

                            // Total
                            decimal total = purchase.Items.Sum(i => i.TotalAmount);
                            col.Item().Element(x =>
                                x.AlignRight()
                                 .PaddingTop(10)
                                 .Text($"Total: ₹{total:F2}")
                                 .Bold()
                                 .FontSize(14)
                            );
                        });
                    });

                    // Footer
                    page.Footer().AlignCenter().Text($"Generated by {orgData.OrganizationName} • {DateTime.Now:dd MMM yyyy HH:mm}").FontSize(10);
                });
            })
            .GeneratePdf();
        }
    }
}
