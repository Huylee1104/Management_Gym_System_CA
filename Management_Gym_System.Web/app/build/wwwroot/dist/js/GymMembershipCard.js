$(document).ready(function () {
    // 1. Khởi tạo Tom-Select cho bộ lọc
    new TomSelect("#selectFilter", {
        create: false,
        sortField: { field: "text", direction: "asc" }
    });

    // 2. Tải dữ liệu lần đầu
    loadCards();

    // 3. Lắng nghe sự kiện thay đổi bộ lọc
    $("#selectFilter").on("change", function () {
        loadCards();
    });

    $('#txtSearchCard').on('keydown', function (e) {

        if (e.keyCode == 13) {
            loadCards();
        }

    });
});

// ================= HÀM XỬ LÝ CHUNG =================

// Hàm gọi Ajax tải dữ liệu đổ vào bảng
function loadCards() {
    let filterValue = $("#selectFilter").val();
    let keyword = $("#txtSearchCard").val().trim();

    $.ajax({
        url: '/GymMembershipCard/GetCards',
        type: 'GET',
        data: { filter: filterValue, keyword: keyword },
        success: function (response) {
            if (response.success) {
                renderTable(response.data);
            } else {
                showToast("Có lỗi xảy ra khi tải dữ liệu!", 500);
            }
        },
        error: function () {
            showToast("Lỗi kết nối đến máy chủ.", 500);
        }
    });
}

// Hàm render HTML cho bảng
function renderTable(data) {
    let tbody = $("#tableBody");
    tbody.empty();

    if (data.length === 0) {
        tbody.append('<tr><td colspan="6" class="text-center">Không tìm thấy thẻ nào.</td></tr>');
        return;
    }

    $.each(data, function (index, item) {
        // Xử lý hiển thị trạng thái và nút Khóa/Mở
        let rfidDisplay = item.rfiD_UID ? `<span class="badge bg-green-lt">${item.rfiD_UID}</span>` : `<span class="badge bg-secondary-lt">Chưa đăng ký</span>`;
        
        let statusBadge = "";
        let toggleBtnClass = "";
        let toggleIcon = "";
        let toggleText = "";

        // Status = true là Đang dùng (xanh), Status = false là Khóa (đỏ), null là thẻ trống
        if (item.status === true) {
            statusBadge = `<span class="badge bg-green">Đang dùng</span>`;
            toggleBtnClass = "btn-outline-success"; // Màu xanh lam để khóa thẻ
            toggleIcon = "ti-lock";
            toggleText = "Khóa thẻ";
        } else if (item.status === false) {
            statusBadge = `<span class="badge bg-red">Đã khóa</span>`;
            toggleBtnClass = "btn-outline-danger"; // Màu đỏ là thẻ đang khóa, bấm để mở
            toggleIcon = "ti-lock-open";
            toggleText = "Mở khóa";
        } else {
            statusBadge = `<span class="badge bg-secondary">Thẻ trống</span>`;
            toggleBtnClass = "btn-outline-primary"; // Màu xanh lá để kích hoạt thẻ trống
            toggleIcon = "ti-lock";
            toggleText = "Khóa thẻ";
        }

        let row = `
            <tr>
                <td class="text-center"><strong>#${item.id}</strong></td>
                <td class="text-center">${rfidDisplay}</td>
                <td>${item.userName}</td>
                <td>${item.productName}</td>
                <td class="text-center">${item.startDate}</td>
                <td class="text-center">${item.endDate}</td>
                <td class="text-center">${item.pauseDate}</td>
                <td class="text-center">${item.resumeDate}</td>
                <td class="text-center">${statusBadge}</td>
                <td class="text-center">
                    <button class="btn btn-outline-info me-1" onclick="showRegisterModal(${item.id}, '${item.rfiD_UID || ''}')" title="Đăng ký mã">
                        <i class="ti ti-id"></i>
                    </button>
                    <button class="btn ${toggleBtnClass} me-1" onclick="toggleStatus(${item.id})" title="${toggleText}">
                        <i class="ti ${toggleIcon}"></i>
                    </button>
                    <button class="btn btn-outline-danger" onclick="showDeleteModal(${item.id})" title="Xóa thẻ">
                        <i class="ti ti-trash"></i>
                    </button>
                </td>
            </tr>
        `;
        tbody.append(row);
    });
}

// ================= THÊM MỚI THẺ =================

function showAddModal() {
    let quantity = $("#txtQuantity").val();
    if (!quantity || quantity <= 0) {
        showToast("Vui lòng nhập số lượng hợp lệ (lớn hơn 0).", 500);
        return;
    }
    $("#lblConfirmQuantity").text(quantity);
    $("#modal-add-confirm").modal("show");
}

function submitAddCards() {
    let quantity = $("#txtQuantity").val();
    
    $.ajax({
        url: '/GymMembershipCard/GenerateCards',
        type: 'POST',
        data: { quantity: quantity },
        success: function (response) {
            if (response.success) {
                showToast(response.message, 200);
                $("#modal-add-confirm").modal("hide");
                $("#txtQuantity").val(1); // Reset input
                loadCards(); // Tải lại bảng
            } else {
                showToast(response.message, 500);
            }
        }
    });
}

// ================= ĐĂNG KÝ MÃ RFID =================

function showRegisterModal(id, currentRfid) {
    $("#rfidCardId").val(id);
    $("#txtRfidUid").val(currentRfid);
    $("#modal-register-rfid").modal("show");
    
    // Đặt trỏ chuột vào input sau khi modal mở để quẹt thẻ tiện hơn
    setTimeout(function() { $("#txtRfidUid").focus(); }, 500);
}

function submitRfid() {
    let id = $("#rfidCardId").val();
    let rfidUid = $("#txtRfidUid").val();

    if (!rfidUid.trim()) {
        showToast("Vui lòng nhập hoặc quẹt mã RFID.", 500);
        return;
    }

    $.ajax({
        url: '/GymMembershipCard/UpdateRFID',
        type: 'POST',
        data: { id: id, rfidUid: rfidUid.trim() },
        success: function (response) {
            if (response.success) {
                showToast(response.message, 200);
                $("#modal-register-rfid").modal("hide");
                loadCards();
            } else {
                showToast(response.message, 500);
            }
        }
    });
}

// ================= KHÓA / MỞ THẺ =================

function toggleStatus(id) {
    $.ajax({
        url: '/GymMembershipCard/ToggleStatus',
        type: 'POST',
        data: { id: id },
        success: function (response) {
            if (response.success) {
                showToast(response.message, 200);
                loadCards();
            } else {
                showToast(response.message, 500);
            }
        }
    });
}

// ================= XÓA THẺ =================

function showDeleteModal(id) {
    $("#deleteCardId").val(id);
    $("#modal-delete-confirm").modal("show");
}

function submitDelete() {
    let id = $("#deleteCardId").val();
    
    $.ajax({
        url: '/GymMembershipCard/DeleteCard',
        type: 'POST',
        data: { id: id },
        success: function (response) {
            if (response.success) {
                showToast(response.message, 200);
                $("#modal-delete-confirm").modal("hide");
                loadCards();
            } else {
                showToast(response.message, 500);
            }
        }
    });
}