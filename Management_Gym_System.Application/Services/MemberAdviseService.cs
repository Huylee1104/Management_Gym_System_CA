using System.Globalization;
using System.Security.Claims;
using Management_Gym_System.Application.DTOs.MemberAdvise;
using Management_Gym_System.Application.Services;
using Management_Gym_System.Domain.Entities;
using Management_Gym_System.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

public class MemberAdviseService : IMemberAdviseService
{
    private readonly IMemberAdviseRepository _memberAdviseRepo;
    private readonly IProductRepository _productRepo;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public MemberAdviseService(IMemberAdviseRepository memberAdviseRepo, IProductRepository productRepo, IHttpContextAccessor httpContextAccessor)
    {
        _memberAdviseRepo = memberAdviseRepo;
        _productRepo = productRepo;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<List<AdviseCustomerDto>> GetListMemberAdviseAsync(AdviseFilterRequest filter)
    {
        var memberAdvises = await _memberAdviseRepo.GetListAdviseAsync(filter.FromDate, filter.ToDate);

        var query = memberAdvises.AsEnumerable();

        if (filter.ProductId.HasValue)
        {
            query = query.Where(x => x.ProductId == filter.ProductId.Value);
        }

        if (filter.StaffId.HasValue)
        {
            query = query.Where(x => x.StaffId == filter.StaffId.Value);
        }

        if (!string.IsNullOrWhiteSpace(filter.Status))
        {
            query = filter.Status switch
            {
                "NotContacted" => query.Where(x => x.IsContacted != true),
                "Contacted" => query.Where(x => x.IsContacted == true),
                _ => query
            };
        }

        if (!string.IsNullOrWhiteSpace(filter.Keyword))
        {
            var compareInfo = CultureInfo.GetCultureInfo("vi-VN").CompareInfo;
            var keyword = filter.Keyword.Trim();

            query = query.Where(x =>
                !string.IsNullOrWhiteSpace(x.FullName) &&
                compareInfo.IndexOf(
                    x.FullName,
                    keyword,
                    CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace
                ) >= 0
            );
        }

        return query
            .OrderByDescending(x => x.Date)
            .Select(x => new AdviseCustomerDto
            {
                Id = x.Id,
                CreatedDate = x.Date,
                CustomerName = x.FullName ?? "",
                Phone = x.PhoneNumber ?? "",
                Email = x.Email ?? "",
                ProductName = x.Product?.ProductName ?? "",
                LastContactDate = x.ContactedDate,
                ContactCount = x.ContactedCount ?? 0,
                IsRegistered = false,
                StaffName = x.Staff?.FullName ?? ""
            })
            .ToList();
    }

    public async Task<ServiceResult> UpdateMemberAdviseAsync(long IdMember)
    {
        try
        {
            if (IdMember > 0)
            {
                var existingMemberAdvise = await _memberAdviseRepo.GetByIdAsync(IdMember);

                if (existingMemberAdvise == null)
                {
                    return ServiceResult.Failure("Không tìm thấy thông tin tư vấn.");
                }

                var value = _httpContextAccessor.HttpContext?
                    .User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (!long.TryParse(value, out var userId))
                    return ServiceResult.Failure("Không xác định được nhân viên.");
            
                existingMemberAdvise.IsContacted = true;
                existingMemberAdvise.ContactedDate = DateTime.UtcNow;
                existingMemberAdvise.ContactedCount = existingMemberAdvise.ContactedCount += 1;
                existingMemberAdvise.StaffId = userId;

                await _memberAdviseRepo.UpdateAsync(existingMemberAdvise);
            }
            await _memberAdviseRepo.SaveChangesAsync();
            return ServiceResult.Success("Thành công!");
        }
        catch (Exception ex)
        {
            return ServiceResult.Failure($"Cập nhật thất bại: {ex.Message}");
        }
    }

    public async Task<ServiceResult> AddMemberAdviseAsync(RequestMemberAdvise requestMemberAdvise)
    {
        try
        {
            var newMemberAdvise = new MemberAdvise
            {
                ProductId = requestMemberAdvise.ProductId,
                Date = DateTime.Now,
                FullName = requestMemberAdvise.FullName,
                PhoneNumber = requestMemberAdvise.PhoneNumber,
                Email = requestMemberAdvise.Email,
                IsContacted = false,
                ContactedDate = null,
                ContactedCount = 0
            };

            await _memberAdviseRepo.AddAsync(newMemberAdvise);
            await _memberAdviseRepo.SaveChangesAsync();

            return ServiceResult.Success("Thêm thông tin tư vấn thành công!");
        }
        catch (Exception ex)
        {
            return ServiceResult.Failure($"Thêm thông tin tư vấn thất bại: {ex.Message}");
        }
    }

    public async Task<ServiceResult> ReviewProductAsync(long ProductId, string Review)
    {
        var product = await _productRepo.GetByIdAsync(ProductId);
        if (product == null)
        {
            return ServiceResult.Failure("Không tìm thấy sản phẩm.");
        }

        product.Review = product.Review + Review;
        await _productRepo.UpdateAsync(product);
        await _productRepo.SaveChangesAsync();
        return ServiceResult.Success("Đánh giá sản phẩm thành công!");
    }

    public async Task<byte[]> ExportPdfAsync(AdviseFilterRequest? filter)
    {
        filter ??= new AdviseFilterRequest();

        List<AdviseCustomerDto> data = await GetListMemberAdviseAsync(filter);

        string statusText = GetStatusDisplayName(filter.Status);
        string fromDateStr = filter.FromDate?.ToString("dd/MM/yyyy") ?? "...";
        string toDateStr = filter.ToDate?.ToString("dd/MM/yyyy") ?? "...";
        string dateRangeText = $"Từ ngày: {fromDateStr} - Đến ngày: {toDateStr}";

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(1, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                page.Header().PaddingBottom(10).Column(col =>
                {
                    col.Item().AlignCenter().Text("DANH SÁCH KHÁCH HÀNG CẦN TƯ VẤN")
                        .FontSize(16).Bold().FontColor(Colors.Blue.Darken2);

                    col.Item().PaddingTop(4).AlignCenter().Text(dateRangeText).FontSize(11).Italic();

                    var filterParts = new List<string> { $"Trạng thái: {statusText}" };
                    if (filter.ProductId.HasValue && !string.IsNullOrWhiteSpace(filter.ProductName))
                        filterParts.Add($"Gói tư vấn: {filter.ProductName}");
                    if (filter.StaffId.HasValue && !string.IsNullOrWhiteSpace(filter.StaffName))
                        filterParts.Add($"Nhân viên tư vấn: {filter.StaffName}");

                    col.Item().PaddingTop(4).AlignCenter().Text(string.Join("   |   ", filterParts))
                        .FontSize(10).SemiBold();
                });

                page.Content().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(30);
                        columns.ConstantColumn(65);
                        columns.RelativeColumn(2f);
                        columns.ConstantColumn(80);
                        columns.RelativeColumn(2f);
                        columns.RelativeColumn(1.8f);
                        columns.ConstantColumn(75);
                        columns.ConstantColumn(55);
                        columns.ConstantColumn(65);
                        columns.RelativeColumn(1.5f);
                    });

                    table.Header(header =>
                    {
                        string[] headers = { "STT", "Ngày", "Tên khách hàng", "Số điện thoại", "Email", "Gói tư vấn", "Liên lạc gần nhất", "Số lần LL", "Đã ĐK gói", "Người liên lạc" };
                        foreach (var h in headers)
                        {
                            header.Cell().Background(Colors.Grey.Lighten3).Border(0.5f).BorderColor(Colors.Grey.Medium)
                                .Padding(5).AlignCenter().AlignMiddle().Text(h).Bold().FontSize(9);
                        }
                    });

                    int stt = 1;
                    foreach (var item in data)
                    {
                        table.Cell().Element(CellStyle).AlignCenter().Text(stt++.ToString());
                        table.Cell().Element(CellStyle).AlignCenter().Text(item.CreatedDate?.ToString("dd/MM/yyyy"));
                        table.Cell().Element(CellStyle).Text(item.CustomerName ?? "");
                        table.Cell().Element(CellStyle).AlignCenter().Text(item.Phone ?? "");
                        table.Cell().Element(CellStyle).Text(item.Email ?? "");
                        table.Cell().Element(CellStyle).Text(item.ProductName ?? "");
                        table.Cell().Element(CellStyle).AlignCenter().Text(item.LastContactDate?.ToString("dd/MM/yyyy") ?? "-");
                        table.Cell().Element(CellStyle).AlignCenter().Text(item.ContactCount.ToString());
                        table.Cell().Element(CellStyle).AlignCenter().Text(item.IsRegistered ? "Đã ĐK" : "Chưa");
                        table.Cell().Element(CellStyle).Text(item.StaffName ?? "-");
                    }

                    static IContainer CellStyle(IContainer container)
                    {
                        return container.Border(0.5f).BorderColor(Colors.Grey.Lighten1).Padding(4).AlignMiddle();
                    }
                });

                page.Footer().PaddingTop(5).Row(row =>
                {
                    row.RelativeItem().Text($"Tổng số: {data.Count} khách hàng").FontSize(9).Italic();
                    row.RelativeItem().AlignRight().Text(x =>
                    {
                        x.Span("Trang ").FontSize(9);
                        x.CurrentPageNumber().FontSize(9);
                        x.Span(" / ").FontSize(9);
                        x.TotalPages().FontSize(9);
                    });
                });
            });
        });

        return document.GeneratePdf();
    }

    public async Task<byte[]> ExportExcelAsync(AdviseFilterRequest? filter)
    {
        filter ??= new AdviseFilterRequest();

        List<AdviseCustomerDto> data = await GetListMemberAdviseAsync(filter);

        string statusText = GetStatusDisplayName(filter.Status);
        string fromDateStr = filter.FromDate?.ToString("dd/MM/yyyy") ?? "...";
        string toDateStr = filter.ToDate?.ToString("dd/MM/yyyy") ?? "...";

        using var package = new ExcelPackage();
        var ws = package.Workbook.Worksheets.Add("KhachHangCanTuVan");

        ws.Cells["A1:J1"].Merge = true;
        ws.Cells["A1"].Value = "DANH SÁCH KHÁCH HÀNG CẦN TƯ VẤN";
        ws.Cells["A1"].Style.Font.Size = 16;
        ws.Cells["A1"].Style.Font.Bold = true;
        ws.Cells["A1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

        ws.Cells["A2:J2"].Merge = true;
        ws.Cells["A2"].Value = $"Từ ngày: {fromDateStr} - Đến ngày: {toDateStr}";
        ws.Cells["A2"].Style.Font.Italic = true;
        ws.Cells["A2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

        var filterParts = new List<string> { $"Trạng thái: {statusText}" };
        if (filter.ProductId.HasValue && !string.IsNullOrWhiteSpace(filter.ProductName))
            filterParts.Add($"Gói tư vấn: {filter.ProductName}");
        if (filter.StaffId.HasValue && !string.IsNullOrWhiteSpace(filter.StaffName))
            filterParts.Add($"Nhân viên tư vấn: {filter.StaffName}");

        ws.Cells["A3:J3"].Merge = true;
        ws.Cells["A3"].Value = string.Join("   |   ", filterParts);
        ws.Cells["A3"].Style.Font.Bold = true;
        ws.Cells["A3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

        string[] headers = { "STT", "Ngày", "Tên khách hàng", "Số điện thoại", "Email", "Gói tư vấn", "Ngày liên lạc gần nhất", "Số lần liên lạc", "Đã đăng ký gói", "Người liên lạc" };
        for (int i = 0; i < headers.Length; i++)
        {
            var cell = ws.Cells[5, i + 1];
            cell.Value = headers[i];
            cell.Style.Font.Bold = true;
            cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
            cell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(230, 230, 230));
            cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            cell.Style.Border.BorderAround(ExcelBorderStyle.Thin);
        }

        int rowIdx = 6;
        int stt = 1;
        foreach (var item in data)
        {
            ws.Cells[rowIdx, 1].Value = stt++;
            ws.Cells[rowIdx, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            ws.Cells[rowIdx, 2].Value = item.CreatedDate?.ToString("dd/MM/yyyy");
            ws.Cells[rowIdx, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            ws.Cells[rowIdx, 3].Value = item.CustomerName ?? "";
            ws.Cells[rowIdx, 4].Value = item.Phone ?? "";
            ws.Cells[rowIdx, 5].Value = item.Email ?? "";
            ws.Cells[rowIdx, 6].Value = item.ProductName ?? "";

            ws.Cells[rowIdx, 7].Value = item.LastContactDate?.ToString("dd/MM/yyyy HH:mm") ?? "-";
            ws.Cells[rowIdx, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            ws.Cells[rowIdx, 8].Value = item.ContactCount;
            ws.Cells[rowIdx, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            ws.Cells[rowIdx, 9].Value = item.IsRegistered ? "Đã đăng ký" : "Chưa đăng ký";
            ws.Cells[rowIdx, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            ws.Cells[rowIdx, 10].Value = item.StaffName ?? "-";

            using (var range = ws.Cells[rowIdx, 1, rowIdx, 10])
            {
                range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
            }
            rowIdx++;
        }

        if (ws.Dimension != null)
        {
            ws.Cells[ws.Dimension.Address].AutoFitColumns();
        }

        return package.GetAsByteArray();
    }

    private string GetStatusDisplayName(string? status)
    {
        return status switch
        {
            "NotContacted" => "Chưa liên lạc",
            "Contacted" => "Đã liên lạc",
            "Registered" => "Đã đăng ký",
            _ => "Tất cả"
        };
    }
}