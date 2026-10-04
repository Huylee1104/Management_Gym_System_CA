$(document).ready(() => { loadRoles(); loadPlans(); loadData(); });

// 3. Lắng nghe sự kiện thay đổi bộ lọc
$("#selectFilter").on("change", function () {
    loadData();
});

$('#txtSearch').on('keydown', function (e) {

    if (e.keyCode == 13) {
        loadData();
    }

});

function loadRoles() {
    $.get(roleApiUrl, function (res) {
        let options = res.map(r => `<option value="${r.id}">${r.roleName}</option>`);
        $('#roleId').html(options.join(''));
        tomSelectRole = new TomSelect("#roleId", { create: false });
    });
}

function loadPlans() {
    $.get(planApiUrl, function (res) {

        // chỉ lấy thoiHan != null
        let filtered = res.filter(p => p.thoiHan != null);

        let options = filtered.map(p => `
                    <option 
                        value="${p.id}" 
                        data-thoihan="${p.thoiHan}">
                        ${p.productName}
                    </option>
                `);

        // Destroy trước khi re-init để tránh lỗi duplicate
        if (tomSelectProduct) {
            tomSelectProduct.destroy();
            tomSelectProduct = null;
        }
        if (tomSelectFilter) {
            tomSelectFilter.destroy();
            tomSelectFilter = null;
        }

        $('#planId').html(options.join(''));
        $('#selectFilter').html('<option value="">Tất cả</option>' + options.join(''));

        tomSelectProduct = new TomSelect("#planId", {
            create: false
        });

        tomSelectFilter = new TomSelect("#selectFilter", {
            create: false
        });
    });
}

function loadData() {
    let keyword = $('#txtSearch').val();
    let filterValue = $('#selectFilter').val();
    $.get(`${apiUrl}/listUsers`, { keyword, filterValue }, function (res) {
        let html = '';
        res.forEach(item => {
            let statusBadge = item.status ? '<span class="badge bg-success">Active</span>' : '<span class="badge bg-danger">Banned</span>';
            let imgSrc = item.avatar ? item.avatar : 'https://placehold.co/50';
            html += `<tr>
                        <td class="text-center"><img src="${imgSrc}" class="rounded-circle shadow-sm" style="width:45px; height:45px; object-fit:cover;"/></td>
                        <td class="fw-bold">${item.fullName}</td>
                        <td>${item.roleName}</td>
                        <td>${item.goiTapName}</td> 
                        <td class="text-center">${item.phoneNumber || 'N/A'}</td>
                        <td class="text-center"><a href="javascript:void(0)" onclick="toggleStatus(${item.id})">${statusBadge}</a></td>
                        <td class="text-center">
                            <button class="btn btn-outline-info" data-item='${JSON.stringify(item)}' onclick="editData(this)">
                                <i class="bi bi-pencil"></i>
                            </button>
                            <button class="btn btn-outline-danger" onclick="showDeleteModal(${item.id})">
                                <i class="bi bi-trash"></i>
                            </button>
                        </td>
                    </tr>`;
        });
        $('#tableBody').html(html);
    });
}

function encodeImageFileAsURL(element) {
    let file = element.files[0];
    if (!file) return;
    let reader = new FileReader();
    reader.onloadend = function () {
        $('#imgPreview').attr('src', reader.result);
        $('#imageBase64').val(reader.result);
    }
    reader.readAsDataURL(file);
}

function showModal() {
    $('#userId').val(0); $('#fullName').val(''); $('#phone').val(''); $('#userStatus').prop('checked', true);
    $('#imgPreview').attr('src', 'https://placehold.co/150'); $('#imageBase64').val(''); $('#fileUpload').val('');
    $('#birthDate').val('');$('#gender').val('');$('#isMember').prop('checked', true);
    tomSelectRole.clear(); modal.show();
    tomSelectProduct.clear(); modal.show();
}

function editData(btn) {
    let item = JSON.parse($(btn).attr('data-item'));

    $('#userId').val(item.id);
    $('#fullName').val(item.fullName);
    $('#phone').val(item.phoneNumber);
    $('#userStatus').prop('checked', item.status);
    $('#isMember').prop('checked', item.userType === 1);
    tomSelectRole.clear(true);
    tomSelectRole.setValue(item.roleID);
    tomSelectProduct.clear(true);
    tomSelectProduct.setValue(item.goiTapID);
    $('#birthDate').val(item.ngaySinh?.split('T')[0] || "");
    $('#gender').val(item.gioiTinh);

    let imgSrc = item.avatar ? item.avatar : 'https://placehold.co/150';
    $('#imgPreview').attr('src', imgSrc);
    $('#imageBase64').val(item.avatar);
    $('#fileUpload').val('');
    modal.show();
}

function saveData() {
    let id = $('#userId').val();
    let selectedOption = $('#planId option:selected');
    let payload = {
        id: parseInt(id),
        fullName: $('#fullName').val(),
        phoneNumber: $('#phone').val() || null,
        roleID: parseInt($('#roleId').val()) || null,
        goiTapID: parseInt($('#planId').val()) || null,
        thoiHan: parseInt(selectedOption.attr('data-thoihan')) || null,
        NgaySinh: $('#birthDate').val() || null,
        GioiTinh: parseInt($('#gender').val()) || null,
        UserType: $("#isMember").is(":checked") ? 1 : null,
        avatar: $('#imageBase64').val() || null,
        status: $('#userStatus').is(':checked') || null,
    };
    $.ajax({
        url: id == 0 ? apiUrl : `${apiUrl}/${id}`, type: 'POST', contentType: 'application/json',
        data: JSON.stringify(payload),
        success: (response) => { 
            console.log(response);
            showToast(response.message, response.IsSuccess ? 200 : 500);
            if (response.IsSuccess) {
                modal.hide();
                loadData();
            }
        }
    });
}

function toggleStatus(id) { $.post(`${apiUrl}/${id}/status`, () => { showToast('Đã cập nhật!', 200); loadData(); }); }

function showDeleteModal(id) {
    $('#deleteCardId').val(id);
    $('#modal-delete-confirm').modal('show');
}

function submitDelete() {
    let id = $('#deleteCardId').val();
    $.post(`${apiUrl}/delete`, { id: id }, function (res) {
        if (res.success) {
            $('#modal-delete-confirm').modal('hide');
            showToast("Xóa thành công!", 200);
            loadData();
        } else {
            showToast(res.message || "Có lỗi xảy ra!", 500);
        }
    });
}