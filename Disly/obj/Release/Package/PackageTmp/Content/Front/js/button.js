function checkButtons()
{
  var like = getCookie("like");
  var compare = getCookie("compare");
  var likeCount=0;
  var compareCount=0;
  if (like != ""){
    var likeArray=like.split(',');
    likeArray.forEach( function(entry) {
      if (entry=="")
        return null;
      var btn= $(".like-btn[guid*="+entry+"]");
      btn.attr("state","in");
      btn.find("i").addClass("active");
      btn.find("i").removeClass("fa-heart-o");
      btn.find("i").addClass("fa-heart");
      var btn_detail= $(".like-detail-btn[guid*="+entry+"]");
      btn_detail.attr("state","in");
      btn_detail.addClass("active");
      btn_detail.find("i").addClass("active");
      btn_detail.find("span").text("В ИЗБРАННОМ");
    });
    likeCount = likeArray.length;
  }

  if (compare != ""){
    var compareArray=compare.split(',');
    compareArray.forEach( function(entry) {
      if (entry=="")
        return null;
      var btn= $(".cmp-btn[guid*="+entry+"]");
      btn.attr("state","in");
      btn.find("i").addClass("active");
      var btn_detail= $(".cmp-detail-btn[guid*="+entry+"]");
      btn_detail.attr("state","in");
      btn_detail.addClass("active");
      btn_detail.find("i").addClass("active");
    });
    compareCount = compareArray.length;
  }
  $(".like-number span").text(likeCount);
  if (likeCount == 0)
  {
    $(".like-number i").removeClass("active");
    $(".like-number i").removeClass("fa-heart");
    $(".like-number i").addClass("fa-heart-o");

  }
  else
  {
    $(".like-number i").addClass("active");
    $(".like-number i").removeClass("fa-heart-o");
    $(".like-number i").addClass("fa-heart");
  }
  $(".compare-number span").text(compareCount);
  if(compareCount == 0){
    $(".compare-number i").removeClass("active");
  }
  else
  {
    $(".compare-number i").addClass("active");
  }
}




$( ".cmp-detail-btn" ).click(function() {
  if ($(this).attr("state") == "out")
  {
    addCookie("compare",$(this).attr("guid"));
    $(this).attr("state","in");
    $(this).addClass("active");
    $(this).find("i").addClass("active");
  }
  else if($(this).attr("state") == "in")
  {
    removeCookie("compare", $(this).attr("guid"))
    $(this).attr("state","out");
    $(this).removeClass("active");
    $(this).find("i").removeClass('active');
  }
  checkButtons();
});

$( ".like-detail-btn" ).click(function() {
  if ($(this).attr("state") == "out")
  {
    addCookie("like",$(this).attr("guid"));
    $(this).attr("state","in");
    $(this).addClass("active");
    $(this).find("i").addClass("active");
    $(this).find("i").removeClass('fa-heart-o');
    $(this).find("i").addClass('fa-heart');
    $(this).find("span").text("В ИЗБРАННОМ");
  }
  else if($(this).attr("state") == "in")
  {
    removeCookie("like", $(this).attr("guid"))
    $(this).attr("state","out");
    $(this).removeClass("active");
    $(this).find("i").removeClass('active');
    $(this).find("i").removeClass('fa-heart');
    $(this).find("i").addClass('fa-heart-o');
    $(this).find("span").text("В ИЗБРАННОЕ");
  }
  checkButtons();
});



$( ".cmp-btn" ).click(function() {
  if ($(this).attr("state") == "out")
  {
    addCookie("compare",$(this).attr("guid"));
    $(this).attr("state","in");
    $(this).find("i").addClass("active");
  }
  else if($(this).attr("state") == "in")
  {
    removeCookie("compare", $(this).attr("guid"))
    $(this).attr("state","out");
    $(this).find("i").removeClass('active');
  }
  checkButtons();
});

$( ".like-btn" ).click(function() {
  if ($(this).attr("state") == "out")
  {
    addCookie("like",$(this).attr("guid"));
    $(this).attr("state","in");
    $(this).find("i").addClass("active");
    $(this).find("i").removeClass('fa-heart-o');
    $(this).find("i").addClass('fa-heart');
  }
  else if($(this).attr("state") == "in")
  {
    removeCookie("like", $(this).attr("guid"))
    $(this).attr("state","out");
    $(this).find("i").removeClass('active');
    $(this).find("i").removeClass('fa-heart');
    $(this).find("i").addClass('fa-heart-o');
  }
  checkButtons();
})

checkButtons();