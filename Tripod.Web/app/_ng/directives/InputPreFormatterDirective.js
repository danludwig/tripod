'use strict';
define(["require", "exports"], function(require, exports) {
    exports.directiveName = 'input';

    function ngT3InputPreFormatter() {
        var directive = {
            restrict: 'E',
            require: '?ngModel',
            link: function ($scope, $element, $attrs, ngModelController) {
                var inputType = angular.lowercase($attrs.type);

                if (!ngModelController || inputType === 'radio' || inputType === 'checkbox')
                    return;

                ngModelController.$formatters.unshift(function (value) {
                    if (ngModelController.$invalid && angular.isUndefined(value) && typeof ngModelController.$modelValue === 'string') {
                        return ngModelController.$modelValue;
                    } else {
                        return value;
                    }
                });
            }
        };
        return directive;
    }
    exports.ngT3InputPreFormatter = ngT3InputPreFormatter;
});
