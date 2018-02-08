var $cecu = $('.cecu');

$(document).ready(function () {

    $('.default-site').click(function (e) {
        e.preventDefault();
        $.cookie("spec_version", null, {
            path:'/'
        });
        location.reload();
    });


    var cookie_color = $.cookie('cs');
    if (cookie_color != undefined) {
        EditColor(cookie_color);
    }
    else {
        EditColor('cs-bw');
    }

    var cookie_size = $.cookie('fs');
    if (cookie_size != undefined) {
        EditSize(cookie_size);
    }
    else {
        EditSize('fs-l');
    }

    var cookie_img = $.cookie('img');
    if (cookie_img != undefined) {
        EditImg(cookie_img);
    }
    else {
        EditImg('on');
    }


    //color
    $('.cs-outer button').click(function () {
        $('.cs-outer button').removeClass('active');
        $(this).addClass("active");
        EditColor($(this).attr('id'));
        $.cookie('cs', $(this).attr('id'))
    });
    //size
    $('.fs-outer button').click(function () {
        $('.fs-outer button').removeClass('active');
        $(this).addClass("active");
        EditSize($(this).attr('id'));
        $.cookie('fs', $(this).attr('id'))
    });
    //img
    $('.img-outer button').click(function () {
        if ($('#img-onoff-text').text() == ' Выключить') {
            //off
            $.cookie('img', 'off');
            EditImg("off")
            $('#img-onoff-text').text(' Включить');

        }
        else {
            //on
            $.cookie('img', 'on');
            EditImg("on")
            $('#img-onoff-text').text(' Выключить');
            $(this).removeClass("active");
        }

    });








    if ($('.geo_area').length > 0) {
        $('.geo_area').each(function () {
            GeoCollection($(this));
        });
    }

    $('input[data-type=date').datepicker({ onSelect: function (dateText, inst) { $(this).attr('value', dateText); } });

    


});

function EditColor(newclass) {
    $cecu.removeClass('cs-bw cs-wb cs-bb cs-gb cs-yg');
    $cecu.addClass(newclass);
}

function EditSize(newclass) {
    $cecu.removeClass('fs-n fs-m fs-l');
    $cecu.addClass(newclass);
}
function EditImg(status) {
    $cecu.removeClass('on off');
    $cecu.addClass(status);
}




function GeoCollection(obj) {

    //опредлеим центр карты
    var x_center = 0;
    var y_center = 0;
    var point_count = obj.find('.geopoint').length;

    if (point_count >0) {
        obj.find('.geopoint').each(function () {
            x_center = x_center + parseFloat($(this).attr('data-x'));
            y_center = y_center + parseFloat($(this).attr('data-y'));
        });


        function init() {
            var idmaparea = obj.find('.maplist').attr('id');
            myMap = new ymaps.Map(idmaparea, {
                center: [x_center / point_count, y_center / point_count],
                zoom: 14,
                controls: []
            }, {
                    searchControlProvider: 'yandex#search'
                });

            myMap.controls.add('zoomControl');
            myMap.behaviors.disable('scrollZoom');
            var gCollection = new ymaps.GeoObjectCollection();

            obj.find('.geopoint').each(function () {
                var _placemark = push($(this));

                gCollection.add(_placemark);

                _placemark.events.add('click', function (e) {
                    gCollection.each(function (geoObject) {
                        geoObject.options.set({
                            preset: 'islands#blueDotIcon'
                        });
                    });
                    var activeGeoObject = e.get('target');
                    activeGeoObject.options.set({
                        preset: 'islands#redDotIcon'
                    });
                });
            });
            myMap.geoObjects.add(gCollection);




        }


        function push(obj) {
            var x = obj.attr('data-x');
            var y = obj.attr('data-y');
            var title = obj.attr('data-title');
            var addres = obj.attr('data-addres');

            var _placemark = new ymaps.Placemark([x, y],
                {
                    balloonContent: '<b>' + title + '</b><br> ' + addres
                }, {
                    preset: 'islands#blueDotIcon'
                });


            return _placemark;
        }



        ymaps.ready(init);
    }
}

function PhGall() {
    $(".swipebox").swipebox();
}