'use strict';

const YEAR_RANGE = 3;

const today = new Date();

// Trạng thái của trang
const state = {
    year: today.getFullYear(),
    month: today.getMonth(),
    schedule: null,
    popovers: []
};

// Các phần tử DOM + cấu hình đọc từ data-*. Chỉ được gán trong DOMContentLoaded
const dom = {};
document.addEventListener('DOMContentLoaded', function () {
    cacheDom();
    init();
});

// Chỉ những việc thật sự cần DOM mới nằm ở đây
function cacheDom() {
    dom.container = document.getElementById('memberCalendar');
    dom.calendarTitle = document.getElementById('calendarTitle');
    dom.calendarDays = document.getElementById('calendarDays');
    dom.selectMonth = document.getElementById('selectMonth');
    dom.selectYear = document.getElementById('selectYear');
    dom.btnPrevMonth = document.getElementById('btnPrevMonth');
    dom.btnNextMonth = document.getElementById('btnNextMonth');}

// Date -> "yyyy-MM-dd"
function toDateKey(date) {
    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const day = String(date.getDate()).padStart(2, '0');
    return year + '-' + month + '-' + day;
}

// Chuỗi ngày từ API ("2026-10-11T00:00:00") -> "2026-10-11". Rỗng -> null
function apiDateToKey(apiDate) {
    return apiDate ? apiDate.substring(0, 10) : null;
}

// "2026-10-11" -> "11/10/2026"
function formatDateKey(dateKey) {
    const parts = dateKey.split('-');
    return parts[2] + '/' + parts[1] + '/' + parts[0];
}

function loadSchedule() {
    return $.ajax({
        url: '/WorkoutSchedule/GetCalendar',
        type: 'POST',
        dataType: 'json'
    });
}

// Dữ liệu mẫu để chạy thử giao diện (JSON mặc định của ASP.NET là camelCase)
// function getMockSchedule() {
//     return {
//         id: 1,
//         startDate: '2026-09-20T00:00:00',
//         endDate: '2026-11-20T00:00:00',
//         pauseDate: '2026-10-14T00:00:00',
//         resumeDate: '2026-10-20T00:00:00',
//         checkinTime: '2026-10-10T18:30:00',
//         workoutNoteDetails: [
//             {
//                 memberNote: 'Hôm nay đau vai trái nên chỉ tập chân.',
//                 memberNoteDate: '2026-10-08T00:00:00',
//                 trainerNote: 'Giảm tạ bài đẩy vai, tập thêm giãn cơ.',
//                 trainerNoteDate: '2026-10-08T00:00:00'
//             },
//             {
//                 memberNote: null,
//                 memberNoteDate: null,
//                 trainerNote: 'Đã tăng 5kg bài squat.\nGiữ nhịp độ này tuần sau.',
//                 trainerNoteDate: '2026-10-10T00:00:00'
//             }
//         ]
//     };
// }

// Chuyển dữ liệu API thành dạng dễ dùng cho việc vẽ lịch
function buildScheduleView(dto) {
const view = {
    startKey: apiDateToKey(dto.startDate),
    endKey: apiDateToKey(dto.endDate),
    pauseKey: apiDateToKey(dto.pauseDate),
    resumeKey: apiDateToKey(dto.resumeDate),
    checkinKeys: new Set((dto.checkinTime || []).map(apiDateToKey).filter(Boolean)),
    notesByDate: {}
};

    const noteDetails = dto.workoutNoteDetails || [];
    noteDetails.forEach(function (detail) {
        if (detail.memberNote && detail.memberNoteDate) {
            addNote(view, apiDateToKey(detail.memberNoteDate), 'member', detail.memberNote);
        }
        if (detail.trainerNote && detail.trainerNoteDate) {
            addNote(view, apiDateToKey(detail.trainerNoteDate), 'trainer', detail.trainerNote);
        }
    });

    return view;
}

function addNote(view, dateKey, type, text) {
    if (!view.notesByDate[dateKey]) {
        view.notesByDate[dateKey] = [];
    }
    view.notesByDate[dateKey].push({ type: type, text: text });
}

// Ngày nằm trong thời gian của gói tập (StartDate -> EndDate)
function isInPackage(dateKey) {
    const s = state.schedule;
    if (!s.startKey || dateKey < s.startKey) {
        return false;
    }
    if (s.endKey && dateKey > s.endKey) {
        return false;
    }
    return true;
}

function isPaused(dateKey) {
    const s = state.schedule;
    if (!s.pauseKey || dateKey < s.pauseKey) {
        return false;
    }
    if (!isInPackage(dateKey)) {
        return false;
    }
    if (s.resumeKey) {
        return dateKey < s.resumeKey;
    }
    return true;
}

// Các nhãn nhỏ hiển thị trong ô ngày
function getDayLabels(dateKey) {
    const s = state.schedule;
    const labels = [];

    if (dateKey === s.startKey) labels.push('Bắt đầu');
    if (dateKey === s.pauseKey) labels.push('Tạm dừng');
    if (dateKey === s.resumeKey) labels.push('Tập lại');
    if (dateKey === s.endKey) labels.push('Hết hạn');
if (state.schedule.checkinKeys.has(dateKey)) labels.push('✓ Check-in');
    return labels;
}

function renderCalendar() {
    destroyPopovers();

    dom.calendarTitle.textContent = 'Tháng ' + (state.month + 1) + ' - ' + state.year;
setSelectValue(dom.selectMonth, state.month);
setSelectValue(dom.selectYear, state.year);

    dom.calendarDays.innerHTML = '';

    // Tuần bắt đầu từ thứ hai: Chủ nhật (getDay() = 0) phải nằm cuối tuần
    const firstDayOfMonth = new Date(state.year, state.month, 1);
    const offset = (firstDayOfMonth.getDay() + 6) % 7;
    const daysInMonth = new Date(state.year, state.month + 1, 0).getDate();
    const totalCells = Math.ceil((offset + daysInMonth) / 7) * 7;

    for (let i = 0; i < totalCells; i++) {
        // Date(year, month, 1 - offset + i) tự tràn sang tháng trước / tháng sau
        const date = new Date(state.year, state.month, 1 - offset + i);
        dom.calendarDays.appendChild(createDayCell(date));
    }
}

function createDayCell(date) {
    const dateKey = toDateKey(date);
    const isOtherMonth = date.getMonth() !== state.month;
    const isSunday = date.getDay() === 0;

    const cell = document.createElement('div');
    cell.className = 'calendar-cell border-end border-bottom p-1';

    // Màu nền theo trạng thái
    if (isPaused(dateKey)) {
        cell.classList.add('bg-gym-paused');
    } else if (isInPackage(dateKey)) {
        cell.classList.add('bg-gym-active');
    }

    if (dateKey === toDateKey(today)) {
        cell.classList.add('calendar-today');
    }

    // Số ngày
    const dayNumber = document.createElement('div');
    dayNumber.className = 'calendar-day-number fw-bold';
    dayNumber.textContent = date.getDate();
    if (isOtherMonth) {
        dayNumber.classList.add('text-black-50');
    } else if (isSunday) {
        dayNumber.classList.add('text-gym-danger');
    }
    cell.appendChild(dayNumber);

    // Nhãn nhỏ (Bắt đầu, Hết hạn, ...)
    getDayLabels(dateKey).forEach(function (labelText) {
        const label = document.createElement('div');
        label.className = 'calendar-label text-muted text-truncate';
        label.textContent = labelText;
        cell.appendChild(label);
    });

    // Ghi chú
    const notes = state.schedule.notesByDate[dateKey] || [];
    notes.forEach(function (note) {
        cell.appendChild(createNoteChip(note, dateKey));
    });

    return cell;
}

// Ghi chú rút gọn trong ô ngày. Bấm vào thì hiện popover đầy đủ
function createNoteChip(note, dateKey) {
    const isMember = note.type === 'member';
    const prefix = isMember ? 'HV' : 'HLV';

    // Dùng <a role="button" tabindex="0"> để popover đóng được khi bấm ra ngoài (kể cả Safari)
    const chip = document.createElement('a');
    chip.className = 'note-chip badge text-start text-truncate mt-1 '
        + (isMember ? 'note-chip-member' : 'note-chip-trainer');
    chip.setAttribute('role', 'button');
    chip.setAttribute('tabindex', '0');

    // Mobile (< sm): chỉ hiện "HV" / "HLV". Từ sm trở lên: hiện thêm nội dung rút gọn
    const prefixSpan = document.createElement('span');
    prefixSpan.textContent = prefix;
    const textSpan = document.createElement('span');
    textSpan.className = 'd-none d-sm-inline';
    textSpan.textContent = ': ' + note.text;
    chip.appendChild(prefixSpan);
    chip.appendChild(textSpan);

    // Nội dung popover: dùng textContent để tránh chèn HTML từ ghi chú
    const content = document.createElement('div');
    content.className = 'note-popover-content';
    content.textContent = note.text;

    const title = (isMember ? 'Ghi chú của hội viên' : 'Ghi chú của huấn luyện viên')
        + ' - ' + formatDateKey(dateKey);

    const popover = new bootstrap.Popover(chip, {
        container: 'body',
        trigger: 'focus',
        html: true,
        title: title,
        content: content
    });
    state.popovers.push(popover);

    return chip;
}

function destroyPopovers() {
    state.popovers.forEach(function (popover) {
        popover.dispose();
    });
    state.popovers = [];
}

function fillMonthAndYearOptions() {
    for (let m = 0; m < 12; m++) {
        dom.selectMonth.add(new Option('Tháng ' + (m + 1), m));
    }

    const currentYear = today.getFullYear();
    for (let y = currentYear - YEAR_RANGE; y <= currentYear + YEAR_RANGE; y++) {
        dom.selectYear.add(new Option(y, y));
    }
}

function goToPrevMonth() {
    state.month--;
    if (state.month < 0) {
        state.month = 11;
        state.year--;
    }
    renderCalendar();
}

function goToNextMonth() {
    state.month++;
    if (state.month > 11) {
        state.month = 0;
        state.year++;
    }
    renderCalendar();
}

function onSelectChanged() {
    state.month = parseInt(dom.selectMonth.value, 10);
    state.year = parseInt(dom.selectYear.value, 10);
    renderCalendar();
}

async function init() {
    fillMonthAndYearOptions();
initTomSelect(dom.selectMonth);
initTomSelect(dom.selectYear);

    dom.btnPrevMonth.addEventListener('click', goToPrevMonth);
    dom.btnNextMonth.addEventListener('click', goToNextMonth);
    dom.selectMonth.addEventListener('change', onSelectChanged);
    dom.selectYear.addEventListener('change', onSelectChanged);

    try {
        const dto = await loadSchedule();
        state.schedule = buildScheduleView(dto || {});
    } catch (error) {
        console.error('Không tải được lịch tập:', error);
        showToast('Không tải được lịch tập. Vui lòng thử lại sau.', 500);
        state.schedule = buildScheduleView({});
    }

    renderCalendar();
}

function initTomSelect(select) {
    if (select.tomselect) {
        select.tomselect.destroy();
    }
    return new TomSelect(select, {
        controlInput: null,       // không cho gõ tìm kiếm, chỉ chọn như select thường
        dropdownParent: 'body'    // tránh bị cắt bởi overflow của card
    });
}

// Gán giá trị cho select, có tính đến Tom Select
function setSelectValue(select, value) {
    if (select.tomselect) {
        select.tomselect.setValue(String(value), true);   // true = không bắn lại sự kiện change
    } else {
        select.value = value;
    }
}