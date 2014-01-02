/// <amd-dependency path="angular"/>

'use strict';

import cForm = require('./FormController');
import dSubmitAction = require('../../ng/directives/SubmitActionDirective');
import dInputPreFormatter = require('../../ng/directives/InputPreFormatterDirective');
import dRemoveClass = require('../../ng/directives/RemoveCssClassDirective');
import testValidator = require('./TestValidator');

export var ngModule = angular.module('test', [])
    .controller(cForm.controllerName, cForm.FormController)
    .directive(dSubmitAction.directiveName, dSubmitAction.ngT3SubmitAction)
    .directive(dInputPreFormatter.directiveName, dInputPreFormatter.ngT3InputPreFormatter)
    .directive(dRemoveClass.directiveName, dRemoveClass.ngT3RemoveClass)
//.directive('ngTripodValidateEmailAddress', testValidator.ngTripodValidateEmailAddress)
;
return ngModule;