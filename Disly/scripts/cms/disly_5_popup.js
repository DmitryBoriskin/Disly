$(document).ready(function () {
    var iframe = $('.modal_frame', parent.document.body);
    iframe.height($(document).height() + 5);

    // Инициализация полосы прокрутки
    if ($(".scrollbar").length > 0) $(".scrollbar").mCustomScrollbar();

    // устанавливаем курсор
    setCursor();

    //Назначение прав группе
    $("#modal-userGroupResolutions-table input[type='checkbox']").on('ifChanged', function () {
        var targetUrl = "/Admin/Services/SaveGroupClaims";
        var _group = $(this).data("group");
        var _url = $(this).data("url");
        var _action = $(this).data("action");
        var _checked = $(this).is(':checked');

        var el = $(this);
       // var content = $(".alert-success");
        

        try
        {
            var params = {
                GroupAlias: _group,
                ContentId:_url,
                Claim: _action,
                Checked: _checked
            };

            var _data = JSON.stringify(params);

            //ShowPreloader(content);
            
            $.ajax({
                url: targetUrl,
                method: "POST",
                cache: false,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: JSON.stringify(params)
            })
                .done(function (response) {
                    el.tooltip('show');
                    //content.html("Сохранено");
                    //content.fadeIn("slow");
                    //EnableButton(el);
                })
                .fail(function (jqXHR, status) {
                    //content.html("Ошибка" + " " + status + " " + jqXHR);
                    //content.fadeIn("slow");
                    //EnableButton(el);
                })
                .always(function (response) {
                    setTimeout(function () {
                        //content.fadeOut("slow");
                        el.tooltip('hide');
                    }, 1000);
                    //EnableButton(el);
                    //content.html("Выполнено " + response);
                    //if (documentSetId !== null) {
                    //    //CaseReloadEDocument(e, el, documentSetId);
                    //}
                });
        }
        catch(ex){

        }

        //var Content = null;
        //$.ajax({
        //    type: "POST",
        //    async: false,
        //    url: '/Admin/services/GroupResolut/',
        //    data: ({ user: _User, url: _Url, action: _Action, val: _Value }),
        //    error: function () { Content = 'Error' },
        //    success: function (data) { Content = data; }
        //});
    });

})


// устанавливаем курсор
function setCursor() {
    if ($('input.input-validation-error').length > 0) $('input.input-validation-error:first').focus();
    else if ($('input[required]').val() == '') $('input[required]:first').focus();
    else if ($(' input').length > 0) $('input:first').focus();
}