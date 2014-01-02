/// <amd-dependency path="angular"/>
'use strict';
define(["require", "exports", './FormController', '../../ng/directives/SubmitActionDirective', "angular"], function(require, exports, c, t3SubmitAction) {
    exports.ngModule = angular.module('test', []).controller('FormController', c.FormController).directive('ngT3SubmitAction', t3SubmitAction.ngT3SubmitAction).directive('input', function () {
        return {
            require: '?ngModel',
            restrict: 'E',
            link: function ($scope, $element, $attrs, ngModelController) {
                var inputType = angular.lowercase($attrs.type);

                if (!ngModelController || inputType === 'radio' || inputType === 'checkbox') {
                    return;
                }

                ngModelController.$formatters.unshift(function (value) {
                    if (ngModelController.$invalid && angular.isUndefined(value) && typeof ngModelController.$modelValue === 'string') {
                        return ngModelController.$modelValue;
                    } else {
                        return value;
                    }
                });
            }
        };
    });
    return exports.ngModule;
});
