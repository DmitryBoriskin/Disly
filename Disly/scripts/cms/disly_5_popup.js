$(document).ready(function () {
    var iframe = $('.modal_frame', parent.document.body);
    iframe.height($(document).height() + 5);

    // Инициализация полосы прокрутки
    if ($(".scrollbar").length > 0) $(".scrollbar").mCustomScrollbar();

    // устанавливаем курсор
    setCursor();
})


// устанавливаем курсор
function setCursor() {
    if ($('input.input-validation-error').length > 0) $('input.input-validation-error:first').focus();
    else if ($('input[required]').val() == '') $('input[required]:first').focus();
    else if ($(' input').length > 0) $('input:first').focus();
}