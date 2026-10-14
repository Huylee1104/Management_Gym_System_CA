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
        productId: productVal || '',
        productName: productVal ? $('#filterProduct option:selected').text().trim() : '',
        staffId: staffVal || '',
        staffName: staffVal ? $('#filterStaff option:selected').text().trim() : '',
        keyword: $('#filterKeyword').val().trim()
    };
}

// 1. Hàm Load danh sách hiển thị lên Table
function loadAdviseData() {
    if (typeof LoadingData === 'function') LoadingData();

    const params = getFilterParams();

    $.post(adviseListApiUrl, params, function (res) {
        const $tbody = $('#adviseTableBody');
        $tbody.empty();

        if (!res || res.length === 0) {
            $tbody.append(`
                <tr id="adviseEmptyRow">
                    <td colspan="10" class="text-center text-muted py-4">Không có khách hàng nào</td>
                </tr>
            `);
            $('#adviseTotalText').text('Tổng: 0 khách hàng');
            return;
        }

        const rows = res.map(item => `
            <tr>
                <td>${formatDate(item.createdDate)}</td>
                <td class="fw-bold">${item.customerName || ''}</td>
                <td>${item.phone || ''}</td>
                <td>${item.email || ''}</td>
                <td>${item.productName || ''}</td>
                <td>${item.lastContactDate ? formatDateTime(item.lastContactDate) : '<span class="text-muted">-</span>'}</td>
                <td class="text-center">${item.contactCount}</td>
                <td class="text-center">
                    ${item.isRegistered
                ? '<span class="badge bg-success-lt">Đã đăng ký</span>'
                : '<span class="badge bg-secondary-lt">Chưa đăng ký</span>'}
                </td>
                <td>${item.staffName || '<span class="text-muted">-</span>'}</td>
                <td class="w-1">
                    <button type="button" class="btn btn-sm btn-icon btn-ghost-primary" onclick="openAdviseModal(${item.id})" title="Cập nhật tư vấn">
                        <i class="ti ti-edit"></i>
                    </button>
                </td>
            </tr>
        `).join('');

        $tbody.html(rows); $('#adviseTotalText').text(`Tổng: ${res.length} khách hàng`);
    })
        .always(function () {
            if (typeof HidenLoadingData === 'function') HidenLoadingData();
        });
}

// 2. Hàm chung để xuất PDF / Excel
function exportFile(baseUrl) {
    const params = $.param(getFilterParams());
    window.location.href = `${baseUrl}?${params}`;
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