$(document).ready(function () {
    // Set ngày mặc định cho ô chọn lịch là hôm nay
    document.getElementById('filterDate').valueAsDate = new Date();
    loadLatestToday();
    loadCheckins();
});

$('#cardIdInput').on('keypress', function(e) {
    if (e.key === 'Enter') {
        performCheckin();
    }
});

// 2. Load danh sách Check-in theo ngày
function loadCheckins() {
    let date = $('#filterDate').val();
    $.get(`/Checkin/listCheckins?date=${date}`, function (res) {
        let html = '';
        if (res.length === 0) {
            html = '<tr><td colspan="5" class="text-center text-muted py-4">Chưa có ai check-in ngày này.</td></tr>';
        } else {
            res.forEach(item => {
                const statusMap = {
                    active:  '<span class="badge bg-success">Còn hạn</span>',
                    expired: '<span class="badge bg-danger">Hết hạn</span>',
                    locked:  '<span class="badge bg-warning text-dark">Thẻ khóa</span>'
                };
                let statusBadge = statusMap[item.cardStatus] || statusMap.active;

                let expText  = formatEndDate(item.endDate);

                html += `<tr>
                            <td class="text-center fw-bold text-primary">${item.checkinTime}</td>
                            <td class="fw-bold">${item.rfidUid}</td>
                            <td>
                                <div class="d-flex align-items-center">
                                    <img src="${item.avatar}" class="rounded-circle me-2" style="width:30px;height:30px;object-fit:cover;">
                                    ${item.fullName}
                                </div>
                            </td>
                            <td>${expText}</td>
                            <td class="text-center">${statusBadge}</td>
                        </tr>`;
            });
        }
        $('#checkinTableBody').html(html);
    });
}

// 3. Thực hiện Check-in
function performCheckin() {
    let RFID_UID = $('#cardIdInput').val().toUpperCase().trim();
    if (!RFID_UID) {
        showToast('Không xác định được mã thẻ!', 500);
        return;
    }

    $.post(`/Checkin/${RFID_UID}`)
        .done(function (res) {
            showToast(res.message, 200);
            loadCheckins(); // Reload bảng bên trái
            showMemberInfo(res.cardInfo); // Hiển thị bảng bên phải
            $('#cardIdInput').val(''); // Xóa ô input
        })
        .fail(function (err) {
            showToast(err.responseText || 'Có lỗi xảy ra khi check-in!', 500);
        });
}

// 4. Hiển thị thông tin lên khung bên phải
function showMemberInfo(info) {
    currentScannedCardId = info.id;
    $('#infoAvatar').attr('src', info.avatar);
    $('#infoName').text(info.fullName);
    $('#infoPhone').text(info.phoneNumber || 'Chưa cập nhật');
    $('#infoCardId').text(info.rfidUid);
    $('#infoStartDate').text(info.startDate);
    $('#infoEndDate').html(formatEndDate(info.endDate)); // đổi .text() → .html()

    const statusMap = {
        active:  { text: 'Còn hạn',  cls: 'text-success' },
        expired: { text: 'Hết hạn',  cls: 'text-danger'  },
        locked:  { text: 'Thẻ khóa', cls: 'text-warning' }
    };

    const s = statusMap[info.cardStatus] || statusMap.active;
    $('#infoCardStatus').removeClass('text-success text-danger text-warning')
                        .addClass(s.cls)
                        .text(s.text);

    // Bỏ hết khối if/else xử lý màu endDate ở đây vì formatEndDate đã lo

    if (info.cardStatus === 'expired') {
        $('#expiredWarning').show();
    } else {
        $('#expiredWarning').hide();
    }

    $('#memberInfoCard').fadeIn();
}

function loadLatestToday() {
    $.get('/Checkin/latestToday', function(res) {
        if (res) {
            showMemberInfo(res);
        }
        else{
            showToast('Không có thông tin check-in nào hôm nay.', 200);
        }
    });
}

// 5. Nút Gia hạn
function extendMembership() {
    if (!currentScannedCardId) return;

    HienModalXacNhan('Bạn chắc chắn muốn gia hạn cho hội viên này?', 'bg-success', function () {
        $.post(`/Checkin/extend/${currentScannedCardId}`)
            .done(function (res) {
                showToast(res.message, 200);
                $('#infoEndDate').text(res.newEndDate);
            })
            .fail(function (xhr) {
                var msg = xhr.responseJSON?.message || 'Lỗi khi gia hạn!';
                showToast(msg, 500);
            });
    });
}

// 6. Nút Khóa thẻ
function lockMembership() {
    if (!currentScannedCardId) return;

    var isLocked = $('#btnLockToggle').data('status') === false;

    if (isLocked) {
        // Đang khóa → Mở thẻ
        HienModalXacNhan('Bạn chắc chắn muốn mở lại thẻ này?', 'bg-primary', function () {
            $.post(`/Checkin/unlock/${currentScannedCardId}`)
                .done(function (res) {
                    showToast(res.message, 200);
                    $('#infoEndDate').text(res.newEndDate);
                    $('#infoCardStatus').html('<span class="badge bg-success">Hoạt động</span>');
                    setLockButtonState(true);
                })
                .fail(function (xhr) {
                    var msg = xhr.responseJSON?.message || 'Lỗi khi mở thẻ!';
                    showToast(msg, 500);
                });
        });
    } else {
        // Đang mở → Khóa thẻ
        HienModalXacNhan('Bạn chắc chắn muốn khóa tạm thời thẻ này?', 'bg-danger', function () {
            $.post(`/Checkin/lock/${currentScannedCardId}`)
                .done(function (res) {
                    showToast(res.message, 200);
                    $('#infoCardStatus').html('<span class="badge bg-danger">Đã khóa</span>');
                    setLockButtonState(false);
                })
                .fail(function (xhr) {
                    var msg = xhr.responseJSON?.message || 'Lỗi khi khóa thẻ!';
                    showToast(msg, 500);
                });
        });
    }
}

function setLockButtonState(isActive) {
    var btn = $('#btnLockToggle');
    if (isActive) {
        btn.removeClass('btn-outline-primary').addClass('btn-outline-danger')
           .html('<i class="bi bi-shield-lock"></i> Khóa thẻ')
           .data('status', true);
        $('#infoCardStatus').html('<span class="badge bg-success">Hoạt động</span>');
    } else {
        btn.removeClass('btn-outline-danger').addClass('btn-outline-primary')
           .html('<i class="bi bi-shield-check"></i> Mở thẻ')
           .data('status', false);
        $('#infoCardStatus').html('<span class="badge bg-danger">Đã khóa</span>');
    }
}

function soNgayConLai(endDateStr) {
    var parts = endDateStr.split('/');
    var endDate = new Date(parts[2], parts[1] - 1, parts[0]);
    var today = new Date();
    today.setHours(0, 0, 0, 0);
    return Math.ceil((endDate - today) / (1000 * 60 * 60 * 24));
}

function formatEndDate(endDateStr, nguongCanhBao) {
    if (!endDateStr || endDateStr === '--') return '--';

    nguongCanhBao = nguongCanhBao || 5;
    var conLai = soNgayConLai(endDateStr);

    if (conLai < 0) {
        return `<span class="text-danger fw-bold">${endDateStr}</span>`;
    }

    if (conLai <= nguongCanhBao) {
        return `<span class="text-warning fw-bold" title="Còn ${conLai} ngày">${endDateStr}</span>`;
    }

    return endDateStr;
}