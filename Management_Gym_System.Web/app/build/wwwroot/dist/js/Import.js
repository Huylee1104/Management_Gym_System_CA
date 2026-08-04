$(document).ready(function () {
    loadProductsForSelect();
    const today = new Date();
    
    document.getElementById('filterFromDate').valueAsDate = today;
    document.getElementById('filterToDate').valueAsDate = today;

});

function loadProductsForSelect() {
    $.get('/api/products/listProducts', function (res) {
        // Chỉ lấy sản phẩm không có thời hạn
        productsList = res.filter(p => p.thoiHan == null);
        let options = productsList.map(p =>
            `<option value="${p.id}" data-price="${p.price}">
                ${p.productName} (ĐVT: ${p.unit})
            </option>`
        );
        $('#productSelect').html(options.join(''));
        tomSelectProduct = new TomSelect("#productSelect", {
            create: false,
            sortField: {
                field: "text",
                direction: "asc"
            }
        });

        $('#filterProductSelect').html(options.join(''));
        tomSelectProduct = new TomSelect("#filterProductSelect", {
            create: false,
            sortField: {
                field: "text",
                direction: "asc"
            }
        });
    });
}

function addProductToCart() {
    let productId = $('#productSelect').val();
    if (!productId) {
        showToastWithBg("Vui lòng chọn sản phẩm!", 300, "yellow");
        return;
    }
    let existing = cart.find(x => x.productId == productId);
    // Nếu đã có thì tăng SL
    if (existing) {

        existing.quantity += 1;

    } else {
        let pData = productsList.find(x => x.id == productId);
        if (!pData) return;
        cart.push({
            productId: parseInt(productId),
            productName: pData.productName,
            unit: pData.unit,
            thoiHan: pData.thoiHan,
            quantity: 1,
            price: pData.price ?? 0,
            batchCode: '',
            expiryDate: '',
            discount: 0,
            taxRate: 0
        });
    }
    renderCart();
}

function renderCart() {
    if (cart.length === 0) {
        $('#cartBody').html(`
            <tr id="emptyCartRow">
                <td colspan="12" class="text-center text-muted py-4">
                    <i class="bi bi-cart-x fs-4 d-block mb-1"></i>
                    Chưa có sản phẩm nào.
                </td>
            </tr>
        `);
        calculateTotal();
        return;
    }
    let html = '';
    cart.forEach((item, index) => {
        let rowOrigin = item.quantity * item.price;
        let discountAmt = rowOrigin * (item.discount / 100);
        let afterDiscount = rowOrigin - discountAmt;
        let taxAmt = afterDiscount * (item.taxRate / 100);
        let rowFinalTotal = afterDiscount + taxAmt;
        html += `
            <tr>
                <td class="text-center align-middle">${index + 1}</td>

                <td class="align-middle">
                    <div class="fw-semibold">${item.productName}</div>
                </td>

                <td class="cart-input-td bg-info-subtle">
                    <input type="text"
                        value="${item.batchCode ?? ''}"
                        onchange="updateCart(${index}, 'batchCode', this.value)">
                </td>

                <td class="cart-input-td bg-info-subtle">
                    <input type="date"
                        value="${''}"
                        onchange="updateCart(${index}, 'expiryDate', this.value)">
                </td>

                <td class="cart-input-td bg-info-subtle">
                    <input type="text"
                        style="text-align:center"
                        value="${formatNumber(item.quantity)}"
                        oninput="handleNumberInput(this, ${index}, 'quantity')">
                </td>

                <td class="text-center align-middle">${item.unit}</td>

                <td class="cart-input-td bg-info-subtle">
                    <input type="text"
                        style="text-align:right"
                        value="${formatNumber(item.price)}"
                        oninput="handleNumberInput(this, ${index}, 'price')">
                </td>

                <td class="cart-input-td bg-info-subtle">
                    <input type="text"
                        style="text-align:center"
                        value="${formatNumber(item.discount)}"
                        oninput="handleNumberInput(this, ${index}, 'discount')">
                </td>

                <td class="text-end align-middle col-discount-amt">${discountAmt.toLocaleString()}</td>

                <td class="cart-input-td bg-info-subtle">
                    <input type="text"
                        style="text-align:center"
                        value="${formatNumber(item.taxRate)}"
                        oninput="handleNumberInput(this, ${index}, 'taxRate')">
                </td>

                <td class="text-end align-middle col-tax-amt">${taxAmt.toLocaleString()}</td>

                <td class="text-end align-middle fw-bold text-danger col-row-total">
                    ${rowFinalTotal.toLocaleString()}
                </td>

                <td class="text-center align-middle">
                    <button class="btn btn-sm btn-outline-danger"
                            onclick="removeFromCart(${index})">
                        <i class="bi bi-trash"></i>
                    </button>
                </td>
            </tr>`;
        });
    $('#cartBody').html(html);
    calculateTotal();
}

function updateCart(index, field, value) {
    if (
        field === 'quantity' ||
        field === 'price' ||
        field === 'discount' ||
        field === 'taxRate'
    ) {
        value = parseFloat(value) || 0;
    }
    cart[index][field] = value;

    // Chỉ re-render khi là field text (batchCode) — không re-render khi nhập số hoặc chọn ngày
    // vì sẽ làm mất focus
    if (field === 'quantity' || field === 'price' || field === 'discount' || field === 'taxRate') {
        updateRowCalculation(index);
    }
    // batchCode, expiryDate: chỉ lưu vào cart, không cần làm gì thêm
}

function updateRowCalculation(index) {
    const item = cart[index];
    let rowOrigin = item.quantity * item.price;
    let discountAmt = rowOrigin * (item.discount / 100);
    let afterDiscount = rowOrigin - discountAmt;
    let taxAmt = afterDiscount * (item.taxRate / 100);
    let rowFinalTotal = afterDiscount + taxAmt;

    const row = $('#cartBody tr').eq(index);
    row.find('.col-discount-amt').text(discountAmt.toLocaleString());
    row.find('.col-tax-amt').text(taxAmt.toLocaleString());
    row.find('.col-row-total').text(rowFinalTotal.toLocaleString());

    calculateTotal();
}

function removeFromCart(index) {
    cart.splice(index, 1);
    renderCart();
}

function calculateTotal() {
    console.log('cart:', JSON.stringify(cart));

    let subTotalOriginal = 0;
    let totalDiscount = 0;
    let totalTax = 0;
    let finalTotalAll = 0;

    cart.forEach(item => {
        let rowOrigin = item.quantity * item.price;
        subTotalOriginal += rowOrigin;
        let discountAmt = rowOrigin * (item.discount / 100);
        totalDiscount += discountAmt;
        let afterDiscount = rowOrigin - discountAmt;
        let taxAmt = afterDiscount * (item.taxRate / 100);
        totalTax += taxAmt;
        finalTotalAll += (afterDiscount + taxAmt);
    });
    $('#lblSubTotal').text(
        subTotalOriginal.toLocaleString() + ' đ'
    );
    $('#lblDiscountTotal').text(
        totalDiscount.toLocaleString() + ' đ'
    );
    $('#lblTaxTotal').text(
        totalTax.toLocaleString() + ' đ'
    );
    $('#lblFinalTotal').text(
        finalTotalAll.toLocaleString() + ' đ'
    );
}

// Format số để hiển thị trong value (dùng khi render html)
function formatNumber(value) {
    if (value == null || value === '') return '';
    return Number(value).toLocaleString('en-US');
}

// Xử lý khi người dùng nhập (dùng trong oninput)
function handleNumberInput(input, index, field) {
    let raw = input.value.replace(/,/g, '').replace(/\D/g, '');
    input.value = raw ? Number(raw).toLocaleString('en-US') : '';
    updateCart(index, field, raw ? Number(raw) : 0);
}

function submitImport() {
    if (cart.length === 0) { showToastWithBg("Phiếu nhập phải có ít nhất 1 sản phẩm!", 300, "red"); return; }
    if (!$('#supplierName').val()) { showToastWithBg("Vui lòng nhập tên nhà cung cấp!", 300, "red"); return; }

    let payload = {
        supplier: $('#supplierName').val(),
        details: cart.map(item => ({
            productId: item.productId,
            productName: item.productName,
            batchCode: item.batchCode ?? '',
            expiryDate: item.expiryDate ?? null,
            quantity: item.quantity,
            price: item.price,
            discount: item.discount,
            taxRate: item.taxRate,
            unit: item.unit,
        }))
    };

    $.ajax({
        url: '/api/inventory/import',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(payload),
        success: function (res) {
            showToastWithBg(res.message, 300, "green");
            modal.hide();
            cart = [];
            renderCart();
            $('#supplierName').val('');
            $('#cartBody').empty();
        },
        error: function (err) {
            showToastWithBg(err.responseText || "Lỗi khi lưu phiếu nhập!", 300, "red")
        }
    });
}

function applyHistoryFilter() {
    let params = {
        fromDate: $('#filterFromDate').val() || null,
        toDate: $('#filterToDate').val() || null,
        productId: $('#filterProductSelect').val() || 0,
        supplier: $('#filterSupplier').val() || '',
    };

    $.ajax({
        url: '/api/inventory/import/history',
        type: 'GET',
        data: params,
        success: function (res) {
            renderHistoryTable(res);
        },
        error: function (err) {
            showToastWithBg(err.responseText || "Lỗi khi tải lịch sử nhập kho!", 300, "red");
        }
    });
}

function renderHistoryTable(data) {
    let html = '';
    if (!data || data.length === 0) {
        html = `<tr><td colspan="8" class="text-center text-muted py-4">
                    <i class="bi bi-inbox fs-4 d-block mb-1"></i>Không có dữ liệu.
                </td></tr>`;
    } else {
        data.forEach((item, index) => {
            html += `
                <tr>
                    <td class="text-center">${index + 1}</td>
                    <td class="text-center">${item.batchCode ?? ''}</td>
                    <td class="text-center">${item.importDate ?? ''}</td>
                    <td>${item.staffName ?? ''}</td>
                    <td>${item.supplier ?? ''}</td>
                    <td class="text-end">${Number(item.totalAmount).toLocaleString()} đ</td>
                    <td class="text-center">
                        ${item.isCancelled
                            ? '<span class="badge bg-danger">Đã hủy</span>'
                            : '<span class="badge bg-success">Còn hiệu lực</span>'}
                    </td>
                    <td class="text-center">
                        <button class="btn btn-sm btn-outline-primary" onclick="viewImportDetail(${item.id})">
                            <i class="bi bi-eye"></i>
                        </button>
                    </td>
                </tr>`;
        });
    }
    $('#importHistoryTable').html(html);
}

function getTodayString() {
    const today = new Date();
    const yyyy = today.getFullYear();
    const mm = String(today.getMonth() + 1).padStart(2, '0');
    const dd = String(today.getDate()).padStart(2, '0');
    return `${yyyy}-${mm}-${dd}`; // → "2026-05-20"
}