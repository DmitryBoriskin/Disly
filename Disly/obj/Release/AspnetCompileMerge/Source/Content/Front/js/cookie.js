

function getParameterByName(name, url) {
  if (!url) url = window.location.href;
  name = name.replace(/[\[\]]/g, "\\$&");
  var regex = new RegExp("[?&]" + name + "(=([^&#]*)|&|#|$)", "i"),
  results = regex.exec(url);
  if (!results) return null;
  if (!results[2]) return '';
  return decodeURIComponent(results[2].replace(/\+/g, " "));
}


function getCookie(cname) {
  var name = cname + "=";
  var ca = document.cookie.split(';');
  for(var i=0; i<ca.length; i++) {
    var c = ca[i];
    while (c.charAt(0)==' ') c = c.substring(1);
    if (c.indexOf(name) == 0) return c.substring(name.length,c.length);
  }
  return "";
}


function setCookie(cname, cvalue, exdays) {
  var d = new Date();
  d.setTime(d.getTime() + (exdays*24*60*60*1000));
  var expires = "expires="+d.toUTCString();
  document.cookie = cname + "=" + cvalue + "; " + expires;
}

function addCookie(cname, cvalue)
{
  var oldcookie=getCookie(cname);
  if ($.inArray(cvalue,oldcookie.split(',')) != -1)
    return null;
  if (oldcookie == "")
    document.cookie = cname + "=" + cvalue;
  else 
    document.cookie = cname + "=" + oldcookie +","+ cvalue;
}

function removeCookie(cname, cvalue)
{
  var oldcookie=getCookie(cname).split(","); 
  var cookie= jQuery.grep(oldcookie, function(value){
    return value != cvalue;
  });
  document.cookie = cname + "=" + cookie;
}

