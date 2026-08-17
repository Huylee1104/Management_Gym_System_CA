const modal = new bootstrap.Modal(document.getElementById('dataModal'));

let selectedRoleId = null;
let selectedRoleName = '';
let permissionChanged = false;

const apiUrl = '/Role';

$(document).ready(function () {
    loadData();

    $('#txtSearch').on('keydown', function (e) {
        if (e.key === 'Enter') {
            loadData();
        }
    });
});

function loadData() {

    const keyword = $('#txtSearch').val();

    $.get(`${apiUrl}/listRoles`, { keyword })
        .done(function (res) {

            let html = '';

            if (!res || res.length === 0) {

                html = `
                    <tr>
                        <td colspan="4"
                            class="text-center text-muted py-5">
                            Không có vai trò nào.
                        </td>
                    </tr>
                `;

                $('#tableBody').html(html);
                return;
            }

            res.forEach(item => {

                const isSelected =
                    selectedRoleId == item.id;

                const statusClass =
                    item.status
                        ? 'bg-success-subtle text-success'
                        : 'bg-danger-subtle text-danger';

                const statusText =
                    item.status
                        ? 'Hoạt động'
                        : 'Khóa';

                // JSON.stringify giúp roleName có dấu ' " cũng không
                // làm hỏng onclick.
                const roleName =
                    JSON.stringify(item.roleName ?? '');

                html += `
                    <tr class="${isSelected ? 'table-active' : ''}"
                        style="cursor: pointer;"
                        onclick='selectRole(${item.id}, ${roleName})'>

                        <td class="text-center">
                            ${item.id}
                        </td>

                        <td>
                            <div class="fw-semibold">
                                ${escapeHtml(item.roleName)}
                            </div>
                        </td>

                        <td class="text-center">
                            <span class="badge ${statusClass}">
                                ${statusText}
                            </span>
                        </td>

                        <td class="text-center">

                            <div class="d-flex
                                        justify-content-center
                                        gap-1">

                                <!-- Sửa -->
                                <button type="button"
                                        class="btn btn-sm btn-outline-primary"
                                        title="Sửa"
                                        onclick="
                                            event.stopPropagation();
                                            editRole(${item.id});
                                        ">

                                    <i class="bi bi-pencil"></i>

                                </button>

                                <!-- Trạng thái -->
                                <button type="button"
                                        class="btn btn-sm btn-outline-warning"
                                        title="${item.status ? 'Khóa' : 'Kích hoạt'}"
                                        onclick="
                                            event.stopPropagation();
                                            toggleStatus(${item.id});
                                        ">

                                    <i class="bi ${
                                        item.status
                                            ? 'bi-toggle-on'
                                            : 'bi-toggle-off'
                                    }"></i>

                                </button>

                                <!-- Xóa -->
                                <button type="button"
                                        class="btn btn-sm btn-outline-danger"
                                        title="Xóa"
                                        onclick="
                                            event.stopPropagation();
                                            deleteRole(${item.id});
                                        ">

                                    <i class="bi bi-trash"></i>

                                </button>

                            </div>

                        </td>

                    </tr>
                `;
            });

            $('#tableBody').html(html);
        })
        .fail(function (xhr) {

            $('#tableBody').html(`
                <tr>
                    <td colspan="4"
                        class="text-center text-danger py-5">
                        Không thể tải danh sách vai trò.
                    </td>
                </tr>
            `);

            console.error(
                'Lỗi tải danh sách role:',
                xhr
            );
        });
}

function showModal() {
    $('#roleId').val(0);
    $('#roleName').val('');
    $('#roleStatus').prop('checked', true);
    $('#modalTitle').text('Thêm Vai trò');
    modal.show();
}

function editData(btn) {
    let item = JSON.parse($(btn).attr('data-item'));
    $('#roleId').val(item.id);
    $('#roleName').val(item.roleName);
    $('#roleStatus').prop('checked', item.status);
    $('#modalTitle').text('Sửa Vai trò');
    modal.show();
}

function saveData() {
    let id = $('#roleId').val();
    let payload = {
        id: parseInt(id),
        roleName: $('#roleName').val(),
        status: $('#roleStatus').is(':checked')
    };

    let method = id == 0 ? 'POST' : 'POST'; // Cả 2 đều POST theo API controller của bạn
    let url = id == 0 ? apiUrl : `${apiUrl}/${id}`;

    $.ajax({
        url: url, type: method, contentType: 'application/json',
        data: JSON.stringify(payload),
        success: function () {
            showToast('Lưu thành công!', 200);
            modal.hide();
            loadData();
        },
        error: function () { showToast('Có lỗi xảy ra!', 500); }
    });
}

function toggleStatus(id) {
    $.post(`${apiUrl}/${id}/status`, function () {
        showToast('Đã cập nhật trạng thái', 200);
        loadData();
    });
}

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

function selectRole(id, roleName) {

    if (permissionChanged) {

        const confirmChange =
            confirm(
                'Bạn đang có thay đổi chưa lưu. ' +
                'Bạn có chắc muốn chuyển vai trò không?'
            );

        if (!confirmChange)
            return;
    }

    selectedRoleId = id;
    selectedRoleName = roleName;
    permissionChanged = false;

    $('#permissionTitle')
        .text(`Phân quyền: ${roleName}`);

    $('#permissionSubtitle')
        .text('Đang tải quyền...');

    $('#btnSavePermissions')
        .prop('disabled', true);

    loadPermissionTree(id);

    loadData();
}

function loadPermissionTree(roleId) {

    $('#permissionContainer').html(`
        <div class="text-center py-5">
            <div class="spinner-border text-primary"></div>

            <div class="mt-3 text-muted">
                Đang tải quyền...
            </div>
        </div>
    `);

    $.get(`${apiUrl}/${roleId}/permissions`)
        .done(function (res) {

            renderPermissionTree(res);

            $('#permissionSubtitle')
                .text('Quản lý quyền truy cập của vai trò');

        })
        .fail(function (xhr) {

            $('#permissionContainer').html(`
                <div class="alert alert-danger">
                    Không thể tải danh sách quyền.
                </div>
            `);

            $('#permissionSubtitle')
                .text('Không thể tải dữ liệu');

        });
}

function renderPermissionTree(data) {

    if (!data.functions ||
        data.functions.length === 0) {

        $('#permissionContainer').html(`
            <div class="text-center text-muted py-5">
                Chưa có chức năng nào được cấu hình.
            </div>
        `);

        return;
    }

    let html = '';

    data.functions.forEach(function (func, index) {

        const actions = func.actions || [];

        const allowedCount =
            actions.filter(x => x.isAllowed).length;

        const allAllowed =
            actions.length > 0 &&
            allowedCount === actions.length;

        const someAllowed =
            allowedCount > 0 &&
            !allAllowed;

        html += `
            <div class="permission-function mb-3">

                <div class="function-header">

                    <div class="form-check mb-0">

                        <input
                            class="form-check-input
                                   function-checkbox"
                            type="checkbox"
                            id="function_${func.id}"
                            data-function-id="${func.id}"
                            ${allAllowed ? 'checked' : ''}
                            ${someAllowed ? 'data-indeterminate="true"' : ''}
                        >

                        <label
                            class="form-check-label fw-semibold"
                            for="function_${func.id}">

                            ${escapeHtml(func.name)}

                        </label>

                    </div>

                    <span class="permission-count"
                          data-count-for="${func.id}">
                        ${allowedCount}/${actions.length}
                    </span>

                </div>

                <div class="function-actions">
        `;

        actions.forEach(function (action) {

            html += `
                <div class="action-item">

                    <div class="form-check">

                        <input
                            class="form-check-input action-checkbox"
                            type="checkbox"
                            id="action_${action.id}"
                            data-function-id="${func.id}"
                            data-action-id="${action.id}"
                            ${action.isAllowed ? 'checked' : ''}
                        >

                        <label
                            class="form-check-label"
                            for="action_${action.id}">

                            <span class="fw-medium">
                                ${escapeHtml(action.displayName)}
                            </span>

                            <small class="text-muted ms-2">
                                ${escapeHtml(action.code)}
                            </small>

                        </label>

                    </div>

                </div>
            `;
        });

        html += `
                </div>
            </div>
        `;
    });

    $('#permissionContainer').html(html);

    initializePermissionCheckboxes();
}

function initializePermissionCheckboxes() {

    $('.function-checkbox').each(function () {

        const checkbox = this;

        if ($(checkbox).attr('data-indeterminate') === 'true') {
            checkbox.indeterminate = true;
        }
    });

    $('.function-checkbox').on('change', function () {

        const functionId =
            $(this).data('function-id');

        const checked =
            $(this).is(':checked');

        $(`.action-checkbox[data-function-id="${functionId}"]`)
            .prop('checked', checked);

        updateFunctionCount(functionId);

        permissionChanged = true;

        $('#btnSavePermissions')
            .prop('disabled', false);
    });


    $('.action-checkbox').on('change', function () {

        const functionId =
            $(this).data('function-id');

        updateFunctionCheckbox(functionId);

        permissionChanged = true;

        $('#btnSavePermissions')
            .prop('disabled', false);
    });
}

function updateFunctionCount(functionId) {

    const actions =
        $(`.action-checkbox[data-function-id="${functionId}"]`);

    const allowed =
        actions.filter(':checked').length;

    const total =
        actions.length;

    $(`[data-count-for="${functionId}"]`)
        .text(`${allowed}/${total}`);
}

function updateFunctionCheckbox(functionId) {

    const checkbox =
        $(`#function_${functionId}`)[0];

    const actions =
        $(`.action-checkbox[data-function-id="${functionId}"]`);

    const checked =
        actions.filter(':checked').length;

    const total =
        actions.length;

    checkbox.indeterminate =
        checked > 0 && checked < total;

    checkbox.checked =
        total > 0 && checked === total;

    updateFunctionCount(functionId);
}

function savePermissions() {

    if (!selectedRoleId)
        return;

    const permissions = [];

    $('.action-checkbox').each(function () {

        permissions.push({
            actionId: Number($(this).data('action-id')),
            isAllowed: $(this).is(':checked')
        });

    });

    const button =
        $('#btnSavePermissions');

    button.prop('disabled', true);

    $.ajax({
        url: `${apiUrl}/${selectedRoleId}/permissions`,
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(permissions),

        success: function (res) {

            permissionChanged = false;

            showToast(
                res.message || 'Lưu phân quyền thành công!',
                200
            );

            button.prop('disabled', true);

        },

        error: function (xhr) {

            button.prop('disabled', false);

            const message =
                xhr.responseJSON?.message ||
                'Không thể lưu phân quyền.';

            showToast(message, 500);
        }
    });
}

function escapeHtml(value) {

    if (value == null)
        return '';

    return $('<div>')
        .text(value)
        .html();
}