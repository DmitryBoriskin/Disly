$(document).ready(function () {
    $('.carousel').carousel({
        interval: 60000
    });

    // эффект наведения в слайдере
    $('.slider-message').hover(function () {
        $(this).toggleClass('active');
    });
});