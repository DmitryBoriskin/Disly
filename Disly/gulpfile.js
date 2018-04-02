/// <binding BeforeBuild='blue_dark' />
"use strict";

var gulp = require("gulp"),
    rimraf = require("rimraf"),
    less = require("gulp-less"); // добавляем модуль less

var paths = {
    webroot: "./Content/"
};
//  регистрируем задачу по преобразованию styles.less в файл css
gulp.task("comm", function () {
    return gulp.src('content/css/commonless.less')
        .pipe(less())
        .pipe(gulp.dest(paths.webroot + 'css'))
});

gulp.task("blue", function () {
    return gulp.src('content/css/theme/blue.less')
               .pipe(less())
               .pipe(gulp.dest(paths.webroot + 'css/theme'))
});
gulp.task("blue_dark", function () {
    return gulp.src('content/css/theme/blue_dark.less')
        .pipe(less())
        .pipe(gulp.dest(paths.webroot + 'css/theme'))
});
gulp.task("purple", function () {
    return gulp.src('content/css/theme/purple.less')
        .pipe(less())
        .pipe(gulp.dest(paths.webroot + 'css/theme'))
});
gulp.task("turquoise", function () {
    return gulp.src('content/css/theme/turquoise.less')
        .pipe(less())
        .pipe(gulp.dest(paths.webroot + 'css/theme'))
});
gulp.task("green", function () {
    return gulp.src('content/css/theme/green.less')
        .pipe(less())
        .pipe(gulp.dest(paths.webroot + 'css/theme'))
});
gulp.task("green2", function () {
    return gulp.src('content/css/theme/green2.less')
        .pipe(less())
        .pipe(gulp.dest(paths.webroot + 'css/theme'))
});

gulp.task("green_portal", function () {
    return gulp.src('content/css/theme/green_portal.less')
        .pipe(less())
        .pipe(gulp.dest(paths.webroot + 'css/theme'))
});

gulp.task("spec_ver", function () {
    return gulp.src('content/spec/cecu.less')
        .pipe(less())
        .pipe(gulp.dest(paths.webroot + 'spec'))
});