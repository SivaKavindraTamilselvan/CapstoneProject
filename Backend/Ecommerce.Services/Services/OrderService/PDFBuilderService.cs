using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

public static class OrderInvoicePdfBuilder
{
    private static readonly string PrimaryDark = Colors.Blue.Darken3;
    private static readonly string Primary = Colors.Blue.Darken2;
    private static readonly string PrimaryLight = Colors.Blue.Lighten4;
    private static readonly string PrimaryLighter = Colors.Blue.Lighten5;
    private static readonly string SuccessColor = Colors.Green.Darken1;
    private static readonly string TextMuted = Colors.Grey.Darken1;
    private static readonly string TextLight = Colors.Grey.Medium;
    private static readonly string BorderLight = Colors.Grey.Lighten2;

    public static byte[] Build(OrderInvoiceDto data)
    {
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(0);
                page.DefaultTextStyle(x => x.FontSize(10).FontColor(Colors.Grey.Darken3));

                page.Content().Column(col =>
                {
                    // Header band
                    col.Item().Background(PrimaryDark).Padding(30).Row(row =>
                    {
                        row.RelativeItem().Column(c =>
                        {
                            c.Item().Text("INVOICE").FontSize(22).Bold().FontColor(Colors.White);
                            c.Item().PaddingTop(4).Text($"Order #{data.OrderNumber}").FontSize(13).FontColor(PrimaryLight);
                        });
                        row.ConstantItem(140).AlignRight().Column(c =>
                        {
                            c.Item().AlignRight().Text("ShopperApp").FontSize(11).Bold().FontColor(Colors.White);
                            c.Item().AlignRight().Text($"{data.OrderDate:dd MMM yyyy}").FontSize(9).FontColor(PrimaryLight);
                        });
                    });

                    col.Item().Padding(30).Column(inner =>
                    {
                        // Customer + address block
                        inner.Item().Row(r =>
                        {
                            r.RelativeItem().Column(c =>
                            {
                                c.Item().Text("Billed To").FontSize(9).Bold().FontColor(TextLight);
                                c.Item().PaddingTop(4).Text(data.CustomerName).FontSize(12).Bold().FontColor(PrimaryDark);
                                c.Item().Text(data.CustomerEmail).FontSize(9).FontColor(TextMuted);
                                c.Item().Text(data.CustomerPhone).FontSize(9).FontColor(TextMuted);
                            });
                            r.RelativeItem().Column(c =>
                            {
                                c.Item().Text("Shipping Address").FontSize(9).Bold().FontColor(TextLight);
                                c.Item().PaddingTop(4).Text(data.AddressLine).FontSize(9).FontColor(TextMuted);
                                c.Item().Text($"{data.City}, {data.State} - {data.PinCode}").FontSize(9).FontColor(TextMuted);
                            });
                        });

                        // Status/payment info
                        inner.Item().PaddingTop(16).Row(statsRow =>
                        {
                            statsRow.Spacing(10);
                            StatCard(statsRow, "ORDER STATUS", data.OrderStatus, Primary);
                            StatCard(statsRow, "PAYMENT METHOD", data.PaymentMethod, Primary);
                            StatCard(statsRow, "ITEMS", data.Items.Count.ToString(), Primary);
                            StatCard(statsRow, "TOTAL", $"₹{data.FinalAmount:N2}", SuccessColor);
                        });

                        // Line items header
                        inner.Item().PaddingTop(30).Text("Order Items").FontSize(14).Bold().FontColor(PrimaryDark);
                        inner.Item().PaddingTop(6).Height(2).Background(PrimaryLight);

                        inner.Item().PaddingTop(10).Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(3);  // Product
                                columns.RelativeColumn(1.5f); // SKU
                                columns.ConstantColumn(50);  // Qty
                                columns.ConstantColumn(80);  // Unit price
                                columns.ConstantColumn(80);  // Discount
                                columns.ConstantColumn(90);  // Line total
                            });

                            table.Header(header =>
                            {
                                header.Cell().Background(PrimaryLighter).Padding(6).Text("Product").Bold().FontSize(9);
                                header.Cell().Background(PrimaryLighter).Padding(6).Text("SKU").Bold().FontSize(9);
                                header.Cell().Background(PrimaryLighter).Padding(6).AlignRight().Text("Qty").Bold().FontSize(9);
                                header.Cell().Background(PrimaryLighter).Padding(6).AlignRight().Text("Unit Price").Bold().FontSize(9);
                                header.Cell().Background(PrimaryLighter).Padding(6).AlignRight().Text("Discount").Bold().FontSize(9);
                                header.Cell().Background(PrimaryLighter).Padding(6).AlignRight().Text("Total").Bold().FontSize(9);
                            });

                            foreach (var item in data.Items)
                            {
                                table.Cell().BorderBottom(1).BorderColor(BorderLight).Padding(6).Text(item.ProductName).FontSize(9);
                                table.Cell().BorderBottom(1).BorderColor(BorderLight).Padding(6).Text(item.Sku).FontSize(9).FontColor(TextMuted);
                                table.Cell().BorderBottom(1).BorderColor(BorderLight).Padding(6).AlignRight().Text(item.Quantity.ToString()).FontSize(9);
                                table.Cell().BorderBottom(1).BorderColor(BorderLight).Padding(6).AlignRight().Text($"₹{item.UnitPrice:N2}").FontSize(9);
                                table.Cell().BorderBottom(1).BorderColor(BorderLight).Padding(6).AlignRight().Text($"₹{item.Discount:N2}").FontSize(9);
                                table.Cell().BorderBottom(1).BorderColor(BorderLight).Padding(6).AlignRight().Text($"₹{item.LineTotal:N2}").FontSize(9).Bold();
                            }
                        });

                        // Totals summary
                        inner.Item().PaddingTop(20).AlignRight().Width(260).Column(c =>
                        {
                            SummaryRow(c, "Subtotal", data.TotalProductAmount, TextMuted);
                            SummaryRow(c, "Shipping", data.TotalShippingAmount, TextMuted);
                            if (data.TotalCouponAmount > 0)
                                SummaryRow(c, "Coupon Discount", -data.TotalCouponAmount, SuccessColor);
                            if (data.TotalWalletAmount > 0)
                                SummaryRow(c, "Wallet Applied", -data.TotalWalletAmount, SuccessColor);
                            SummaryRow(c, "Tax (18%)", data.Tax, TextMuted);

                            c.Item().PaddingTop(8).Height(1).Background(BorderLight);
                            c.Item().PaddingTop(8).Row(r =>
                            {
                                r.RelativeItem().Text("Total Paid").FontSize(12).Bold().FontColor(PrimaryDark);
                                r.ConstantItem(100).AlignRight().Text($"₹{data.FinalAmount:N2}").FontSize(14).Bold().FontColor(PrimaryDark);
                            });
                        });
                    });
                });

                page.Footer().Padding(20).Row(row =>
                {
                    row.RelativeItem().Text("Thank you for shopping with ShopperApp").FontSize(8).FontColor(TextLight);
                    row.RelativeItem().AlignRight().Text(x =>
                    {
                        x.Span("Page ").FontSize(8).FontColor(TextLight);
                        x.CurrentPageNumber().FontSize(8).FontColor(TextLight);
                        x.Span(" of ").FontSize(8).FontColor(TextLight);
                        x.TotalPages().FontSize(8).FontColor(TextLight);
                    });
                });
            });
        }).GeneratePdf();
    }

    private static void StatCard(RowDescriptor row, string label, string value, string valueColor)
    {
        row.RelativeItem().Border(1).BorderColor(BorderLight).Background(Colors.White)
            .Padding(12).Column(c =>
        {
            c.Item().Text(label).FontSize(7).FontColor(TextLight).Bold();
            c.Item().PaddingTop(4).Text(value).FontSize(13).Bold().FontColor(valueColor);
        });
    }

    private static void SummaryRow(ColumnDescriptor col, string label, decimal value, string color)
    {
        col.Item().PaddingBottom(4).Row(r =>
        {
            r.RelativeItem().Text(label).FontSize(9).FontColor(TextMuted);
            r.ConstantItem(100).AlignRight().Text($"{(value < 0 ? "-" : "")}₹{Math.Abs(value):N2}").FontSize(9).FontColor(color);
        });
    }
}