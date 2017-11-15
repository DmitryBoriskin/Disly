$(document).ready(function () {
    //$('.carousel').carousel({
    //    interval: 60000
    //});

    // эффект наведения в слайдере
    //$('.slider-message').hover(function () {
    //    $(this).toggleClass('active');
    //});

    $('.select2').select2();

    $('input[data-type=date').datepicker({ onSelect: function (dateText, inst) { $(this).attr('value', dateText); } });

    $('input[data-mask]').each(function () {
        $(this).mask($(this).attr('data-mask'));
    });



    // анимация баннеров
    if ($('.ling_img_on').length > 0) {
        $('.ling_img_on').each(function () {
            CaruselSlide($(this));
        });
    }


    //Фильтр по новостям
    //$('.filtr_news').submit(function (e) {
        
    //    var start = $('#data_start').val();
    //    var end = $('#data_fin').val();
    //    var search = $('#search_news').val();        

    //    var params = {
    //        datearea: search,
    //        datestart: start,
    //        datefin: end
    //    };

    //    var str = jQuery.param(params);
    //    location.href = UrlPage() + "?" + str;
    //    e.preventDefault();
    //});


    //coords
    if ($('.buildmap').length > 0) {
        $('.buildmap').each(function () {
            var x = $(this).attr('data-x');
            var y = $(this).attr('data-y');
            var title = $(this).attr('data-title');
            var desc = $(this).attr('data-desc');
            var zoom = $(this).attr('data-zoom');
            var height = $(this).attr('data-height');
            Coords(x, y, title, desc, zoom, height);

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



function Coords(x, y, title, desc, zoom, height) {
    ymaps.ready(function () {
        if (title == '') { title = "Название организации"; }
        if (desc == '') { desc = "Описание организации"; }

        var ContactMap = new ymaps.Map("map", {
            center: [x, y],
            zoom: zoom,
            controls: ['zoomControl']
            //controls: ['zoomControl', 'searchControl', 'typeSelector', 'fullscreenControl', 'routeButtonControl']
        });
        ContactMap.controls.add('zoomControl', { top: 5 });

        myPlacemark = new ymaps.Placemark([x, y], {
            balloonContentHeader: title,
            balloonContentBody: desc,
            hintContent: title
        }, {
            //iconLayout: 'default#image',
            //iconImageHref: '/img/marker_map.png',
            //iconImageSize: [26, 39],
            //iconImageOffset: [-13, -39],
            hasBalloon: false
        });

        ContactMap.geoObjects.add(myPlacemark);
        $('ymaps.ymaps-map').css({ 'height': height + 'px', "width": "inherit" });
    });
}