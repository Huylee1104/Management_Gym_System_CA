using Management_Gym_System.Application.Services;
using Management_Gym_System.Domain.Entities;
using Management_Gym_System.Domain.Interfaces;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

public class AuditLogService : IAuditLogService
{
    private readonly IMembershipAuditLogRepository _auditLogRepo;


    public AuditLogService(IMembershipAuditLogRepository auditLogRepo)
    {
        _auditLogRepo = auditLogRepo;
    }

    public async Task<ActivityLogResponse> GetActivityLogsAsync(ActivityLogRequest request)
    {
        var listLog = await _auditLogRepo.GetListAsync(request.FromDate, request.ToDate, request.StaffId);

        if (!string.IsNullOrWhiteSpace(request.Action))
            listLog = listLog.Where(x => x.Action == request.Action).ToList();

        if (!string.IsNullOrWhiteSpace(request.Keyword))
        {
            var keyword = request.Keyword.Trim();
            listLog = listLog.Where(x => x.Member is { } member && member.FullName?.Contains(keyword, StringComparison.OrdinalIgnoreCase) == true).ToList();
        }

        var totalItems = listLog.Count();
        var totalPages = (int)Math.Ceiling(totalItems / (double)request.PageSize);

        var items = listLog.OrderByDescending(x => x.Date)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new ActivityLogDto
            {
                Id = x.Id,
                Date = x.Date,
                StaffName = x.Staff?.FullName,
                Action = x.Action,
                MemberName = x.Member?.FullName,
                Note = x.Note,
                DataEdited = x.DataEdited
            }).ToList();

        return new ActivityLogResponse
        {
            Items = items,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalItems = totalItems,
            TotalPages = totalPages
        };
    }

    public async Task<byte[]> GetExportExcelLogsAsync(ActivityLogRequest request)
    {
        var response = await GetActivityLogsAsync(request);
        var data = response.Items;

        using var package = new ExcelPackage();

        var worksheet = package.Workbook.Worksheets.Add("Lịch sử hoạt động");

        worksheet.Cells["A1:E1"].Merge = true;
        worksheet.Cells["A1"].Value = "BÁO CÁO LỊCH SỬ HOẠT ĐỘNG";

        worksheet.Cells["A1"].Style.Font.Bold = true;
        worksheet.Cells["A1"].Style.Font.Size = 18;
        worksheet.Cells["A1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        worksheet.Cells["A1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

        worksheet.Row(1).Height = 30;

        worksheet.Cells["A2:E2"].Merge = true;

        var fromDate = request.FromDate?.ToString("dd/MM/yyyy") ?? "";
        var toDate = request.ToDate?.ToString("dd/MM/yyyy") ?? "";

        worksheet.Cells["A2"].Value = $"Từ ngày {fromDate} đến ngày {toDate}";

        worksheet.Cells["A2"].Style.Font.Size = 11;
        worksheet.Cells["A2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

        var headerRow = 4;

        worksheet.Cells[headerRow, 1].Value = "Ngày";
        worksheet.Cells[headerRow, 2].Value = "Nhân viên";
        worksheet.Cells[headerRow, 3].Value = "Hành động";
        worksheet.Cells[headerRow, 4].Value = "Hội viên";
        worksheet.Cells[headerRow, 5].Value = "Ghi chú";

        using (var header = worksheet.Cells[headerRow, 1, headerRow, 5])
        {
            header.Style.Font.Bold = true;
            header.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            header.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            header.Style.WrapText = true;
        }

        worksheet.Row(headerRow).Height = 25;

        var currentRow = headerRow + 1;

        foreach (var item in data)
        {
            worksheet.Cells[currentRow, 1].Value = item.Date;
            worksheet.Cells[currentRow, 1].Style.Numberformat.Format = "dd/MM/yyyy HH:mm";

            worksheet.Cells[currentRow, 2].Value = item.StaffName ?? "";
            worksheet.Cells[currentRow, 3].Value = item.Action;
            worksheet.Cells[currentRow, 4].Value = item.MemberName ?? "";
            worksheet.Cells[currentRow, 5].Value = item.Note ?? "";

            currentRow++;
        }

        if (data.Any())
        {
            var tableRange = worksheet.Cells[
                headerRow,
                1,
                currentRow - 1,
                5
            ];

            tableRange.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            tableRange.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            tableRange.Style.Border.Left.Style = ExcelBorderStyle.Thin;
            tableRange.Style.Border.Right.Style = ExcelBorderStyle.Thin;

            tableRange.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            tableRange.Style.WrapText = true;
        }

        worksheet.Column(1).Width = 20;
        worksheet.Column(2).Width = 25;
        worksheet.Column(3).Width = 25;
        worksheet.Column(4).Width = 25;
        worksheet.Column(5).Width = 50;

        worksheet.View.FreezePanes(headerRow + 1, 1);

        worksheet.Cells[
            headerRow,
            1,
            Math.Max(currentRow - 1, headerRow),
            5
        ].AutoFilter = true;

        worksheet.PrinterSettings.Orientation =
            eOrientation.Landscape;

        worksheet.PrinterSettings.PaperSize =
            ePaperSize.A4;

        worksheet.PrinterSettings.FitToPage = true;
        worksheet.PrinterSettings.FitToWidth = 1;
        worksheet.PrinterSettings.FitToHeight = 0;

        worksheet.PrinterSettings.HorizontalCentered = true;

        worksheet.HeaderFooter.OddFooter.CenteredText =
            "Trang &P / &N";

        return await package.GetAsByteArrayAsync();
    }

    public async Task<byte[]> GetExportPdfLogsAsync(ActivityLogRequest request)
    {
        var response = await GetActivityLogsAsync(request);
        var data = response.Items;

        var fromDate = request.FromDate?.ToString("dd/MM/yyyy") ?? "";
        var toDate = request.ToDate?.ToString("dd/MM/yyyy") ?? "";

        QuestPDF.Settings.License = LicenseType.Community;

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);

                page.MarginHorizontal(30);
                page.MarginVertical(25);

                page.Header()
                    .Column(column =>
                    {
                        column.Item()
                            .AlignCenter()
                            .Text("BÁO CÁO LỊCH SỬ HOẠT ĐỘNG")
                            .Bold()
                            .FontSize(18);

                        column.Item()
                            .PaddingTop(5)
                            .AlignCenter()
                            .Text($"Từ ngày {fromDate} đến ngày {toDate}")
                            .FontSize(10);
                    });

                page.Content()
                    .PaddingTop(20)
                    .Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(1.3f); // Ngày
                            columns.RelativeColumn(1.5f); // Nhân viên
                            columns.RelativeColumn(1.5f); // Hành động
                            columns.RelativeColumn(1.5f); // Hội viên
                            columns.RelativeColumn(3f);   // Ghi chú
                        });

                        // =========================
                        // TABLE HEADER
                        // =========================

                        table.Header(header =>
                        {
                            header.Cell().Element(HeaderStyle).Text("Ngày");
                            header.Cell().Element(HeaderStyle).Text("Nhân viên");
                            header.Cell().Element(HeaderStyle).Text("Hành động");
                            header.Cell().Element(HeaderStyle).Text("Hội viên");
                            header.Cell().Element(HeaderStyle).Text("Ghi chú");
                        });

                        // =========================
                        // DATA
                        // =========================

                        foreach (var item in data)
                        {
                            table.Cell()
                                .Element(CellStyle)
                                .Text(item.Date.ToString("dd/MM/yyyy HH:mm"));

                            table.Cell()
                                .Element(CellStyle)
                                .Text(item.StaffName ?? "");

                            table.Cell()
                                .Element(CellStyle)
                                .Text(item.Action);

                            table.Cell()
                                .Element(CellStyle)
                                .Text(item.MemberName ?? "");

                            table.Cell()
                                .Element(CellStyle)
                                .Text(item.Note ?? "");
                        }
                    });

                page.Footer()
                    .AlignCenter()
                    .Text(text =>
                    {
                        text.Span("Trang ");
                        text.CurrentPageNumber();
                        text.Span(" / ");
                        text.TotalPages();
                    });
            });
        });

        return document.GeneratePdf();

        static IContainer HeaderStyle(IContainer container)
        {
            return container
                .Background(Colors.Grey.Lighten2)
                .Border(1)
                .BorderColor(Colors.Grey.Medium)
                .Padding(5)
                .AlignCenter()
                .AlignMiddle()
                .DefaultTextStyle(x =>
                    x.Bold()
                     .FontSize(9));
        }

        static IContainer CellStyle(IContainer container)
        {
            return container
                .Border(1)
                .BorderColor(Colors.Grey.Lighten1)
                .Padding(5)
                .AlignMiddle()
                .DefaultTextStyle(x =>
                    x.FontSize(8));
        }
    }
}