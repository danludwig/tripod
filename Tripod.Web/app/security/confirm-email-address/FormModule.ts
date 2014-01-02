/// <amd-dependency path="angular"/>

'use strict';

import c = require('./FormController');
import dSubmitAction = require('../../ng/directives/SubmitActionDirective');
import dInputPreFormatter = require('../../ng/directives/InputPreFormatterDirective');
import testValidator = require('./TestValidator');

export var ngModule = angular.module('test', [])
    .controller('FormController', c.FormController)
    .directive('ngT3SubmitAction', dSubmitAction.ngT3SubmitAction)
    .directive('input', dInputPreFormatter.ngT3InputPreFormatter)
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