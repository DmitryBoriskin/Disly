$(document).ready(function () {
    $('.carousel').carousel({
        interval: 60000
    });

    // эффект наведения в слайдере
    //$('.slider-message').hover(function () {
    //    $(this).toggleClass('active');
    //});

    // анимация баннеров
    if ($('.ling_img_on').length > 0) {
        $('.ling_img_on').each(function () {
            CaruselSlide($(this));
        });
    }
});

function CaruselSlide(obj) {
    var WrItem = obj.find('.carousel-inner');
    var CountLine = WrItem.attr('data-count');//количество одновременно отоброжаемых банннеров 

    var arr = [];
    var $elem = $("<div/>", { "class": "item active row" });


    var ImgLkCount = $('.carousel-inner>div').length;//фактическое количсество баннеров в секции
    if (CountLine < ImgLkCount) {

        var maxHeight = 0;
        obj.find('.carousel-inner>div').each(function (index) {
            arr[index] = $(this);
            var HeightThis = $(this).outerHeight();
            if (HeightThis > maxHeight) maxHeight = HeightThis;
        });
        WrItem.css('min-height', maxHeight);

        for (var i = 0; i < arr.length; i++) {
            $elem.append(arr[i]);
            if (((i + 1) % CountLine == 0) && (i != 0)) {
                WrItem.append($elem);
                $elem = $("<div/>", { "class": "item" });
            }
        }
        WrItem.append($elem);

        if (obj.find('.item:last-child').text() == "") {
            obj.find('.item:last-child').remove();
        }

        //запуск слайдера
        obj.carousel({
            interval: 6500
        });
        obj.find('.next').click(function (e) { obj.carousel('next'); e.preventDefault(); });
        obj.find('.prev').click(function (e) { obj.carousel('prev'); e.preventDefault(); });
    }
    else {
        obj.find('nav').hide();
    }
}