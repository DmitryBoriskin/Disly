function toggleCodes(on) {
  var obj = document.getElementById('icons');

  if (on) {
    obj.className += ' codesOn';
  } else {
    obj.className = obj.className.replace(' codesOn', '');
  }
}


$( "#input-search" ).keyup(function() {
//   alert( "Handler for .change() called." );

$.ajax({
 async: false,
 type: "POST",
 url: '/list/searchAjax',
 data: { 
   'searchtext': $( "#input-search" ).val()
 },
 // error: function () { Content = '<div>Error! This page is not found!</div>'; },
 success: appendAutocomplete
});

});

function appendAutocomplete(data) {
  var autocomplite = $( "#autocomplete");
  autocomplite.empty();
  $.each(data, function( index, value ) {
    autocomplite.append("<li class='list-group-item'><a href='/list/"+value.C_Path+"/"+value.C_Slug+"'>"+value.C_Name+"</a></li>");
  });
}

function search() {
  event.preventDefault();
  var checked = $(".visible .valuequery:input:checkbox:checked, #edu-type-block input:checkbox:checked, #city-block input:checkbox:checked, #sort-block input:checkbox:checked");
  var querystring = "list?";
  var hash = {};
  checked.each( function(){
    if (hash[$(this).attr("key")] != null)
    {
      hash[$(this).attr("key")] += ","+$(this).attr("val");
    }else{
      hash[$(this).attr("key")] = $(this).attr("val");
    }
  });

  $.each(hash, function(key, value){
    querystring += key +"="+value+"&";
  });
  var keyword_search = $("#input-search").val();
  if (keyword_search != "")
    querystring += "keyword=" + keyword_search + "&";
  querystring = querystring.substring(0, querystring.length - 1);
  window.location.href = querystring;
  return false;
}







// $(window).load(function () {

//     $('#update-container').click(function (e) {
//         e.preventDefault();

//         $.ajax({
//             url: '@Url.Action("Index", "Institution")',
//             success: function (data) {
//                 $('#container').html(data);
//             }
//         });
//     })

//     navigator.geolocation.getCurrentPosition(success, error);
// });

// function success(position) {
//     var s = document.querySelector('#status');

//     if (s.className == 'success') {
//         // not sure why we're hitting this twice in FF, I think it's to do with a cached result coming back    
//         return;
//     }

//     s.innerHTML = "found you!";
//     s.className = 'success';

//     var mapcanvas = document.createElement('div');
//     mapcanvas.id = 'mapcanvas';
//     mapcanvas.style.height = '600px';
//     mapcanvas.style.width = '1100px';

//     document.querySelector('article').appendChild(mapcanvas);

//     var latlng = new google.maps.LatLng(position.coords.latitude, position.coords.longitude);
//     var myOptions = {
//         zoom: 16,
//         center: latlng,
//         mapTypeControl: false,
//         navigationControlOptions: { style: google.maps.NavigationControlStyle.SMALL },
//         mapTypeId: google.maps.MapTypeId.ROADMAP
//     };
//     var map = new google.maps.Map(document.getElementById("mapcanvas"), myOptions);

//     var marker = new google.maps.Marker({
//         position: latlng,
//         map: map,
//         title: "You are here!"
//     });
// }

// function error(msg) {
//     var s = document.querySelector('#status');
//     s.innerHTML = typeof msg == 'string' ? msg : "failed";
//     s.className = 'fail';

//     // console.log(arguments);
// }