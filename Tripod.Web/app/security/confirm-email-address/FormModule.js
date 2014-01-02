/// <amd-dependency path="angular"/>
'use strict';
define(["require", "exports", './FormController', '../../ng/directives/SubmitActionDirective', '../../ng/directives/InputPreFormatterDirective', "angular"], function(require, exports, c, dSubmitAction, dInputPreFormatter) {
    exports.ngModule = angular.module('test', []).controller('FormController', c.FormController).directive('ngT3SubmitAction', dSubmitAction.ngT3SubmitAction).directive('input', dInputPreFormatter.ngT3InputPreFormatter).directive('ngT3RemoveClass', function () {
        return {
            require: '?ngModel',
            restrict: 'A',
            link: function (scope, element, attrs) {
                element.removeClass(attrs.removeClass);
            }
        };
    });
    return exports.ngModule;
});
