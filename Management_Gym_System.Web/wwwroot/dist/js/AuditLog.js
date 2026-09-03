$(document).ready(function () {

    loadStaffs();
    initActionSelect();

    const today = new Date().toISOString().split('T')[0];

    $('#fromDate').val(today);
    $('#toDate').val(today);


    // Tìm kiếm
    $("#btnSearch").on("click", function () {
        loadAuditLogs(1);
    });


    // Nhấn Enter trong ô tìm kiếm
    $("#keyword").on("keydown", function (event) {

        if (event.key === "Enter") {
            loadAuditLogs(1);
        }

    });


    // Có thể tự động tìm lại khi đổi bộ lọc.
    $("#fromDate, #toDate, #staffId, #action").on("change", function () {
        loadAuditLogs(1);
    });


    // Xuất Excel
    $("#btnExcel").on("click", function () {
        exportFile("/AuditLog/ExportExcel");
    });


    // Xuất PDF
    $("#btnPdf").on("click", function () {
        exportFile("/AuditLog/ExportPdf");
    });

});

function loadStaffs() {
    $.get(staffApiUrl, function (res) {
        const staffs = res.filter(u =>
            u.status == true &&
            u.roleID !== null &&
            u.roleID !== 3
        );

        let options = `
            <option value="">--Tất cả--</option>
        `;

        options += staffs.map(u =>
            `<option value="${u.id}">${u.fullName}</option>`
        ).join('');

        $('#staffId').html(options);

        tomSelectStaff = new TomSelect("#staffId", {
            create: false
        });
    });
}

function initActionSelect() {
    tomSelectAction = new TomSelect("#action", {
        create: false,
        allowEmptyOption: true,
        placeholder: "Tất cả",
        maxOptions: null
    });
}

// =========================================================
// LẤY DỮ LIỆU AUDIT LOG
// =========================================================

function loadAuditLogs(page) {

    currentPage = page;

    const filter = getFilter(page);

    $.ajax({
        url: "/AuditLog/GetList",
        type: "GET",
        data: JSON.stringify(filter),
        contentType: "application/json",
        beforeSend: function () {
            showLoading();
        },

        success: function (response) {

            renderTable(response.items);

            renderPagination(
                response.page,
                response.pageSize,
                response.totalItems,
                response.totalPages
            );

        },

        error: function () {

            $("#auditLogBody").empty();
            $("#emptyData")
                .removeClass("d-none")
                .text("Không thể tải dữ liệu.");

        }
    });

}



// =========================================================
// LẤY GIÁ TRỊ BỘ LỌC
// =========================================================

function getFilter(page) {
    return {
        fromDate: $("#fromDate").val() || null,
        toDate: $("#toDate").val() || null,
        staffId: tomSelectStaff.getValue() || null,
        action: tomSelectAction.getValue() || null,
        keyword: $("#keyword").val().trim(),
        page: page,
        pageSize: pageSize
    };
}



// =========================================================
// RENDER BẢNG
// =========================================================

function renderTable(items) {

    const tbody = $("#auditLogBody");

    tbody.empty();


    if (!items || items.length === 0) {

        $("#emptyData").removeClass("d-none");
        return;

    }


    $("#emptyData").addClass("d-none");


    items.forEach(function (item) {

        const date = formatEndDateNormal(item.date);
        const action = getActionBadge(item.action);

        const staffName = escapeHtml(item.staffName || "-");
        const memberName = escapeHtml(item.memberName || "-");
        const note = escapeHtml(item.note || "-");


        const row = `
                    <tr>

                        <td class="audit-date">
                            ${date}
                        </td>

                        <td>
                            ${staffName}
                        </td>

                        <td>
                            ${action}
                        </td>

                        <td>
                            ${memberName}
                        </td>

                        <td class="audit-note">
                            ${note}
                        </td>

                    </tr>
                `;


        tbody.append(row);

    });

}



// =========================================================
// HIỂN THỊ ACTION
// =========================================================

function getActionBadge(action) {

    switch (action) {

        case "Register":
            return `
                        <span class="badge bg-success">
                            Đăng ký mới
                        </span>
                    `;

        case "Extend":
            return `
                        <span class="badge bg-primary">
                            Gia hạn
                        </span>
                    `;

        case "Lock":
            return `
                        <span class="badge bg-danger">
                            Khóa
                        </span>
                    `;

        case "Unlock":
            return `
                        <span class="badge bg-info text-dark">
                            Mở khóa
                        </span>
                    `;

        default:
            return `
                        <span class="badge bg-secondary">
                            ${escapeHtml(action || "-")}
                        </span>
                    `;
    }

}



// =========================================================
// PHÂN TRANG
// =========================================================

function renderPagination(page, pageSize, totalItems, totalPages) {

    const pagination = $("#pagination");

    pagination.empty();


    // ----------------------------
    // Thông tin số bản ghi
    // ----------------------------

    if (totalItems === 0) {

        $("#paginationInfo").text("Hiển thị 0 - 0 / 0 bản ghi");

    } else {

        const from = ((page - 1) * pageSize) + 1;
        const to = Math.min(page * pageSize, totalItems);

        $("#paginationInfo")
            .text(`Hiển thị ${from} - ${to} / ${totalItems} bản ghi`);

    }


    if (totalPages <= 1) {
        return;
    }


    // ----------------------------
    // Nút Previous
    // ----------------------------

    pagination.append(`
                <li class="page-item ${page === 1 ? "disabled" : ""}">
                    <button class="page-link"
                            type="button"
                            data-page="${page - 1}">
                        <i class="ti ti-chevron-left"></i>
                    </button>
                </li>
            `);


    /*
        Chỉ hiện tối đa khoảng 5 trang quanh trang hiện tại.
        Tránh trường hợp có hàng trăm page làm pagination quá dài.
    */

    let startPage = Math.max(1, page - 2);
    let endPage = Math.min(totalPages, page + 2);


    if (page <= 3) {
        endPage = Math.min(5, totalPages);
    }


    if (page >= totalPages - 2) {
        startPage = Math.max(1, totalPages - 4);
    }


    for (let i = startPage; i <= endPage; i++) {

        pagination.append(`
                    <li class="page-item ${i === page ? "active" : ""}">
                        <button class="page-link"
                                type="button"
                                data-page="${i}">
                            ${i}
                        </button>
                    </li>
                `);

    }


    // ----------------------------
    // Nút Next
    // ----------------------------

    pagination.append(`
                <li class="page-item ${page === totalPages ? "disabled" : ""}">
                    <button class="page-link"
                            type="button"
                            data-page="${page + 1}">
                        <i class="ti ti-chevron-right"></i>
                    </button>
                </li>
            `);


    // Event chuyển trang
    $("#pagination .page-link").on("click", function () {

        const parent = $(this).closest(".page-item");

        if (parent.hasClass("disabled") ||
            parent.hasClass("active")) {
            return;
        }


        const selectedPage = Number($(this).data("page"));

        loadAuditLogs(selectedPage);

    });

}



// =========================================================
// LOADING
// =========================================================

function showLoading() {

    $("#emptyData").addClass("d-none");

    $("#auditLogBody").html(`
                <tr>
                    <td colspan="5"
                        class="text-center py-5 text-muted">

                        <div class="spinner-border spinner-border-sm
                                    text-primary me-2"
                             role="status">
                        </div>

                        Đang tải dữ liệu...

                    </td>
                </tr>
            `);

}



// =========================================================
// FORMAT DATETIME
// =========================================================

function formatDateTime(value) {

    if (!value) {
        return "-";
    }


    const date = new Date(value);


    if (isNaN(date.getTime())) {
        return value;
    }


    return date.toLocaleString("vi-VN", {
        day: "2-digit",
        month: "2-digit",
        year: "numeric",
        hour: "2-digit",
        minute: "2-digit"
    });

}



// =========================================================
// EXPORT EXCEL / PDF
// =========================================================

function exportFile(url) {

    /*
        Export dùng chính bộ lọc hiện tại.

        Không truyền page/pageSize vì thông thường export
        sẽ xuất toàn bộ dữ liệu thỏa điều kiện.
    */

    const params = new URLSearchParams();

    params.append("fromDate", $("#fromDate").val());
    params.append("toDate", $("#toDate").val());
    params.append("staffId", $("#staffId").val());
    params.append("action", $("#action").val());
    params.append("keyword", $("#keyword").val().trim());


    window.location.href = url + "?" + params.toString();

}



// =========================================================
// ESCAPE HTML
// Tránh dữ liệu từ DB chứa HTML/script rồi render trực tiếp.
// =========================================================

function escapeHtml(value) {

    return $("<div>")
        .text(value)
        .html();

}