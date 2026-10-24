const planApiUrl = '/Product/listProducts';
const staffApiUrl = '/User/listStaffs';
const adviseListApiUrl = '/MemberAdvise/GetList';
const exportPdfApiUrl = '/MemberAdvise/ExportPdf';
const exportExcelApiUrl = '/MemberAdvise/ExportExcel';

let tomSelectStaff = null;
let tomSelectProduct = null;
let tomSelectAction = null;

$(document).ready(function () {
    loadStaffs();
    loadPlans();
    initActionSelect();

    loadAdviseData();

    // Sự kiện 3 nút
    $('#btnLoadTuVan').on('click', function () {
        loadAdviseData();
    });

    $('#btnExportPdfTuVan').on('click', function () {
        exportFile(exportPdfApiUrl);
    });

    $('#btnExportExcelTuVan').on('click', function () {
        exportFile(exportExcelApiUrl);
    });
});

function loadStaffs() {
    if (typeof LoadingData === 'function') LoadingData();

    $.get(staffApiUrl, function (res) {
        const staffs = (res || []).filter(u =>
            u.status === true &&
            u.roleID !== null &&
            u.userType !== 1
        );

        let options = '<option value="">-- Tất cả --</option>';
        options += staffs.map(u => `<option value="${u.id}">${u.fullName}</option>`).join('');

        // 1. Destroy instance cũ trước khi gán HTML mới
        if (tomSelectStaff) {
            tomSelectStaff.destroy();
            tomSelectStaff = null;
        }

        // 2. Gán HTML vào selector chuẩn #filterStaff
        $('#filterStaff').html(options);

        // 3. Khởi tạo lại TomSelect
        tomSelectStaff = new TomSelect("#filterStaff", {
            create: false,
            allowEmptyOption: true,
            placeholder: "-- Chọn nhân viên --"
        });
    })
        .always(function () {
            if (typeof HidenLoadingData === 'function') HidenLoadingData();
        });
}

function loadPlans() {
    $.get(planApiUrl, function (res) {
        const filtered = (res || []).filter(p => p.thoiHan != null);

        let options = '<option value="">-- Tất cả --</option>';
        options += filtered.map(p => `
            <option value="${p.id}" data-thoihan="${p.thoiHan}">
                ${p.productName}
            </option>
        `).join('');

        // 1. Destroy instance cũ trước khi gán HTML mới
        if (tomSelectProduct) {
            tomSelectProduct.destroy();
            tomSelectProduct = null;
        }

        // 2. Gán HTML vào selector chuẩn #filterProduct
        $('#filterProduct').html(options);

        // 3. Khởi tạo lại TomSelect
        tomSelectProduct = new TomSelect("#filterProduct", {
            create: false,
            allowEmptyOption: true,
            placeholder: "-- Chọn gói tư vấn --"
        });
    });
}

function initActionSelect() {
    const $action = $('#action');
    if ($action.length === 0) return;

    if (tomSelectAction) {
        tomSelectAction.destroy();
        tomSelectAction = null;
    }

    tomSelectAction = new TomSelect("#action", {
        create: false,
        allowEmptyOption: true,
        placeholder: "Tất cả",
        maxOptions: null
    });
}

function getFilterParams() {
    const productVal = $('#filterProduct').val();
    const staffVal = $('#filterStaff').val();

    return {
        fromDate: $('#filterFromDate').val(),
        toDate: $('#filterToDate').val(),
        status: $('#filterStatus').val(),
        productId: productVal || null,
        productName: productVal ? $('#filterProduct option:selected').text().trim() : '',
        staffId: staffVal || null,
        staffName: staffVal ? $('#filterStaff option:selected').text().trim() : '',
        keyword: $('#filterKeyword').val().trim()
    };
}

// 1. Hàm Load danh sách hiển thị lên Table
function loadAdviseData() {
    if (typeof LoadingData === "function") {
        LoadingData();
    }

    var filter = getFilterParams();

    $.ajax({
        url: adviseListApiUrl,
        type: "POST",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify(filter),
        success: function (res) {

            var tbody = $("#adviseTableBody");
            tbody.empty();

            if (res == null || res.length === 0) {

                tbody.append(
                    '<tr id="adviseEmptyRow">' +
                    '<td colspan="10" class="text-center text-muted py-4">' +
                    'Không có khách hàng nào' +
                    '</td>' +
                    '</tr>'
                );

                $("#adviseTotalText").text("Tổng: 0 khách hàng");
                return;
            }

            var html = "";

            for (var i = 0; i < res.length; i++) {

                var item = res[i];

                var registerStatus = "";

                if (item.isRegistered) {
                    registerStatus = '<span class="badge bg-success-lt">Đã đăng ký</span>';
                }
                else {
                    registerStatus = '<span class="badge bg-secondary-lt">Chưa đăng ký</span>';
                }

                var lastContactDate = '<span class="text-muted">-</span>';

                if (item.lastContactDate != null) {
                    lastContactDate = formatDateTime(item.lastContactDate);
                }

                var staffName = item.staffName;

                if (!staffName) {
                    staffName = '<span class="text-muted">-</span>';
                }

                html +=
                    '<tr>' +
                        '<td class="text-center">' + formatDate(item.createdDate) + '</td>' +
                        '<td class="fw-bold text-start">' + (item.customerName || "") + '</td>' +
                        '<td class="text-center">' + (item.phone || "") + '</td>' +
                        '<td class="text-start">' + (item.email || "") + '</td>' +
                        '<td class="text-start">' + (item.productName || "") + '</td>' +
                        '<td class="text-center">' + lastContactDate + '</td>' +
                        '<td class="text-center">' + item.contactCount + '</td>' +
                        '<td class="text-center">' + registerStatus + '</td>' +
                        '<td class="text-start">' + staffName + '</td>' +
                        '<td class="text-center">' +
                            '<button type="button" class="btn btn-sm btn-primary me-1" ' +
                                'onclick="ContactMember(' + item.id + ', \'' + (item.phone || "") + '\', \'' + (item.email || "") + '\')" ' +
                                'title="Liên hệ khách hàng">' +
                                '<i class="ti ti-phone"></i>' +
                            '</button>' +
                            // '<button type="button" class="btn btn-sm btn-success" ' +
                            //     'onclick="MemberRegistered(' + item.id + ')" ' +
                            //     'title="Đánh dấu đã đăng ký">' +
                            //     '<i class="ti ti-check"></i>' +
                            // '</button>' +
                        '</td>' +
                    '</tr>';
            }

            tbody.html(html);

            $("#adviseTotalText").text("Tổng: " + res.length + " khách hàng");
        },
        error: function (xhr, status, error) {
            console.error("Lỗi khi tải danh sách tư vấn:");
            console.error(xhr);
            console.error(status);
            console.error(error);

            showToast("Không thể tải danh sách khách hàng.", 500);
        },
        complete: function () {
            if (typeof HidenLoadingData === "function") {
                HidenLoadingData();
            }
        }
    });
}

// 2. Hàm chung để xuất PDF / Excel
function exportFile(url, extension) {

    var filter = getFilterParams();

    var fromDate = filter.fromDate ? filter.fromDate.replaceAll("-", "") : null;
    var toDate = filter.toDate ? filter.toDate.replaceAll("-", "") : null;

    if (!fromDate || !toDate){
        showToast("Vui lòng chọn đủ từ ngày đến ngày");
        return;
    }

    var fileName = "DanhSachKhachHang_" + fromDate + "_" + toDate + "." + extension;

    $.ajax({
        url: url,
        type: "POST",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify(filter),
        xhrFields: {
            responseType: "blob"
        },
        success: function (blob) {

            var downloadUrl = URL.createObjectURL(blob);

            var a = document.createElement("a");
            a.href = downloadUrl;
            a.download = fileName;
            a.click();

            URL.revokeObjectURL(downloadUrl);
        },
        error: function (xhr) {
            console.error(xhr);
            showToast("Không thể xuất file.", 500);
        }
    });
}
// Helper định dạng ngày DD/MM/YYYY
function formatDate(dateStr) {
    if (!dateStr) return '';
    const d = new Date(dateStr);
    return d.toLocaleDateString('vi-VN');
}

function formatDateTime(dateStr) {
    if (!dateStr) return '';
    const d = new Date(dateStr);
    return d.toLocaleDateString('vi-VN') + ' ' + d.toLocaleTimeString('vi-VN', { hour: '2-digit', minute: '2-digit' });
}

function ContactMember(id, phone, email){
    if (!id){
        showToast("Không xác định được khách hàng!", 500);
        return;
    }
    $.ajax({
        url: '/MemberAdvise/UpdateMemberAdvise',
        type: "POST",
        data: {
            Id: id,
            PhoneNumber: phone,
            Email: email
        },
        success: function (response) {
            if (response.isSuccess == true){
                loadAdviseData();
            }
            else{
                showToast(response.message, 500);
            }
        },
        error: function (xhr) {
            console.error(xhr);
            showToast("Không thể cập nhật trạng thái.", 500);
        }
    });
}