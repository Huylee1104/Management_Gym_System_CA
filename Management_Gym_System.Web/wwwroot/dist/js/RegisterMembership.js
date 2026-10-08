/* =====================================================================
   Trang Đăng ký gói tập
   Yêu cầu: jQuery, Bootstrap 5 JS (biến toàn cục `bootstrap`), Tabler Icons
   HTML cần có: #gymPackages, #gpList, #gpListCol, #gpSide, #gpPanel
   ===================================================================== */
$(function () {

    /* =================================================================
       1. CẤU HÌNH
       ================================================================= */
    var API_URL = '/RegisterMembership/GetProducts';
    var DESKTOP_MIN_WIDTH = 992;        // trùng breakpoint "lg" của Bootstrap
    var MAX_VISIBLE_DESC_LINES = 3;     // số dòng mô tả hiển thị trước khi thu gọn
    var SCROLL_OFFSET = 80;             // chừa chỗ cho thanh menu khi cuộn tới item (điện thoại)

    /* =================================================================
       2. BIẾN TOÀN CỤC CỦA TRANG
       ================================================================= */
    var $root = $('#gymPackages');
    var $list = $('#gpList');           // vùng chứa các card gói tập
    var $listCol = $('#gpListCol');     // cột chứa danh sách (đổi col-12 <-> col-lg-6)
    var $side = $('#gpSide');           // cột chi tiết bên phải (laptop)
    var $panel = $('#gpPanel');         // khung chi tiết bên phải (laptop)

    var productsById = {};              // { "1": {...}, "2": {...} }
    var activeId = null;                // id gói đang mở, null = chưa mở gói nào
    var wasDesktop = isDesktop();       // dùng để biết khi nào đổi giữa laptop <-> điện thoại

    // Người dùng bật "giảm chuyển động" thì tắt toàn bộ animation của jQuery
    if (window.matchMedia('(prefers-reduced-motion: reduce)').matches) {
        $.fx.off = true;
    }

    /* =================================================================
       3. HÀM TIỆN ÍCH
       ================================================================= */
    function isDesktop() {
        return $(window).width() >= DESKTOP_MIN_WIDTH;
    }

    var HTML_ESCAPES = { '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#39;' };

    // Chống chèn mã HTML/JS từ dữ liệu
    function escapeHtml(value) {
        if (value === null || value === undefined) {
            return '';
        }
        return String(value).replace(/[&<>"']/g, function (ch) {
            return HTML_ESCAPES[ch];
        });
    }

    // Tách text thành từng dòng (bỏ dòng trống)
    function splitLines(text) {
        var result = [];
        var rawLines = String(text || '').split(/\r?\n/);
        for (var i = 0; i < rawLines.length; i++) {
            var line = $.trim(rawLines[i]);
            if (line !== '') {
                result.push(line);
            }
        }
        return result;
    }

    function getFirstLine(text) {
        var lines = splitLines(text);
        if (lines.length > 0) {
            return lines[0];
        }
        return '';
    }

    function formatMoney(value) {
        if (value === null || value === undefined) {
            return 'Liên hệ';
        }
        return new Intl.NumberFormat('vi-VN').format(value) + 'đ';
    }

    // Số ngày -> "x tháng" (ước lượng 1 tháng = 30 ngày)
    function formatDuration(days) {
        var months = Math.round(days / 30);
        if (months < 1) {
            return days + ' ngày';
        }
        return months + ' tháng';
    }

    // ImageProduct là base64: nếu chưa có tiền tố "data:" thì tự thêm vào
    function buildImageSrc(image) {
        if (!image) {
            return '';
        }
        if (/^(data:|https?:|\/)/.test(image)) {
            return image;
        }
        return 'data:image/jpeg;base64,' + image;
    }

    // Vẽ n sao vàng + (5 - n) sao xám
    function buildStars(rating) {
        var filled = '★'.repeat(rating);
        var empty = '★'.repeat(5 - rating);
        return '<span class="text-warning">' + filled + '</span>' +
               '<span class="text-secondary opacity-50">' + empty + '</span>';
    }

    // Controller có thể trả camelCase (productName) hoặc PascalCase (ProductName)
    function pick(item, camelName, pascalName) {
        if (item[camelName] !== undefined) {
            return item[camelName];
        }
        return item[pascalName];
    }

    // Chuẩn hóa 1 gói từ ProductDto
    function normalizeProduct(item) {
        return {
            id: String(pick(item, 'id', 'Id')),
            name: pick(item, 'productName', 'ProductName') || '',
            price: pick(item, 'price', 'Price'),
            days: pick(item, 'thoiHan', 'ThoiHan'),
            image: pick(item, 'imageProduct', 'ImageProduct'),
            description: pick(item, 'description', 'Description') || '',
            review: pick(item, 'review', 'Review') || ''
        };
    }

    // Tách chuỗi Review: "⭐⭐⭐⭐⭐ Nội dung" -> { rating: 5, text: "Nội dung" }
    function parseReviews(reviewText) {
        var lines = splitLines(reviewText);
        var reviews = [];

        for (var i = 0; i < lines.length; i++) {
            var match = lines[i].match(/^((?:⭐\uFE0F?|★)*)\s*(.*)$/u);
            var starIcons = match[1];
            var text = match[2];

            var starMatches = starIcons.match(/⭐|★/gu);
            var rating = 0;
            if (starMatches) {
                rating = starMatches.length;
            }

            if (text !== '') {
                reviews.push({ rating: rating, text: text });
            }
        }
        return reviews;
    }

    /* =================================================================
       4. TẠO HTML
       ================================================================= */
    function buildDurationBadge(days) {
        if (!days) {
            return '';
        }
        return `<span class="badge rounded-pill text-bg-light border fw-semibold">
                    <i class="ti ti-clock text-primary"></i> ${formatDuration(days)}
                </span>`;
    }

    // Card hiển thị trong danh sách
    function buildCard(product) {
        var id = escapeHtml(product.id);
        var name = escapeHtml(product.name || "");
        var imageSrc = escapeHtml(buildImageSrc(product.image));
        var price = formatMoney(product.price);
        var durationBadge = buildDurationBadge(product.days);
        var firstLine = escapeHtml(getFirstLine(product.description));

        return `
        <div class="col" data-id="${id}">
            <div class="card h-100 shadow-sm gp-card">
                <div role="button" tabindex="0" data-action="toggle">
                    <div class="ratio ratio-4x3">
                        <img class="object-fit-cover rounded-top" src="${imageSrc}" alt="${name}">
                    </div>
                    <div class="card-body">
                        <h5 class="fw-bold">${name}</h5>
                        <div class="d-flex justify-content-between align-items-center mb-2">
                            <span class="fs-4 fw-bold text-primary">${price}</span>
                            ${durationBadge}
                        </div>
                        <p class="text-secondary text-truncate mb-0">${firstLine}</p>
                    </div>
                </div>
                <div class="collapse border-top"></div>
            </div>
        </div>`;
    }

    // Một dòng mô tả
    function buildDescriptionItem(line) {
        return `<li class="d-flex gap-2 mb-2">
                    <i class="ti ti-circle-check text-primary mt-1"></i>
                    <span>${escapeHtml(line)}</span>
                </li>`;
    }

    function buildDescriptionList(lines) {
        var items = '';
        for (var i = 0; i < lines.length; i++) {
            items += buildDescriptionItem(lines[i]);
        }
        return '<ul class="list-unstyled mb-1">' + items + '</ul>';
    }

    // Mô tả: hiện MAX_VISIBLE_DESC_LINES dòng đầu, phần còn lại thu gọn
    function buildDescription(description) {
        var lines = splitLines(description);
        var visibleLines = lines.slice(0, MAX_VISIBLE_DESC_LINES);
        var hiddenLines = lines.slice(MAX_VISIBLE_DESC_LINES);

        var html = buildDescriptionList(visibleLines);

        if (hiddenLines.length > 0) {
            html += `
                <div class="collapse">${buildDescriptionList(hiddenLines)}</div>
                <button type="button" class="btn btn-link text-primary p-0" data-action="more">
                    <span>Xem thêm</span><span class="d-none">Thu gọn</span>
                </button>`;
        }
        return html;
    }

    // Một đánh giá
    function buildReviewItem(review) {
        var starsHtml = '';
        if (review.rating > 0) {
            starsHtml = buildStars(review.rating);
        }
        return `
        <li class="list-group-item d-flex gap-3 px-0">
            <span class="rounded-circle bg-light p-2 align-self-start d-inline-flex">
                <i class="ti ti-user fs-4 text-secondary"></i>
            </span>
            <div>
                <div class="fw-semibold">Hội viên <span class="ms-1 small">${starsHtml}</span></div>
                <div>${escapeHtml(review.text)}</div>
            </div>
        </li>`;
    }

    // Khung nhập đánh giá: chọn sao + ô nhập + nút gửi
    function buildChatBox() {
        var starButtons = '';
        for (var n = 1; n <= 5; n++) {
            starButtons += `<button type="button"
                                class="btn btn-link p-0 fs-3 lh-1 text-decoration-none text-secondary"
                                data-action="star" data-value="${n}">★</button>`;
        }
        return `
        <div class="bg-light rounded-3 p-3" data-chat data-rating="0">
            <div class="d-flex align-items-center gap-2 mb-2">
                <span class="small text-secondary me-1">Chọn số sao:</span>
                ${starButtons}
            </div>
            <div class="input-group gap-2">
                <input type="text" class="form-control" maxlength="300" placeholder="Chia sẻ cảm nhận của bạn...">
                <button type="button" class="btn btn-primary" data-action="send" aria-label="Gửi">
                    <i class="ti ti-send"></i>
                </button>
            </div>
        </div>`;
    }

    // Dòng "★★★★★ 5.0 (5)" cạnh tiêu đề Đánh giá
    function buildReviewSummary(reviews) {
        var total = 0;
        var count = 0;
        for (var i = 0; i < reviews.length; i++) {
            if (reviews[i].rating > 0) {
                total += reviews[i].rating;
                count++;
            }
        }
        if (count === 0) {
            return '';
        }
        var average = total / count;
        return '<span>' + buildStars(Math.round(average)) + ' <b>' + average.toFixed(1) + '</b> (' + reviews.length + ')</span>';
    }

    // Chi tiết gói. withImage = true: laptop (có ảnh + nút X). false: điện thoại (item đã có ảnh)
    function buildDetail(product, withImage) {
        var reviews = parseReviews(product.review);
        var name = escapeHtml(product.name || "");

        var imageHtml = '';
        if (withImage) {
            imageHtml = `
            <div class="position-relative">
                <div class="ratio ratio-16x9">
                    <img class="object-fit-cover" src="${escapeHtml(buildImageSrc(product.image))}" alt="${name}">
                </div>
                <button type="button" class="btn-close position-absolute top-0 end-0 m-3 p-2 bg-white rounded-circle shadow"
                        data-action="close" aria-label="Đóng"></button>
            </div>`;
        }

        var reviewsHtml = '';
        if (reviews.length === 0) {
            reviewsHtml = '<li class="list-group-item text-center text-secondary">Chưa có đánh giá nào.</li>';
        } else {
            for (var i = 0; i < reviews.length; i++) {
                reviewsHtml += buildReviewItem(reviews[i]);
            }
        }

        return `
        ${imageHtml}
        <div class="card-body">
            <h3 class="h4 fw-bold">${name}</h3>
            <div class="d-flex justify-content-between align-items-center mb-3">
                <span class="fs-3 fw-bold text-primary">${formatMoney(product.price)}</span>
                ${buildDurationBadge(product.days)}
            </div>

            ${buildDescription(product.description)}

            <div class="d-flex gap-2 my-3">
                <button type="button" class="btn btn-outline-primary flex-fill" data-action="consult">
                    <i class="ti ti-messages"></i> Tư vấn thêm
                </button>
                <button type="button" class="btn btn-warning flex-fill" data-action="pay">
                    <i class="ti ti-credit-card"></i> Thanh toán ngay
                </button>
            </div>
            <hr>

            <div class="d-flex justify-content-between align-items-center mb-2">
                <h4 class="h5 fw-bold mb-0">Đánh giá từ hội viên</h4>
                ${buildReviewSummary(reviews)}
            </div>
            ${buildChatBox()}
            <ul class="list-group list-group-flush mt-2">${reviewsHtml}</ul>
        </div>`;
    }

    /* =================================================================
       5. BỐ CỤC & HIỆU ỨNG
       ================================================================= */

    // Bật/tắt vùng collapse của Bootstrap ($box là jQuery object)
    function setCollapse($box, show) {
        var instance = bootstrap.Collapse.getOrCreateInstance($box[0], { toggle: false });
        if (show) {
            instance.show();
        } else {
            instance.hide();
        }
    }

    // Vùng chi tiết nằm dưới item (dùng cho điện thoại)
    function getInlineBox(id) {
        return $list.find('[data-id="' + id + '"] > .card > .collapse');
    }

    // Laptop: true = danh sách 2 cột bên trái + chi tiết bên phải; false = danh sách 4 cột
    function showSidePanel(show) {
        if (show) {
            $listCol.addClass('col-lg-6');
            $list.removeClass('row-cols-lg-4').addClass('row-cols-lg-2');
            $side.removeClass('d-none');
        } else {
            $listCol.removeClass('col-lg-6');
            $list.removeClass('row-cols-lg-2').addClass('row-cols-lg-4');
            $side.addClass('d-none');
        }
    }

    // Đánh dấu gói đang mở:
    // - laptop: ẩn khỏi danh sách (vì đã "nhảy" sang bên phải)
    // - điện thoại: giữ lại và viền màu cam
    function markActiveCard() {
        $list.children('[data-id]').each(function () {
            var $col = $(this);
            var $card = $col.children('.card');

            if ($col.attr('data-id') === activeId) {
                $col.addClass('d-lg-none');
                $card.addClass('border-primary');
            } else {
                $col.removeClass('d-lg-none');
                $card.removeClass('border-primary');
            }
        });
    }

    // Các card trượt từ vị trí lệch fromX (px) về vị trí gốc
    function slideIn($elements, fromX) {
        $elements.css({ position: 'relative', left: fromX, opacity: 0 });
        $elements.animate({ left: 0, opacity: 1 }, 350, function () {
            $(this).css({ position: '', left: '', opacity: '' });
        });
    }

    // Khung chi tiết hiện dần
    function fadeInPanel() {
        $panel.css('opacity', 0).animate({ opacity: 1 }, 300, function () {
            $panel.css('opacity', '');
        });
    }

    /* =================================================================
       6. MỞ / ĐÓNG GÓI
       ================================================================= */
    function openPackage(id) {
        id = String(id);
        var product = productsById[id];

        if (!product || id === activeId) {
            return;
        }

        var wasAnyOpen = (activeId !== null);

        // Điện thoại: chỉ cho mở 1 item, nên đóng item đang mở trước
        if (!isDesktop() && wasAnyOpen) {
            setCollapse(getInlineBox(activeId), false);
        }

        activeId = id;
        markActiveCard();

        if (isDesktop()) {
            openOnDesktop(product, wasAnyOpen);
        } else {
            openOnMobile(product);
        }
    }

    function openOnDesktop(product, wasAnyOpen) {
        $panel.html(buildDetail(product, true));
        $panel.scrollTop(0);
        showSidePanel(true);
        fadeInPanel();

        // Lần mở đầu tiên: các gói còn lại trượt sang trái
        if (!wasAnyOpen) {
            slideIn($list.children(), 40);
        }
    }

    function openOnMobile(product) {
        var $box = getInlineBox(product.id);
        $box.html(buildDetail(product, false));

        // Mở xong thì cuộn tới item. Dùng addEventListener gốc vì sự kiện của Bootstrap 5 là native event
        $box[0].addEventListener('shown.bs.collapse', function () {
            var $card = $list.find('[data-id="' + product.id + '"]');
            $('html, body').animate({ scrollTop: $card.offset().top - SCROLL_OFFSET }, 300);
        }, { once: true });

        setCollapse($box, true);
    }

    function closePackage() {
        if (activeId === null) {
            return;
        }

        var closingId = activeId;
        activeId = null;
        markActiveCard();

        if (isDesktop()) {
            showSidePanel(false);
            $panel.empty();
            slideIn($list.children(), -40);     // các gói trượt về chỗ cũ
        } else {
            setCollapse(getInlineBox(closingId), false);
        }
    }

    // Khi đổi giữa laptop <-> điện thoại (xoay máy, kéo cửa sổ): dựng lại gói đang mở theo chế độ mới
    function rebuildAfterBreakpointChange() {
        var reopenId = activeId;

        activeId = null;
        markActiveCard();
        showSidePanel(false);
        $panel.empty();
        $list.find('.collapse.show').removeClass('show');

        if (reopenId !== null) {
            openPackage(reopenId);
        }
    }

    /* =================================================================
       7. HÀNH ĐỘNG CỦA NÚT  -->  BẠN TỰ XỬ LÝ Ở ĐÂY
       ================================================================= */
    function onConsultClick(product) {
        // TODO: xử lý nút "Tư vấn thêm"
        console.log('Tư vấn thêm:', product);
    }

    function onPayClick(product) {
        // TODO: xử lý nút "Thanh toán ngay"
        console.log('Thanh toán:', product);
    }

    function onSubmitReview(product, rating, content) {
        // TODO: gọi ajax lưu đánh giá
        console.log('Gửi đánh giá:', product.id, rating, content);
    }

    /* =================================================================
       8. GẮN SỰ KIỆN (event delegation: nội dung được render động)
       ================================================================= */

    // Bấm vào card: mở, hoặc đóng nếu đang mở chính nó
    $root.on('click', '[data-action="toggle"]', function () {
        var id = $(this).closest('[data-id]').attr('data-id');   // dùng attr để giữ kiểu chuỗi
        if (id === activeId) {
            closePackage();
        } else {
            openPackage(id);
        }
    });

    // Enter / Space khi card đang được focus
    $root.on('keydown', '[data-action="toggle"]', function (e) {
        if (e.which === 13 || e.which === 32) {
            e.preventDefault();
            $(this).trigger('click');
        }
    });

    // Nút X
    $root.on('click', '[data-action="close"]', function () {
        closePackage();
    });

    // Phím ESC
    $(document).on('keydown', function (e) {
        if (e.which === 27) {
            closePackage();
        }
    });

    // Xem thêm / Thu gọn mô tả
    $root.on('click', '[data-action="more"]', function () {
        var $button = $(this);
        var $box = $button.prev('.collapse');

        if ($box.hasClass('collapsing')) {
            return;     // đang chạy hiệu ứng thì bỏ qua
        }

        setCollapse($box, !$box.hasClass('show'));
        $button.find('span').toggleClass('d-none');
    });

    // Chọn số sao
    $root.on('click', '[data-action="star"]', function () {
        var $chat = $(this).closest('[data-chat]');
        var selected = parseInt($(this).attr('data-value'), 10);

        $chat.attr('data-rating', selected);

        $chat.find('[data-action="star"]').each(function () {
            var value = parseInt($(this).attr('data-value'), 10);
            if (value <= selected) {
                $(this).addClass('text-warning').removeClass('text-secondary');
            } else {
                $(this).addClass('text-secondary').removeClass('text-warning');
            }
        });
    });

    // Enter trong ô nhập đánh giá = bấm nút gửi
    $root.on('keydown', '[data-chat] input', function (e) {
        if (e.which === 13) {
            e.preventDefault();
            $(this).closest('[data-chat]').find('[data-action="send"]').trigger('click');
        }
    });

    // Nút gửi đánh giá
    $root.on('click', '[data-action="send"]', function () {
        var $chat = $(this).closest('[data-chat]');
        var rating = parseInt($chat.attr('data-rating'), 10);
        var content = $.trim($chat.find('input').val());

        onSubmitReview(productsById[activeId], rating, content);
    });

    // Nút Tư vấn thêm / Thanh toán ngay
    $root.on('click', '[data-action="consult"]', function () {
        onConsultClick(productsById[activeId]);
    });

    $root.on('click', '[data-action="pay"]', function () {
        onPayClick(productsById[activeId]);
    });

    // Đổi kích thước cửa sổ
    $(window).on('resize', function () {
        var nowDesktop = isDesktop();
        if (nowDesktop === wasDesktop) {
            return;
        }
        wasDesktop = nowDesktop;
        rebuildAfterBreakpointChange();
    });

    /* =================================================================
       9. LẤY DỮ LIỆU & HIỂN THỊ DANH SÁCH
       ================================================================= */
    function showListMessage(message) {
        $list.html('<div class="col-12 text-center text-secondary py-5">' + escapeHtml(message) + '</div>');
    }

    function renderProducts(items) {
        productsById = {};
        activeId = null;
        showSidePanel(false);
        $panel.empty();

        if (!items || items.length === 0) {
            showListMessage('Hiện chưa có gói tập nào.');
            return;
        }

        var html = '';
        $.each(items, function (index, item) {
            // Muốn ẩn gói ngừng bán thì bỏ comment 3 dòng dưới:
            if (item.status === false) {
                return;
            }
            var product = normalizeProduct(item);
            productsById[product.id] = product;
            html += buildCard(product);
        });

        $list.html(html);
    }

    function loadProducts() {
        LoadingData();
        $.ajax({
            url: API_URL,
            type: 'POST',
            dataType: 'json',
            success: function (response) {
                renderProducts(response);
                HidenLoadingData();
            },
            error: function (xhr, status, error) {
                console.error('Không tải được danh sách gói tập:', status, error);
                showToast('Không tải được danh sách gói tập. Vui lòng thử lại sau.', 500);
                HidenLoadingData();
            }
        });
    }

    /* =================================================================
       10. KHỞI CHẠY
       ================================================================= */
    loadProducts();
});