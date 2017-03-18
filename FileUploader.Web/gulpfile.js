"use strict";

var gulp = require("gulp"),
    ts = require("gulp-typescript"),
    sourcemaps = require("gulp-sourcemaps")

var config = {
    jsBasePath: "app/",
    ts: "app/**/*.ts",
    tsConfig: "tsconfig.json",
    js: "app/**/*.js",
    dist: "./dist",
    jsDist: "dist/**/*.js"
}

var tsProject = ts.createProject("tsconfig.json");

gulp.task('typescript', function () {
  var tsResult = gulp.src(config.ts)
    .pipe(sourcemaps.init())
    .pipe(ts(tsProject));

  return tsResult.js
    .pipe(sourcemaps.write('.'))
    .pipe(gulp.dest(config.jsBasePath));
});

gulp.task('watchTypescript', ['typescript'], function () {
  gulp.watch(config.ts, ['typescript']);
});

gulp.task('default', ['typescript']);

