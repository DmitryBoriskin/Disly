$(document).ready(function () {
    // Афиша на главной
    $('.afisha_title').each(function () {
        var h = $(this).height();
        if (h > 90) {            
            $(this).append("<div class='afisha_shadow'></div>");
        }
    });

    // раскрываем блок с афишей на сегодня
    if ($('#action_all_btn').length > 0) {
        $('#action_all_btn').click(function (e) {
            e.preventDefault();
            window.history.pushState(null, document.title, location.href);

            if ($('.afisha_list').length === 1) {
                loadPage(location.href);

                function loadPage(url) {
                    $('.afisha_list').load(url + '?placecard=all' + ' .afisha_list > *')
                }
                $(this).hide();
            }
        });
    }

   
});