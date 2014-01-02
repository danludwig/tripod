/// <amd-dependency path="angular"/>

'use strict';

import c = require('./FormController');
//import rcSubmit = require('../../ng/directives/PreventSubmit');
import t3SubmitAction = require('../../ng/directives/SubmitActionDirective');
import testValidator = require('./TestValidator');

export var ngModule = angular.module('test', [])
    .controller('FormController', c.FormController)
    //.directive('rcSubmit', rcSubmit.rcSubmit)
    .directive('ngT3SubmitAction', t3SubmitAction.ngT3SubmitAction)
    .directive('input', function() {
        return {
            require: '?ngModel',
            restrict: 'E',
            link: function ($scope, $element, $attrs, ngModelController) {
                var inputType = angular.lowercase($attrs.type);

                if (!ngModelController || inputType === 'radio' ||
                    inputType === 'checkbox') {
                    return;
                }

                ngModelController.$formatters.unshift(function (value) {
                    if (ngModelController.$invalid && angular.isUndefined(value)
                        && typeof ngModelController.$modelValue === 'string') {
                        return ngModelController.$modelValue;
                    } else {
                        return value;
                    }
                });
            }
        };
    })
    .directive('ngT3RemoveClass', function () {
        return {
            require: '?ngModel',
            restrict: 'A',
            link: function (scope, element, attrs) {
                element.removeClass(attrs.removeClass);
            }
        };
    })
    //.directive('ngTripodValidateEmailAddress', testValidator.ngTripodValidateEmailAddress)
;
return ngModule;