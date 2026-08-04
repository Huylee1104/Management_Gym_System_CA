$(document).ready(() => {
        loadCategories();
        loadData();
});

$('#selectFilterCategory').on('change', function () {
    loadData();
});

$('#txtSearch').on('keydown', function (e) {

    if (e.keyCode == 13) {
        loadData();
    }

});

const input = document.getElementById("prodPrice");

input.addEventListener("input", function (e) {
    let value = e.target.value.replace(/,/g, ""); // bỏ dấu ,

    if (!isNaN(value) && value !== "") {
        e.target.value = Number(value).toLocaleString("en-US");
    } else {
        e.target.value = "";
    }
});

function loadCategories() {

    $.get(catApiUrl, function(res) {

        let options = res.map(c =>
            `<option value="${c.id}">${c.categoryName}</option>`
        );

        $('#categoryId').html(options.join(''));

        categorySelect = new TomSelect("#categoryId", {
            create: false,
            sortField: {
                field: "text",
                direction: "asc"
            }
        });

        $('#selectFilterCategory').html(`
            <option value="0">Tất cả</option>
            ${options.join('')}
        `);

        filterCategorySelect = new TomSelect("#selectFilterCategory", {
            create: false,
            sortField: {
                field: "text",
                direction: "asc"
            }
        });

    });
}

function loadData() {
    let categoryId = $('#selectFilterCategory').val();
    let keyword = $('#txtSearch').val();

    $.get(`${apiUrl}/listProducts`,
    {
        categoryId,
        keyword
    },
    function (res) {
        let html = '';
        res.forEach(item => {
            let statusBadge = item.status ? '<span class="badge bg-success">Đang bán</span>' : '<span class="badge bg-danger">Ngừng bán</span>';
            let imgSrc = item.imageProduct ? item.imageProduct : 'https://placehold.co/50';
            
            html += `<tr>
                        <td><img src="${imgSrc}" style="width:50px; height:50px; object-fit:cover;" class="rounded"/></td>
                        <td class="fw-bold">${item.productName}</td>
                        <td>${item.categoryName}</td>
                        <td class="text-danger fw-bold">${item.price.toLocaleString()} đ</td>
                        <td>${item.unit}</td>
                        <td class="text-center"><a href="javascript:void(0)" onclick="toggleStatus(${item.id})">${statusBadge}</a></td>
                        <td class="text-center">
                            <button class="btn btn-outline-info" onclick='editData(${JSON.stringify(item)})'><i class="bi bi-pencil"></i></button>
                            <button class="btn btn-outline-danger" onclick='confirmDelete(${item.id})'><i class="bi bi-trash"></i></button>
                        </td>
                        </tr>`;
        });
        $('#tableBody').html(html);
    });
}

// Chuyển file ảnh sang Base64
function encodeImageFileAsURL(element) {
    let file = element.files[0];
    if(!file) return;
    let reader = new FileReader();
    reader.onloadend = function() {
        $('#imgPreview').attr('src', reader.result);
        $('#imageBase64').val(reader.result);
    }
    reader.readAsDataURL(file);
}

function showModal() {

    $('#prodId').val(0);
    $('#prodName').val('');
    $('#prodPrice').val('');
    $('#prodUnit').val('');
    $('#thoihan').val('');
    $('#prodStatus').prop('checked', true);

    $('#imgPreview').attr('src', 'https://placehold.co/150');

    $('#imageBase64').val('');
    $('#fileUpload').val('');

    categorySelect.clear();

    $('#modalTitle').text('Thêm Sản phẩm');

    modal.show();
}

function editData(item) { 
    $('#prodId').val(item.id); $('#prodName').val(item.productName); $('#prodPrice').val(Number(item.price).toLocaleString('en-US'));
    $('#prodUnit').val(item.unit); $('#prodStatus').prop('checked', item.status);
    categorySelect.clear(true);
    categorySelect.setValue(item.categoryID);
    
    let imgSrc = item.imageProduct ? item.imageProduct : 'https://placehold.co/150';
    $('#imgPreview').attr('src', imgSrc);
    $('#imageBase64').val(item.imageProduct);
    $('#fileUpload').val('');
    $('#thoihan').val(item.thoiHan || '');

    $('#modalTitle').text('Sửa Sản phẩm'); 
    modal.show(); 
}

function saveData() {
    let id = $('#prodId').val();
    let payload = { 
        id: parseInt(id), 
        productName: $('#prodName').val(), 
        categoryID: parseInt($('#categoryId').val()),
        price: parseFloat($('#prodPrice').val().replace(/,/g, '')),
        unit: $('#prodUnit').val(),
        ThoiHan: parseInt($('#thoihan').val()) || null,
        imageProduct: $('#imageBase64').val(), // Base64 chuỗi ảnh
        status: $('#prodStatus').is(':checked') 
    };

    if(!payload.categoryID || !payload.productName || payload.price <= 0) {
        showToast('Vui lòng điền đủ thông tin hợp lệ!', 500);
        return;
    }

    $.ajax({
        url: id == 0 ? apiUrl : `${apiUrl}/${id}`, type: 'POST', contentType: 'application/json',
        data: JSON.stringify(payload),
        success: () => { showToast('Lưu thành công!', 200); modal.hide(); loadData(); },
        error: (err) => { showToast(err.responseText || 'Có lỗi xảy ra!', 500); }
    });
}

function toggleStatus(id) { $.post(`${apiUrl}/${id}/status`, () => { showToast('Đã cập nhật!', 200); loadData(); }); }

function confirmDelete(id) {
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