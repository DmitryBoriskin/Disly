var $modal, $modalTitle, $modalBody, $modalFooter
var change = 0;

$(document).ready(function () {
    $modal = $('.modal');
    $modalTitle = $('.modal .modal-title');
    $modalBody = $('.modal .modal-body');
    $modalFooter = $('.modal .modal-footer');

    // Если система администрирования загружена в frame открываем её в родительском окне
    if (top != self) {
        top.location.href = location.href;
    }

    // Полоса прокрутки главного меню
    $(".menu-block").mCustomScrollbar();
    // Полоса прокрутки
    $('.scrollbar').mCustomScrollbar();


    
    // События Кнопок
    $('input[type=submit], .button').bind({
        mousedown: function () {
            var btn_class = $(this).attr('value');

            if (btn_class == 'cancel-btn')
            {
                // Показываем диалог при попытке выйти без сохранения данных
                if (change != 0) {
                    Confirm('Уведомление', 'Выйти без изменений?', $(this));
                }
                else {
                    $('form input[required]').removeAttr('required');
                    $(this).trigger('click');
                }
                return false;
            }
            else if (btn_class == 'delete-btn')
            {
                // Показываем диалог при удалении записи
                Confirm('Уведомление', 'Вы хотите удалить эту запись?', $(this));
                return false;
            }
        },
        click: function () {
            var btn_class = $(this).attr('value');
            var req_count = $('form input[required]:invalid').length

            if (req_count > 0 && btn_class == 'save-btn') {
            }
            else {
                // показываем preloader при клике на кнопку
                var $load_bg = $("<div/>", { "class": "load_page" });
                $load_bg.bind({
                    mousedown: function () { return false; },
                    selectstart: function () { return false; }
                });

                $('body').append($load_bg);
            }
        }
    });
    // показываем preloader при клике на ссылку
    //$('a').click(function () {
    //    var $load_bg = $("<div/>", { "class": "load_page" });
    //    $load_bg.bind({
    //        mousedown: function () { return false; },
    //        selectstart: function () { return false; }
    //    });

    //    $('body').append($load_bg);
    //});

    // Панель авторизации пользователя    
    $('.account-info').click(function () {
        $('.admin-settings').toggle();
    });
    
    // Показывает в модальном окне фрейм
    $('a.pop-up_frame').mousedown(function () {
        PopUpFrame($(this));
        return false;
    });

    // Показываем сообщение в модальном окне
    $('.modal[data-show="true"]').modal('toggle');
    $('.modal-footer a[data-action=false]').bind({
        click: function () {
            $('.modal').modal('toggle');
            setCursor();

            return false;
        }
    })

    // инициализаация полей даты
    $('input[data-type=date').datepicker({ onSelect: function (dateText, inst) { $(this).attr('value', dateText); } });
    // инициализаация TinyMCE
    $('textarea[type=editor]').each(function () {
        InitTinyMCE_new($(this).attr('id'), 0, 300, "/UserFiles/");
    });
    $('textarea[type=liteeditor]').each(function () {
        InitLiteTinyMCE($(this).attr('id'), 0, 300);
    });
    // Инициализация чекбоксов
    // Инициализация полей выбора файлов
    // Инициализация полей с выпадающим списком

    
    //Вызов плагина маски ввода
    //alert($('input[data - mask]').data('mask'));
    $('input[data-mask]').each(function () {
        $(this).mask($(this).attr('data-mask'));
    });
    

    // Изменение приоритета
    if ($(".sortable").length > 0) $('.sortable').each(function () {
        Sorting_init($(this));
    });

    // Перехват нажатия клавиш клавиатуры
    $(window).keydown(function (e) {
        //alert(e.keyCode);
        if (e.keyCode == 112) { $('div#HelpIcons a.HelpIcon').trigger('click'); return false; }
        if ((e.ctrlKey) && (e.keyCode == 83)) { $('button[value="create-btn"]').trigger('click'); $('button[value="update-btn"]').trigger('click'); return false; }
        if ((e.ctrlKey) && (e.keyCode == 68)) { $('button[value="delete-btn"]').trigger('click'); return false; }
        if ((e.ctrlKey) && (e.keyCode == 73)) { $('button[value="insert-btn"]').trigger('click'); return false; }
        if (e.keyCode == 27) { $('button[value="cancel-btn"]').trigger('mousedown'); return false; }
        if ((e.ctrlKey) && (e.keyCode == 13)) { FindNext(window.getSelection().toString()); }
    });

    // Помечаем страницу при изменении контента
    $('input, select, textarea').bind({
        change: function () {
            change = 1;
            requiredTest();
        },
        keyup: function () {
            change = 1;
            requiredTest();
        }
    });

    //телефонные номер а отделениях
    $('.depart_phone_del').click(function (e) {
        e.preventDefault();
        var idPhone = $(this).attr("data-id");
        var $Container = $(this).parent().parent();
        $.ajax({
            type: "POST",
            async: false,
            url: "/admin/orgs/DelPhoneDepart",
            data: { id: idPhone },
            error: function () { alert("error"); },
            success: function (data) {
                $Container.remove();                
            }
        });
    });

    // валидация обязательных полей для заполнения 
    requiredTest();

    // Скрываем или раскрываем блоки
    if ($('div.group-block').length > 0) GroupBlock_init();

    // устанавливаем курсор
    setCursor();

    // Показываем страницу, убираем preloader
    load_page();
});

var validSumm = $('.validation-summary-valid');
if (validSumm.length > 0 && validSumm.find('li')[0].innerHTML != '') validSumm.css('display', 'block');

// Если есть пустые, обязательные для заполнения поля, делаем не активной главную кнопку для сохранеиния записи
function requiredTest() {
    var emptyRequiredLength = $('form input[required]:invalid').length;

    if (emptyRequiredLength > 0 || change == 0)
        $('button[data-primary]').animate({ opacity: "0.3" }, 300).attr('disabled', '');
    else
        $('button[data-primary]').animate({ opacity: "1" }, 300).removeAttr('disabled');
}

// Показываем страницу, убираем preloader
function load_page() {
    var $load_bg = $('div.load_page');
    setTimeout(function () {
        $load_bg.fadeOut();
    }, 300);
};

// Очищаем модальное окно
function clear_modal() {
    $modal.find('.modal-dialog').removeClass().addClass('modal-dialog'),
    $modalTitle.empty();
    $modalBody.empty();
    $modalFooter.empty();
}

// Всплывающие окна
function Confirm(Title, Body, Object) {
    clear_modal();

    var $BtnOk = $('<button/>', { 'id': 'modal-btn-ok', 'class': 'btn btn-danger' }).append('Да');
    $BtnOk.click(function () {
        $('form input[required]').removeAttr('required');
        Object.trigger('click');
    })

    var $BtnNo = $('<button/>', { 'id': 'modal-btn-no', 'class': 'btn btn-default' }).append('Нет');
    $BtnNo.click(function () {
        $('.modal').modal('toggle');
    });

    $modalTitle.append(Title);
    $modalBody.append('<p>' + Body + '</p>');
    $modalFooter.append($BtnOk).append($BtnNo);
    
    $modal.modal('toggle');
}

// Всплывающие окна с активным контентом
function PopUpFrame(Object) {
    clear_modal();
    
    $frale = $('<iframe/>', { 'class': 'modal_frame', 'frameborder': '0', 'width': '100%', 'height': '20', 'src': Object.attr("href") });
    
    $modal.find('.modal-dialog').addClass(' modal-lg'),
    $modalTitle.append(Object.attr("title"));
    $modalBody.append($frale);

    $modal.modal('toggle');

    $('.modal').on('hidden.bs.modal', function (e) {
        e.preventDefault();
        History.pushState(null, document.title, location.href);
        loadPage(location.href);

        function loadPage(url) {
            $('.filtr-block').load(url + ' .filtr-block > *', function () {
                $('.filtr-block a.pop-up_frame').click(function () {
                    PopUpFrame($(this));
                    return false;
                });
            });
        }

        History.Adapter.bind(window, 'statechange', function (e) {
            var State = History.getState();
            loadPage(State.url);
        });
        //location.reload();
    })
}

// Сортировка
function Sorting_init(Object) {
    Object.sortable({
        axis: "y",
        start: function () { $(this).addClass('active'); },
        stop: function (event, ui) {
            $(this).removeClass('active');

            var _ServiceUrl = $(this).attr('data-service');
            var _SortableItem = ui.item;
            var _Id = _SortableItem.attr('data-id');
            var _Num = $(this).find('.ui-sortable-handle').index(_SortableItem) + 1;

            _ServiceUrl = _ServiceUrl;

            $.ajax({
                type: "POST",
                async: false,
                url: _ServiceUrl,
                data: { id: _Id, permit: _Num },
                error: function () { Content = '<div>Error!</div>'; },
                success: function (data) { }
            });
        }
    }).disableSelection();
}

// Инициализация раскрывающегося блока
function GroupBlock_init() {
    $('div.group-block').each(function () {
        $(this).wrapInner('<div class="group-block_info"></div>');
        var $BlockTitle = $("<div/>", { "class": "group-block_title" }).append($(this).attr('title'));
        var $BlockInfo = $(this).find('.group-block_info');

        $BlockTitle.click(function () {
            var Class = $(this).parent().attr('class');

            $BlockInfo.slideToggle(
                function () {
                    if (Class.indexOf('open') == -1) $(this).parent().addClass('open');
                    else $(this).parent().removeClass('open');
                });

            return false;
        });

        $(this).prepend($BlockTitle);
    });
}

// устанавливаем курсор
function setCursor()
{
    if ($('.content input.input-validation-error').length > 0) $('.content input.input-validation-error:first').focus();
    else if ($('.content input[required]').val() == '') $('.content input[required]:first').focus();
    else if ($('.content input:not([type=file]):not([data-focus=False])').length > 0) $('.content input:not([type=file]):not([data-focus=False]):first').focus();
}

// TinyMCE
function InitTinyMCE(id, _width, _height, directory) {
    tinymce.init({
        selector: "textarea#" + id,
        //theme: "modern",
        add_unload_trigger: false,
        schema: "html5",
        plugins: [["anchor nonbreaking paste hr searchreplace  textcolor charmap  link autolink image media table visualblocks code fullscreen contextmenu"]],
        toolbar: 'undo redo | styleselect fontsizeselect | bold italic underline superscript subscript | forecolor backcolor | alignleft aligncenter alignright alignjustify | bullist numlist | outdent indent | table | link image media | removeformat code',
        contextmenu: "copy paste | link image",
        extended_valid_elements: "code",
        invalid_elements: "script,!--",
        convert_urls: false,
        relative_urls: false,
        image_advtab: true,
        cleanup: false,
        erify_html: false,
        statusbar: false,
        language: 'ru',
        menubar: false,
        verify_html: false,
        width: _width,
        height: _height,
        file_browser_callback: function (field_name, url, type, win) {
            var cmsURL = "http://" + window.location.hostname + "/FileManager/?cmd=edit&path=" + directory;
            $('div#Canvas').after('<div id="Opacity"></div><iframe id="FileManager" src="' + cmsURL + '">Ваш браузер не поддерживает плавающие фреймы!</iframe>');

            $('iframe#FileManager').bind('load', function () { FileManagerLoad(field_name); });
            if (win.getImageData) win.getImageData();
        }
    });
}
// TinyMCE (Урезанная версия)
function InitLiteTinyMCE(id, _width, _height) {
    tinymce.init({
        selector: "textarea#" + id,
        theme: "modern",
        add_unload_trigger: false,
        schema: "html5",
        plugins: [["textcolor"]],
        toolbar: "fontsizeselect | bold italic underline | forecolor backcolor | alignleft aligncenter alignright alignjustify",
        invalid_elements: "script,!--",
        convert_urls: false,
        relative_urls: false,
        image_advtab: true,
        cleanup: false,
        erify_html: false,
        statusbar: false,
        language: 'ru',
        width: _width,
        height: _height,
        menubar: false
    });
}


// TinyMCE новая версия
function InitTinyMCE_new(id, _width, _height, directory) {
    tinymce.init({
        selector: "textarea#" + id,
        //theme: "modern",
        add_unload_trigger: false,
        schema: "html5",
        plugins: [["anchor nonbreaking paste hr searchreplace  textcolor charmap  link autolink image media table visualblocks code fullscreen contextmenu"]],
        toolbar: 'undo redo | styleselect fontsizeselect | bold italic underline superscript subscript | forecolor backcolor | alignleft aligncenter alignright alignjustify | bullist numlist | outdent indent | table | link image media | removeformat code',
        contextmenu: "copy paste | link image",
        extended_valid_elements: "code",
        invalid_elements: "script,!--",
        convert_urls: false,
        relative_urls: false,
        image_advtab: true,
        cleanup: false,
        statusbar: false,
        language: 'ru',
        menubar: false,
        verify_html: false,
        width: _width,
        height: _height,

        automatic_uploads: true,
        images_upload_url: 'http://' + window.location.hostname +(location.port ? ':'+location.port: '')+ '/Admin/Services/GetFile/',
        file_picker_callback: function(cb, value, meta) {
            var input = document.createElement('input');
            input.setAttribute('type', 'file');
            input.setAttribute('accept', 'image/*');
            input.onchange = function() {
                var file = this.files[0];
      
                var reader = new FileReader();
                reader.readAsDataURL(file);
                reader.onload = function () {
                    var id = 'blobid' + (new Date()).getTime();
                    var blobCache =  tinymce.activeEditor.editorUpload.blobCache;
                    var base64 = reader.result.split(',')[1];
                    var blobInfo = blobCache.create(id, file, base64);
                    blobCache.add(blobInfo);

                    cb(blobInfo.blobUri(), { title: file.name });
                };
            };
    
            input.click();
        }

        //,file_picker_callback: function (field_name, url, type, win) {
        //    var cmsURL = "http://" + window.location.hostname + "/FileManager/?cmd=edit&path=" + directory;
        //    $('div#Canvas').after('<div id="Opacity"></div><iframe id="FileManager" src="' + cmsURL + '">Ваш браузер не поддерживает плавающие фреймы!</iframe>');

        //    $('iframe#FileManager').bind('load', function () { FileManagerLoad(field_name); });
        //    if (win.getImageData) win.getImageData();
        //}
    });
}