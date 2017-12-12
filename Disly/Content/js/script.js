﻿$(document).ready(function () {
    $('.select2').select2();





    $('input[data-type=date').datepicker({ onSelect: function (dateText, inst) { $(this).attr('value', dateText); } });

    $('input[data-mask]').each(function () {
        $(this).mask($(this).attr('data-mask'));
    });

    //раскрытие кнопки поиска
    if ($('.seacrh_button__hide').length > 0) {
        SearchWork();
    }

    //original photo
    if ($('.show_original,.swipebox').length > 0) {
        $('.show_original').swipebox();
        $(".swipebox").swipebox();
    }
    



    // анимация баннеров
    if ($('.ling_img_on').length > 0) {
        $('.ling_img_on').each(function () {
            CaruselSlide($(this));
        });
    }

    //share
    $('body').on('click', '.btn_share', function (e) {
        e.preventDefault();
        ShowShare($(this));
    });


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

    //rss link spot
    if ($('.rss_param').length > 0) {
        setInterval(function () { SpotRssLink();}, 1500);
        $('.rss_param').change(function () { SpotRssLink()});
        $('.rss_param').bind("change", function () { SpotRssLink() });  


        $('.rss_param.rss_date').datepicker().on('changeDate', function () { SpotRssLink() });
        $('.rss_param.rss_date').change(function () {
            SpotRssLink();
        });
        //$('.rss_param').click(function () {
        //    SpotRssLink();
        //});   

        function SpotRssLink() {
            var _group = $('#rss_param_group').val();
            var _start = $('#rss_param_start').val();
            var _end = $('#rss_param_end').val();
            var _size = $('#rss_param_size').val();
            var _rsslink = $('#rss_param_link');
            var new_rsslink = '/press/rss';
            if (_group != '') {
                new_rsslink = new_rsslink + "/" + _group;
            }
            new_rsslink = new_rsslink + "?";
            if (_start != '') {
                new_rsslink = new_rsslink + "date=" + _start;
            }
            //date
            if (_end != '') {
                new_rsslink = new_rsslink + "&dateend=" + _end;
            }
            if (_size != '') {
                new_rsslink = new_rsslink + "&size=" + _size;
            }

            _rsslink.attr('href', "http://" + _rsslink.attr('data-domain') + new_rsslink);
            _rsslink.text(_rsslink.attr('data-domain') + new_rsslink);
        }

    }


    //coords
    if ($('.buildmap').length > 0) {
        $('.buildmap').each(function () {
            var id = $(this).attr('id');
            var x = $(this).attr('data-x');
            var y = $(this).attr('data-y');
            var title = $(this).attr('data-title');
            var desc = $(this).attr('data-desc');
            var zoom = $(this).attr('data-zoom');
            var height = $(this).attr('data-height');
            Coords(x, y, title, desc, zoom, height, id);
        });
    }


    if ($('.searchform_dop').length > 0) {
        SearchDopWork()
    }

});

function PhGall() {
    $(".swipebox").swipebox();
}


function SearchDopWork() {
    $('.searchform_show_dop').click(function (e) {
        $('.searchform_dop').toggleClass('show');
        document.getElementById("search_focus_dop").focus();
        e.preventDefault();
    });
    //hide form
    $('.searchform_close_dop').click(function (e) {
        $('.searchform_dop').toggleClass('show');
        e.preventDefault();
    });

    $('#search_focus_dop').focusout(function () {
        setTimeout(function () {
            $('.searchform_dop').toggleClass('show');
        }, 400);
    });


    $('.searchform_btn_dop').click(function (e) {
        var SerachInp = $('#search_focus_dop').val();
        if (SerachInp === "") {
            document.getElementById("search_focus_dop").focus();
        }
        else {
            CommitSearchDop();
        }
        e.preventDefault();
    });


    $(".searchform_btn_dop").keydown(function (event) {
        if (event.keyCode == 13) {
            CommitSearchDop();
            event.preventDefault();
        }
    });
    $(".searchform_dop").submit(function (e) {
        CommitSearchDop();
        e.preventDefault();
    });



    function CommitSearchDop() {
        var SerachInp = $('.search-input_dop').val();
        var EndUrl = "%20url%3Ahttp%3A%2F%2F" + SiteId + ".med.cap.ru*&web=0";
        var SearchText = SerachInp.replace(" ", "%20") + EndUrl;
        var Link = "/Search?searchid=2297106&text=" + SearchText + "&searchtext=" + SerachInp.replace(" ", "%20");
        if (SearchText != "") {
            document.location.href = Link;
        }
    }
}

function SearchWork() {
    //show form
    $('.searchform_show').click(function (e) {
        $('.searchform').toggleClass('show');
        document.getElementById("search_focus").focus();
        e.preventDefault();
    });
    //hide form
    $('.searchform_close').click(function (e) {
        $('.searchform').toggleClass('show');
        e.preventDefault();
        return false;
    });
    
    $('#search_focus').focusout(function () {
            $('.searchform').toggleClass('show');
    });


    $('.searchform_btn').click(function (e) {
        var SerachInp = $('#search_focus').val();
        if (SerachInp === "") {
            document.getElementById("search_focus").focus();
        }
        else {
            CommitSearch();
        }
        e.preventDefault();
    });
    
    
    $(".searchform_btn").keydown(function (event) {
        if (event.keyCode == 13) {
            CommitSearch();
            event.preventDefault();
        }
    });
    $(".searchform").submit(function (e) {
        CommitSearch();
        e.preventDefault();
    });



    function CommitSearch() {
        var SerachInp = $('.search-input').val();
        var EndUrl = "%20url%3Ahttp%3A%2F%2F" + SiteId + ".med.cap.ru*&web=0";
        var SearchText = SerachInp.replace(" ", "%20") + EndUrl;        
        var Link = "/Search?searchid=2297106&text=" + SearchText + "&searchtext=" + SerachInp.replace(" ", "%20");
        if (SearchText != "") {
            document.location.href = Link;
        }
    }
    //bottom form
    $('.search-form-bottom').submit(function (e) {
        CommitSearchBottom();
        e.preventDefault();
    });
    $('.search-form-bottom .bottom-search').click(function (e) {
        CommitSearchBottom();        
        e.preventDefault();
    });


    function CommitSearchBottom() {
        var SerachInp = $('.search-form').val();
        var EndUrl = "%20url%3Ahttp%3A%2F%2F" + SiteId + "med.cap.ru*&web=0";
        var SearchText = SerachInp.replace(" ", "%20") + EndUrl;
        var Link = "/Search?searchid=2297106&text=" + SearchText;
        document.location.href = Link;        
    }
}



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


function Coords(x, y, title, desc, zoom, height, id) {
    ymaps.ready(function () {
        if (title == '') { title = "Название организации"; }
        if (desc == '') { desc = "Описание организации"; }

        var ContactMap = new ymaps.Map(id, {
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
                hasBalloon: false
        });

        ContactMap.geoObjects.add(myPlacemark);
        $('ymaps.ymaps-map').css({ 'height': height + 'px', "width": "inherit" });
    });
}




//поделиться share
function ShowShare(e) {
    var x = e.offset().top + 20;
    var y = e.offset().left;
    CreateMaskForShare();
    CreateSharePanel(x, y);
}

function CreateMaskForShare() {
    var html = '<div style="position:absolute;top:0;left:0;width:100%;height:100%;z-index:9999;background-color:transparent" id="mask-for-share"></div>';
    $(document.body).html($(document.body).html() + html);
}

function CreateSharePanel(x, y) {
    var code = '<script type="text/javascript" src="//yastatic.net/share/share.js" charset="utf-8"></script>' +
        '<div class="yashare-auto-init" data-yashareL10n="ru" data-yashareType="medium" data-yashareQuickServices="vkontakte,facebook,twitter,odnoklassniki,moimir,gplus" data-yashareTheme="counter"></div>';

    var html = '<div style="position:absolute;top:' + x + 'px;left:' + y + 'px;text-align:left" id="share-container">' +
        '<img src="/Content/img/share-arrow.png" alt="" style="margin-bottom:-13px;margin-left:10px">' +
        '<div style="border:1px solid #d9d9d9;background-color:white;padding:9px;overflow:hidden">' +
        code;
    +'</div></div>';

    $('#mask-for-share').html(html);
}

$(document).on('click', '#mask-for-share', null,
    function () {
        document.body.removeChild(document.getElementById('mask-for-share'));
    });