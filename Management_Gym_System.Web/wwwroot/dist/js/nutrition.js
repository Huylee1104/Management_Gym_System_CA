let isAdminUser = false;
let currentSelectedGroupId = 0;
let currentSelectedGroupName = "";
let currentSelectedDetailId = 0;
let cachedGroups = [];

document.addEventListener("DOMContentLoaded", function () {
    const adminFlagInput = document.getElementById("isAdminFlag");
    if (adminFlagInput && adminFlagInput.value === "true") {
        isAdminUser = true;
    }

    // Mặc định khi mới vào trang: tải danh sách nhóm thực phẩm (thẻ Nhóm đã mở sẵn)
    loadGroups();
});

function toggleAccordion(targetTab) {
    const headerGroups = document.getElementById("headerGroups");
    const panelGroups = document.getElementById("panelGroups");
    const headerDetails = document.getElementById("headerDetails");
    const panelDetails = document.getElementById("panelDetails");

    if (targetTab === "groups") {
        const isCurrentlyOpen = !panelGroups.classList.contains("d-none");

        if (isCurrentlyOpen) {
            // Đang mở -> Bấm lần nữa thì đóng lại
            panelGroups.classList.add("d-none");
            headerGroups.classList.remove("is-open");
        } else {
            // Đang đóng -> Mở thẻ Nhóm và buộc đóng thẻ Thực phẩm
            panelGroups.classList.remove("d-none");
            headerGroups.classList.add("is-open");

            panelDetails.classList.add("d-none");
            headerDetails.classList.remove("is-open");
        }
    } else if (targetTab === "details") {
        const isCurrentlyOpen = !panelDetails.classList.contains("d-none");

        if (isCurrentlyOpen) {
            // Đang mở -> Bấm lần nữa thì đóng lại
            panelDetails.classList.add("d-none");
            headerDetails.classList.remove("is-open");
        } else {
            // Đang đóng -> Mở thẻ Thực phẩm và buộc đóng thẻ Nhóm
            panelDetails.classList.remove("d-none");
            headerDetails.classList.add("is-open");

            panelGroups.classList.add("d-none");
            headerGroups.classList.remove("is-open");
        }
    }
}

function openSpecificAccordion(targetTab) {
    const headerGroups = document.getElementById("headerGroups");
    const panelGroups = document.getElementById("panelGroups");
    const headerDetails = document.getElementById("headerDetails");
    const panelDetails = document.getElementById("panelDetails");

    if (targetTab === "groups") {
        panelGroups.classList.remove("d-none");
        headerGroups.classList.add("is-open");
        panelDetails.classList.add("d-none");
        headerDetails.classList.remove("is-open");
    } else if (targetTab === "details") {
        panelDetails.classList.remove("d-none");
        headerDetails.classList.add("is-open");
        panelGroups.classList.add("d-none");
        headerGroups.classList.remove("is-open");
    }
}

// ==========================================
// XỬ LÝ DỮ LIỆU: NHÓM THỰC PHẨM
// ==========================================

async function loadGroups() {
    const container = document.getElementById("groupListContainer");

    try {
        const response = await fetch("/Nutrition/GetGroups");
        const result = await response.json();

        if (result.success === false) {
            container.innerHTML = `<div class="text-danger text-center py-3">${result.message}</div>`;
            return;
        }

        cachedGroups = result.data;
        renderGroupDropdownInModal();

        if (cachedGroups.length === 0) {
            container.innerHTML = `<div class="text-secondary text-center py-3">Chưa có nhóm thực phẩm nào.</div>`;
            return;
        }

        let html = "";
        for (let i = 0; i < cachedGroups.length; i++) {
            const item = cachedGroups[i];
            const activeClass = item.id === currentSelectedGroupId ? "active" : "";
            const statusBadge = (isAdminUser && item.isActive === false)
                ? `<span class="badge bg-secondary ms-1">Ẩn</span>`
                : "";

            let adminButtonsHtml = "";
            if (isAdminUser) {
                adminButtonsHtml = `
                    <div class="d-flex align-items-center gap-1 ms-2">
                        <button type="button" class="btn btn-sm btn-icon btn-ghost-secondary rounded-circle" title="Sửa nhóm" onclick="openEditGroupModal(event, ${item.id})">
                            <svg xmlns="http://www.w3.org/2000/svg" class="icon" width="18" height="18" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round">
                                <path stroke="none" d="M0 0h24v24H0z" fill="none"/>
                                <path d="M4 20h4l10.5 -10.5a2.828 2.828 0 1 0 -4 -4l-10.5 10.5v4" />
                                <path d="M13.5 6.5l4 4" />
                            </svg>
                        </button>
                        <button type="button" class="btn btn-sm btn-icon btn-ghost-danger rounded-circle" title="Xóa nhóm" onclick="deleteGroup(event, ${item.id})">
                            <svg xmlns="http://www.w3.org/2000/svg" class="icon" width="18" height="18" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round">
                                <path stroke="none" d="M0 0h24v24H0z" fill="none"/>
                                <path d="M4 7l16 0" />
                                <path d="M10 11l0 6" />
                                <path d="M14 11l0 6" />
                                <path d="M5 7l1 12a2 2 0 0 0 2 2h8a2 2 0 0 0 2 -2l1 -12" />
                                <path d="M9 7v-3a1 1 0 0 1 1 -1h4a1 1 0 0 1 1 1v3" />
                            </svg>
                        </button>
                    </div>
                `;
            }

            html += `
                <div class="nutrition-list-item d-flex align-items-center justify-content-between ${activeClass}" onclick="selectGroup(${item.id}, '${escapeHtml(item.name)}')">
                    <div class="fw-medium text-truncate">
                        ${escapeHtml(item.name)}
                        ${statusBadge}
                    </div>
                    ${adminButtonsHtml}
                </div>
            `;
        }

        container.innerHTML = html;
    } catch (error) {
        console.error(error);
        container.innerHTML = `<div class="text-danger text-center py-3">Lỗi kết nối máy chủ.</div>`;
    }
}

function selectGroup(groupId, groupName) {
    currentSelectedGroupId = groupId;
    currentSelectedGroupName = groupName;

    const subtitle = document.getElementById("selectedGroupSubtitle");
    if (subtitle) {
        subtitle.textContent = "Nhóm: " + groupName;
    }

    // Cập nhật lại giao diện active của danh sách nhóm
    loadGroups();

    // Tải danh sách thực phẩm thuộc nhóm này và chuyển sang mở thẻ Quản lý thực phẩm
    loadDetailsByGroup(groupId);
    openSpecificAccordion("details");
}

function renderGroupDropdownInModal() {
    const selectElement = document.getElementById("detailGroupId");
    if (!selectElement) {
        return;
    }

    let optionsHtml = `<option value="">-- Chọn nhóm thực phẩm --</option>`;
    for (let i = 0; i < cachedGroups.length; i++) {
        const group = cachedGroups[i];
        optionsHtml += `<option value="${group.id}">${escapeHtml(group.name)}</option>`;
    }
    selectElement.innerHTML = optionsHtml;
}

// ==========================================
// XỬ LÝ DỮ LIỆU: DANH SÁCH & CHI TIẾT THỰC PHẨM
// ==========================================

async function loadDetailsByGroup(groupId) {
    const container = document.getElementById("detailListContainer");
    container.innerHTML = `<div class="text-center py-3 text-secondary">Đang tải thực phẩm...</div>`;

    try {
        const response = await fetch(`/Nutrition/GetDetailsByGroup?groupId=${groupId}`);
        const result = await response.json();

        if (result.success === false) {
            container.innerHTML = `<div class="text-danger text-center py-3">${result.message}</div>`;
            return;
        }

        const details = result.data;
        if (details.length === 0) {
            container.innerHTML = `<div class="text-secondary text-center py-3">Nhóm này chưa có thực phẩm nào.</div>`;
            return;
        }

        let html = "";
        for (let i = 0; i < details.length; i++) {
            const item = details[i];
            const activeClass = item.id === currentSelectedDetailId ? "active" : "";
            const draftBadge = (isAdminUser && item.isPublished === false)
                ? `<span class="badge bg-secondary ms-1">Nháp</span>`
                : "";

            let adminButtonsHtml = "";
            if (isAdminUser) {
                adminButtonsHtml = `
                    <div class="d-flex align-items-center gap-1 ms-2">
                        <button type="button" class="btn btn-sm btn-icon btn-ghost-secondary rounded-circle" title="Sửa thực phẩm" onclick="openEditDetailModal(event, ${item.id})">
                            <svg xmlns="http://www.w3.org/2000/svg" class="icon" width="18" height="18" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round">
                                <path stroke="none" d="M0 0h24v24H0z" fill="none"/>
                                <path d="M4 20h4l10.5 -10.5a2.828 2.828 0 1 0 -4 -4l-10.5 10.5v4" />
                                <path d="M13.5 6.5l4 4" />
                            </svg>
                        </button>
                        <button type="button" class="btn btn-sm btn-icon btn-ghost-danger rounded-circle" title="Xóa thực phẩm" onclick="deleteDetail(event, ${item.id})">
                            <svg xmlns="http://www.w3.org/2000/svg" class="icon" width="18" height="18" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round">
                                <path stroke="none" d="M0 0h24v24H0z" fill="none"/>
                                <path d="M4 7l16 0" />
                                <path d="M10 11l0 6" />
                                <path d="M14 11l0 6" />
                                <path d="M5 7l1 12a2 2 0 0 0 2 2h8a2 2 0 0 0 2 -2l1 -12" />
                                <path d="M9 7v-3a1 1 0 0 1 1 -1h4a1 1 0 0 1 1 1v3" />
                            </svg>
                        </button>
                    </div>
                `;
            }

            html += `
                <div class="nutrition-list-item d-flex align-items-center justify-content-between ${activeClass}" onclick="selectDetail(${item.id})">
                    <div class="text-truncate">
                        <div class="fw-medium text-truncate">
                            ${escapeHtml(item.name)}
                            ${draftBadge}
                        </div>
                        <small class="text-secondary">${item.calories} kcal / ${item.servingSize}${escapeHtml(item.servingUnit)}</small>
                    </div>
                    ${adminButtonsHtml}
                </div>
            `;
        }

        container.innerHTML = html;
    } catch (error) {
        console.error(error);
        container.innerHTML = `<div class="text-danger text-center py-3">Lỗi tải danh sách thực phẩm.</div>`;
    }
}

async function selectDetail(detailId) {
    currentSelectedDetailId = detailId;

    if (currentSelectedGroupId > 0) {
        loadDetailsByGroup(currentSelectedGroupId);
    }

    try {
        const response = await fetch(`/Nutrition/GetDetailById?id=${detailId}`);
        const result = await response.json();

        if (result.success === false) {
            alert(result.message);
            return;
        }

        const data = result.data;

        document.getElementById("detailEmptyState").classList.add("d-none");
        document.getElementById("detailContentArea").classList.remove("d-none");

        const imageElement = document.getElementById("viewDetailImage");
        if (data.imageUrl && data.imageUrl.trim() !== "") {
            imageElement.src = data.imageUrl;
        } else {
            imageElement.src = "https://placehold.co/600x320/FFF3E0/FF9800?text=" + encodeURIComponent(data.name);
        }

        document.getElementById("viewDetailGroupBadge").textContent = data.groupName || "Dinh dưỡng";
        document.getElementById("viewDetailName").textContent = data.name;
        document.getElementById("viewDetailServing").textContent = `${data.servingSize} ${data.servingUnit}`;
        document.getElementById("viewDetailDescription").textContent = data.description || "Chưa có mô tả chi tiết cho thực phẩm này.";

        document.getElementById("viewCalories").textContent = `${data.calories} kcal`;
        document.getElementById("viewProtein").textContent = `${data.protein} g`;
        document.getElementById("viewCarb").textContent = `${data.carb} g`;
        document.getElementById("viewFat").textContent = `${data.fat} g`;
        document.getElementById("viewFiber").textContent = `${data.fiber} g`;
        document.getElementById("viewSugar").textContent = `${data.sugar} g`;
        document.getElementById("viewSodium").textContent = `${data.sodium} mg`;

        document.getElementById("viewRecommendedTime").textContent = data.recommendedTime || "Dùng linh hoạt trong ngày";
        document.getElementById("viewRecommendedTarget").textContent = data.recommendedTarget || "Mọi hội viên";
        document.getElementById("viewPros").textContent = data.pros || "Chưa cập nhật";
        document.getElementById("viewCons").textContent = data.cons || "Chưa cập nhật";
    } catch (error) {
        console.error(error);
        alert("Không thể tải chi tiết thực phẩm.");
    }
}

// ==========================================
// CRUD QUẢN TRỊ: NHÓM THỰC PHẨM
// ==========================================

function openCreateGroupModal(event) {
    event.stopPropagation();

    document.getElementById("modalGroupTitle").textContent = "Thêm nhóm thực phẩm";
    document.getElementById("formGroup").reset();
    document.getElementById("groupId").value = "0";
    document.getElementById("groupIsActive").value = "true";

    const modal = bootstrap.Modal.getOrCreateInstance(document.getElementById("modalGroup"));
    modal.show();
}

async function openEditGroupModal(event, groupId) {
    event.stopPropagation();

    try {
        const response = await fetch(`/Nutrition/GetGroupById?id=${groupId}`);
        const result = await response.json();

        if (result.success === false) {
            alert(result.message);
            return;
        }

        const data = result.data;
        document.getElementById("modalGroupTitle").textContent = "Cập nhật nhóm thực phẩm";
        document.getElementById("groupId").value = data.id;
        document.getElementById("groupName").value = data.name;
        document.getElementById("groupSortOrder").value = data.sortOrder !== null ? data.sortOrder : 1;
        document.getElementById("groupIsActive").value = data.isActive === false ? "false" : "true";
        document.getElementById("groupDescription").value = data.description || "";
        document.getElementById("groupImageFile").value = "";

        const modal = bootstrap.Modal.getOrCreateInstance(document.getElementById("modalGroup"));
        modal.show();
    } catch (error) {
        console.error(error);
        alert("Lỗi khi tải thông tin nhóm thực phẩm.");
    }
}

async function saveGroup(event) {
    event.preventDefault();

    const formElement = document.getElementById("formGroup");
    const formData = new FormData(formElement);
    const groupId = parseInt(document.getElementById("groupId").value, 10);

    const url = groupId > 0 ? "/Nutrition/UpdateGroup" : "/Nutrition/CreateGroup";

    try {
        const response = await fetch(url, {
            method: "POST",
            body: formData
        });
        const result = await response.json();

        if (result.success === false) {
            alert(result.message);
            return;
        }

        const modal = bootstrap.Modal.getInstance(document.getElementById("modalGroup"));
        if (modal) {
            modal.hide();
        }

        await loadGroups();
    } catch (error) {
        console.error(error);
        alert("Có lỗi xảy ra khi lưu nhóm thực phẩm.");
    }
}

async function deleteGroup(event, groupId) {
    event.stopPropagation();

    const confirmed = confirm("Bạn có chắc chắn muốn xóa nhóm thực phẩm này và các thực phẩm bên trong không?");
    if (confirmed === false) {
        return;
    }

    try {
        const formData = new FormData();
        formData.append("id", groupId);

        const response = await fetch("/Nutrition/DeleteGroup", {
            method: "POST",
            body: formData
        });
        const result = await response.json();

        if (result.success === false) {
            alert(result.message);
            return;
        }

        if (currentSelectedGroupId === groupId) {
            currentSelectedGroupId = 0;
            document.getElementById("selectedGroupSubtitle").textContent = "Chưa chọn nhóm";
            document.getElementById("detailListContainer").innerHTML = `<div class="text-center py-3 text-secondary">Vui lòng chọn một nhóm thực phẩm trước.</div>`;
        }

        await loadGroups();
    } catch (error) {
        console.error(error);
        alert("Không thể xóa nhóm thực phẩm.");
    }
}

// ==========================================
// CRUD QUẢN TRỊ: THỰC PHẨM CHI TIẾT
// ==========================================

function openCreateDetailModal(event) {
    event.stopPropagation();

    document.getElementById("modalDetailTitle").textContent = "Thêm thực phẩm mới";
    document.getElementById("formDetail").reset();
    document.getElementById("detailId").value = "0";
    document.getElementById("detailServingSize").value = "100";
    document.getElementById("detailServingUnit").value = "g";
    document.getElementById("detailIsPublished").value = "true";

    if (currentSelectedGroupId > 0) {
        document.getElementById("detailGroupId").value = currentSelectedGroupId;
    }

    const modal = bootstrap.Modal.getOrCreateInstance(document.getElementById("modalDetail"));
    modal.show();
}

async function openEditDetailModal(event, detailId) {
    event.stopPropagation();

    try {
        const response = await fetch(`/Nutrition/GetDetailById?id=${detailId}`);
        const result = await response.json();

        if (result.success === false) {
            alert(result.message);
            return;
        }

        const data = result.data;
        document.getElementById("modalDetailTitle").textContent = "Cập nhật thực phẩm";
        document.getElementById("detailId").value = data.id;
        document.getElementById("detailGroupId").value = data.nutritionGroupId;
        document.getElementById("detailName").value = data.name;
        document.getElementById("detailServingSize").value = data.servingSize;
        document.getElementById("detailServingUnit").value = data.servingUnit || "g";
        document.getElementById("detailIsPublished").value = data.isPublished === false ? "false" : "true";

        document.getElementById("detailCalories").value = data.calories;
        document.getElementById("detailProtein").value = data.protein;
        document.getElementById("detailCarb").value = data.carb;
        document.getElementById("detailFat").value = data.fat;
        document.getElementById("detailFiber").value = data.fiber;
        document.getElementById("detailSugar").value = data.sugar;
        document.getElementById("detailSodium").value = data.sodium;

        document.getElementById("detailRecommendedTime").value = data.recommendedTime || "";
        document.getElementById("detailRecommendedTarget").value = data.recommendedTarget || "";
        document.getElementById("detailPros").value = data.pros || "";
        document.getElementById("detailCons").value = data.cons || "";
        document.getElementById("detailDescription").value = data.description || "";
        document.getElementById("detailImageFile").value = "";

        const modal = bootstrap.Modal.getOrCreateInstance(document.getElementById("modalDetail"));
        modal.show();
    } catch (error) {
        console.error(error);
        alert("Lỗi khi tải thông tin thực phẩm.");
    }
}

async function saveDetail(event) {
    event.preventDefault();

    const formElement = document.getElementById("formDetail");
    const formData = new FormData(formElement);
    const detailId = parseInt(document.getElementById("detailId").value, 10);
    const selectedGroupId = parseInt(document.getElementById("detailGroupId").value, 10);

    const url = detailId > 0 ? "/Nutrition/UpdateDetail" : "/Nutrition/CreateDetail";

    try {
        const response = await fetch(url, {
            method: "POST",
            body: formData
        });
        const result = await response.json();

        if (result.success === false) {
            alert(result.message);
            return;
        }

        const modal = bootstrap.Modal.getInstance(document.getElementById("modalDetail"));
        if (modal) {
            modal.hide();
        }

        currentSelectedGroupId = selectedGroupId;
        await loadDetailsByGroup(currentSelectedGroupId);
        openSpecificAccordion("details");

        if (detailId > 0 && currentSelectedDetailId === detailId) {
            await selectDetail(detailId);
        }
    } catch (error) {
        console.error(error);
        alert("Có lỗi xảy ra khi lưu thực phẩm.");
    }
}

async function deleteDetail(event, detailId) {
    event.stopPropagation();

    const confirmed = confirm("Bạn có chắc chắn muốn xóa thực phẩm này không?");
    if (confirmed === false) {
        return;
    }

    try {
        const formData = new FormData();
        formData.append("id", detailId);

        const response = await fetch("/Nutrition/DeleteDetail", {
            method: "POST",
            body: formData
        });
        const result = await response.json();

        if (result.success === false) {
            alert(result.message);
            return;
        }

        if (currentSelectedDetailId === detailId) {
            currentSelectedDetailId = 0;
            document.getElementById("detailContentArea").classList.add("d-none");
            document.getElementById("detailEmptyState").classList.remove("d-none");
        }

        if (currentSelectedGroupId > 0) {
            await loadDetailsByGroup(currentSelectedGroupId);
        }
    } catch (error) {
        console.error(error);
        alert("Không thể xóa thực phẩm.");
    }
}

// Hàm phụ trợ chống lỗi ký tự đặc biệt khi render HTML
function escapeHtml(text) {
    if (!text) {
        return "";
    }
    return text
        .replace(/&/g, "&amp;")
        .replace(/</g, "&lt;")
        .replace(/>/g, "&gt;")
        .replace(/"/g, "&quot;")
        .replace(/'/g, "&#039;");
}