/// <binding />
"use strict";

var gulp = require("gulp"),
    rimraf = require("rimraf"),
    less = require("gulp-less"); // добавляем модуль less

var paths = {
    webroot: "./Content/"
};
//  регистрируем задачу по преобразованию styles.less в файл css
gulp.task("first", function () {
    return gulp.src('content/css/theme/first.less')
               .pipe(less())
               .pipe(gulp.dest(paths.webroot + 'css/theme'))
});