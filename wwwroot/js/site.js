(function ($) {
    $.fn.serializeFormJSON = function () {

        var o = {};
        var a = this.serializeArray();
        $.each(a, function () {
            if (o[this.name]) {
                if (!o[this.name].push) {
                    o[this.name] = [o[this.name]];
                }
                o[this.name].push(this.value || '');
            } else {
                o[this.name] = this.value || '';
            }
        });
        return o;
    };
})(jQuery)

function LoadDataTable(TableID) {
    $(`#${TableID}`).DataTable().ajax.reload();
}

$(document).on('hidden.bs.modal', '.modal', function () {
    if ($('.modal:visible').length)
        $('body').addClass('modal-open')
    else
        $('body').removeClass('modal-open')
})

$(document).ready(function () {
    $('.select2').select2({
        placeholder: '----- เลือกรายการ -----',
        theme: 'bootstrap4',
    });
    $('.select2m').select2({
        placeholder: '----- เลือกรายการ -----',
        width: '100%',
        multiple: true,
        theme: 'bootstrap4',
    })
    $('.DateNowTH').html(DateThai(+new Date()))
});

function DateThai(date, short = false, formatHHMMSS = false) {

    if (date == null || date == '0001-01-01T00:00:00' || !date || date == '') return '-'

    const _date = new Date(date)
    if (formatHHMMSS) {
        return _date.toLocaleDateString('th-TH', {
            year: 'numeric',
            month: !short ? 'long' : 'short',
            day: 'numeric',
            hour: 'numeric',
            minute: 'numeric',
            second: 'numeric',
        })
    }
    return _date.toLocaleDateString('th-TH', {
        year: 'numeric',
        month: !short ? 'long' : 'short',
        day: 'numeric',
    })
}