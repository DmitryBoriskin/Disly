// СТРЕЛКИ КОЛАПСА НА ПАНЕЛИ ПОИСКА
//
//ОБЩИЕ ХАРАКТЕРИСТИКИ
$("#common-collapse").on('shown.bs.collapse', function () {
 $("#common-panel .panel-heading .panel-title .fa").removeClass("fa-angle-down").addClass("fa-angle-up");
});
$('#common-collapse').on('hidden.bs.collapse', function () {
 $("#common-panel .panel-heading .panel-title .fa").removeClass("fa-angle-up").addClass("fa-angle-down");
});
//УЧЕБНЫЙ ПРОЦЕСС
$("#education-collapse").on('shown.bs.collapse', function () {
 $("#education-panel .panel-heading .panel-title .fa").removeClass("fa-angle-down").addClass("fa-angle-up");
});
$('#education-collapse').on('hidden.bs.collapse', function () {
 $("#education-panel .panel-heading .panel-title .fa").removeClass("fa-angle-up").addClass("fa-angle-down");
});
//ОСНАЩЕНИЕ
$("#edu-item-collapse").on('shown.bs.collapse', function () {
 $("#edu-item-panel .panel-heading .panel-title .fa").removeClass("fa-angle-down").addClass("fa-angle-up");
});
$('#edu-item-collapse').on('hidden.bs.collapse', function () {
 $("#edu-item-panel .panel-heading .panel-title .fa").removeClass("fa-angle-up").addClass("fa-angle-down");
});
//ДОПОЛНИТЕЛЬНОЕ ОБРАЗОВАНИЕ
$("#additional-education-collapse").on('shown.bs.collapse', function () {
 $("#additional-education-panel .panel-heading .panel-title .fa").removeClass("fa-angle-down").addClass("fa-angle-up");
});
$('#additional-education-collapse').on('hidden.bs.collapse', function () {
 $("#additional-education-panel .panel-heading .panel-title .fa").removeClass("fa-angle-up").addClass("fa-angle-down");
});


// СТРЕЛКИ КОЛАПСА НА ТАБЛИЦЕ СРАВНЕЕНИЯ
//
//ОБЩИЕ ХАРАКТЕРИСТИКИ
$(".common-collapse").on('shown.bs.collapse', function () {
 $("#common-header .fa").removeClass("fa-angle-down").addClass("fa-angle-up");
});
$(".common-collapse").on('hidden.bs.collapse', function () {
 $("#common-header .fa").removeClass("fa-angle-up").addClass("fa-angle-down");
});
//УЧЕБНЫЙ ПРОЦЕСС
$(".edu-collapse").on('shown.bs.collapse', function () {
 $("#edu-header .fa").removeClass("fa-angle-down").addClass("fa-angle-up");
});
$(".edu-collapse").on('hidden.bs.collapse', function () {
 $("#edu-header .fa").removeClass("fa-angle-up").addClass("fa-angle-down");
});
//ОСНАЩЕНИЕ
$(".stuff-collapse").on('shown.bs.collapse', function () {
 $("#stuff-header .fa").removeClass("fa-angle-down").addClass("fa-angle-up");
});
$(".stuff-collapse").on('hidden.bs.collapse', function () {
 $("#stuff-header .fa").removeClass("fa-angle-up").addClass("fa-angle-down");
});
//ДОП ОБРАЗОВАНИЕ
$(".add-collapse").on('shown.bs.collapse', function () {
 $("#add-header .fa").removeClass("fa-angle-down").addClass("fa-angle-up");
});
$(".add-collapse").on('hidden.bs.collapse', function () {
 $("#add-header .fa").removeClass("fa-angle-up").addClass("fa-angle-down");
});






// ЧЕКБОСАЫ НА ТАБЛИЦЕ ПОИСКА, ВЫДЕЛЕНИЕ ТОЛЬКО ОДНОГО
//
//ГОРОДА
// var boxesCity = $("#city-block input:checkbox").click(function(){
//   boxesCity.not(this).attr('checked', false);
// });
// //ВИД УЧРЕЖДЕНИЯ
// var boxesEduType = $("#edu-type-block input:checkbox").click(function(){
//   boxesEduType.not(this).attr('checked', false);
// });

function reset_checkbox()
{
  $("input:checkbox:checked").prop( "checked", false );
}

function check_type()
{
  var SearchList = $(".search-list");
  var CheckdBox = $("#edu-type-block input:checkbox:checked");

  if (CheckdBox.size()>1 )
  {
    hide_seachlist();
  }
  else if (CheckdBox.size()==0)
  {
    hide_seachlist();
  }
  else 
  {
    var show = CheckdBox.attr("show");
    hide_seachlist();
    $("."+show).show();
    $("."+show).addClass("visible");
    $(".sort-list").show();
  }
  hide_empty_block();
}

function hide_empty_block()
{
  var panels =$('.search-panel');
  panels.each( function() {
    var visible = $(this).find(".visible");
    if (visible.length==0)
      $(this).hide();
    else 
      $(this).show();
  });
}


function hide_seachlist()
{
  var SearchList = $(".hideable");
  SearchList.hide();
  SearchList.removeClass("visible");
  $(".sort-list").hide();
  return true;
}

hide_seachlist();
hide_empty_block();
$("#edu-type-block input:checkbox").click(check_type);
check_type();



// Модуль галлереи
var lightbox = $('.photo-module a').simpleLightbox();


function more_city()
{
  var checkbox = $("#city-block .checkbox");
  checkbox.each( function() {
      $(this).show();
  });
  $("#more-btn").hide();
  $("#less-btn").show();

}

function less_city()
{
  var count = 1;
  var checkbox =$($("#city-block .checkbox").get().reverse());
  var show_count = checkbox.length - 5;
  checkbox.each( function() {
      if (show_count<=0)
        return true;
      $(this).hide();
      show_count--;
  }); 
  $("#more-btn").show();
  $("#less-btn").hide();
}

less_city();
// Совместный скрол разных элементов на странице сравнения
$(".parent-scroller").scroll(function(){
  $(".child-scroller").scrollLeft($(".parent-scroller").scrollLeft());
  $(".child-scroller").scrollTop($(".parent-scroller").scrollTop());
});







// Делает высоту столбцов в страницы сравнения одинаковыми
function calculate_height() { 
 $(".calculate-height").each( function(){
   var original =  $(this).find(".child-scroller table tr");
   var compare = $(this).find(".compare-left table tr") ;
   var length = original.length;
   for(var i=0; i<length; i+=1) {
     $(compare[i]).height( $(original[i]).height()) 
   }
 });
}


$(window).resize(function() {
  calculate_height();
});

calculate_height();

$("#input-search").val(getParameterByName("keyword",null));


// $('#grammar-modal').modal('show') 






