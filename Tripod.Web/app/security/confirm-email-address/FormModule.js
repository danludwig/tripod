/// <amd-dependency path="angular"/>
'use strict';
define(["require", "exports", '../../ng/directives/PreventSubmit', './FormController', "angular"], function(require, exports, rcSubmit, c) {
    exports.ngModule = angular.module('test', []).controller('FormController', c.FormController).directive('rcSubmit', rcSubmit.rcSubmit);
    return exports.ngModule;
});
