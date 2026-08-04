//Config chọn ngày
function configDate() {
    $(".input-date").datetimepicker({
        locale: "vi",
        format: "DD-MM-yyyy",
        useStrict: true,
        widgetPositioning: {
            horizontal: "auto",
            vertical: "bottom",
        },
        extraFormats: ["DD-MM-yyyy", "DD/MM/yyyy", "yyyy"],
        icons: {
            date: "ti ti-calendar",
            up: "ti ti-chevron-up",
            down: "ti ti-chevron-down",
            previous: "ti ti-chevron-left",
            next: "ti ti-chevron-right",
            time: "ti ti-alarm",
            close: 'ti ti-x'
        },
        keyBinds: {
            left: null,
            right: null,
        },
        showClose: true
    });
}
function configDateDefault() {
    var today = new Date();
    $(".input-date-default").datetimepicker({
        locale: "vi",
        useStrict: true,
        defaultDate: today,
        format: "DD-MM-yyyy",
        extraFormats: ["DD-MM-yyyy", "DD/MM/yyyy", "yyyy"],
        icons: {
            date: "ti ti-calendar",
            up: "ti ti-chevron-up",
            down: "ti ti-chevron-down",
            previous: "ti ti-chevron-left",
            next: "ti ti-chevron-right",
            time: "ti ti-alarm",
            close: 'ti ti-x'
        },
        keyBinds: {
            left: null,
            right: null,
        },
        showClose: true
    });
}
//Hiện toast từ layout
function showToast(message, statusCode) {
    var toast =
        $(`<div class="toast mb-2" role="alert" aria-live="assertive" aria-atomic="true" data-bs-delay="${statusCode == 200 ? "1250" : "2500"}">
            <div class="alert alert-important ${statusCode == 200 ? "alert-success" : "alert-danger"
            } alert-dismissible mb-0" role="alert">
                <div class="d-flex">
                    <div>
                        ${statusCode == 200
                ? `<svg xmlns="http://www.w3.org/2000/svg" class="icon alert-icon" width="24" height="24" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round"><path stroke="none" d="M0 0h24v24H0z" fill="none"></path><path d="M5 12l5 5l10 -10"></path></svg>`
                : `<svg xmlns="http://www.w3.org/2000/svg" class="icon alert-icon" width="24" height="24" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round"><path stroke="none" d="M0 0h24v24H0z" fill="none"></path><path d="M3 12a9 9 0 1 0 18 0a9 9 0 0 0 -18 0"></path><path d="M12 8v4"></path><path d="M12 16h.01"></path></svg>`
            }
                    </div>
                    <div>
                        ${message}
                    </div>
                </div>
                <a class="btn-close" data-bs-dismiss="toast" aria-label="close"></a>
            </div>
        </div>`);

    // Append toast to container
    $("#toast-container").append(toast);

    // Initialize toast
    toast.toast("show");

    // Remove toast after it's closed
    toast.on("hidden.bs.toast", function () {
        $(this).remove();
    });
}
function showToastWithBg(message, statusCode, backgrounColor) {
    var toast =
        $(`<div class="toast mb-2" role="alert" aria-live="assertive" aria-atomic="true" data-bs-delay="${statusCode == 200 ? "1250" : "2500"}">
            <div class="alert alert-important ${backgrounColor} alert-dismissible mb-0" role="alert">
                <div class="d-flex">
                    <div>
                        ${statusCode == 200
                ? `<svg xmlns="http://www.w3.org/2000/svg" class="icon alert-icon" width="24" height="24" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round"><path stroke="none" d="M0 0h24v24H0z" fill="none"></path><path d="M5 12l5 5l10 -10"></path></svg>`
                : `<svg xmlns="http://www.w3.org/2000/svg" class="icon alert-icon" width="24" height="24" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round"><path stroke="none" d="M0 0h24v24H0z" fill="none"></path><path d="M3 12a9 9 0 1 0 18 0a9 9 0 0 0 -18 0"></path><path d="M12 8v4"></path><path d="M12 16h.01"></path></svg>`
            }
                    </div>
                    <div>
                        ${message}
                    </div>
                </div>
                <a class="btn-close" data-bs-dismiss="toast" aria-label="close"></a>
            </div>
        </div>`);

    // Append toast to container
    $("#toast-container").append(toast);

    // Initialize toast
    toast.toast("show");

    // Remove toast after it's closed
    toast.on("hidden.bs.toast", function () {
        $(this).remove();
    });
}
// Ẩn thông báo
function hideToast() {
    $("#toast").hide();
    $("#toast").removeClass("bg-success");
    $("#toast").removeClass("bg-danger");
}

var ALL_BG = [
  'bg-warning','bg-danger','bg-success','bg-info',
  'bg-primary','bg-secondary','bg-dark','bg-light'
];
var BTN_MAP = {
  'bg-warning':'btn-warning', 'bg-danger':'btn-danger',
  'bg-success':'btn-success', 'bg-info':'btn-info',
  'bg-primary':'btn-primary', 'bg-secondary':'btn-secondary',
  'bg-dark':'btn-dark',       'bg-light':'btn-light',
};
var COLOR_MAP = {
  'bg-warning':'#ffc107', 'bg-danger':'#dc3545',
  'bg-success':'#198754', 'bg-info':'#0dcaf0',
  'bg-primary':'#0d6efd', 'bg-secondary':'#6c757d',
  'bg-dark':'#212529',    'bg-light':'#6c757d',
};
var ICON_MAP = {
  'bg-warning':'bi-exclamation-circle-fill',
  'bg-danger' :'bi-x-octagon-fill',
  'bg-success':'bi-check-circle-fill',
  'bg-info'   :'bi-info-circle-fill',
  'bg-primary':'bi-bell-fill',
  'bg-secondary':'bi-info-circle-fill',
  'bg-dark'   :'bi-shield-fill',
  'bg-light'  :'bi-info-circle-fill',
};

// ---- Modal Thông Tin ----
function HienModalThongTin(tieuDe, bgClass) {
  var cls    = bgClass || 'bg-warning';
  var header = document.getElementById('modal-header');
  var icon   = document.getElementById('modal-icon');

  header.classList.remove(...ALL_BG);
  header.classList.add(cls);

  icon.className = 'bi ' + (ICON_MAP[cls] || 'bi-info-circle-fill');
  icon.style.color = COLOR_MAP[cls] || '#ffc107';

  document.getElementById('modal-title').textContent = tieuDe;
  document.getElementById('modal-overlay').style.display = 'flex';
}

function DongModal() {
  document.getElementById('modal-overlay').style.display = 'none';
}

// ---- Modal Xác Nhận ----
var _callbackXacNhan = null;

function HienModalXacNhan(tieuDe, bgClass, callback) {
  var cls        = bgClass || 'bg-warning';
  var header     = document.getElementById('modal-xn-header');
  var icon       = document.getElementById('modal-xn-icon');
  var btnConfirm = document.getElementById('modal-xn-confirm');

  header.classList.remove(...ALL_BG);
  header.classList.add(cls);

  icon.className = 'bi ' + (ICON_MAP[cls] || 'bi-question-circle-fill');
  icon.style.color = COLOR_MAP[cls] || '#ffc107';

  btnConfirm.classList.remove(...Object.values(BTN_MAP));
  btnConfirm.classList.add(BTN_MAP[cls] || 'btn-warning');

  document.getElementById('modal-xn-title').textContent = tieuDe;
  _callbackXacNhan = callback || null;
  document.getElementById('modal-xn-overlay').style.display = 'flex';
}

function DongModalXacNhan() {
  document.getElementById('modal-xn-overlay').style.display = 'none';
}

function XacNhanModal() {
  document.getElementById('modal-xn-overlay').style.display = 'none';
  if (_callbackXacNhan) _callbackXacNhan();
}

// Cách dùng
//HienModalThongTin('Lưu thành công', 'bg-success')


// cách dùng
// HienModalXacNhan('Xóa dữ liệu?', 'bg-danger', function() {
//   XoaDuLieu();
// });