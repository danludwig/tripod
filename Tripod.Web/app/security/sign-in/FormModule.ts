/// <amd-dependency path="angular"/>

'use strict';

import cForm = require('./FormController');
import dSubmitAction = require('../../_ng/directives/SubmitActionDirective');
import dInputPreFormatter = require('../../_ng/directives/InputPreFormatterDirective');
import dRemoveClass = require('../../_ng/directives/RemoveCssClassDirective');
import dPopover = require('../../_ng/directives/BootstrapPopover');

export var ngModule = angular.module('SignInModule', ['ui.bootstrap'])
    .controller(cForm.controllerName, cForm.FormController)
    .directive(dSubmitAction.directiveName, dSubmitAction.ngT3SubmitAction)
    .directive(dInputPreFormatter.directiveName, dInputPreFormatter.ngT3InputPreFormatter)
    .directive(dRemoveClass.directiveName, dRemoveClass.ngT3RemoveClass)
    .directive(dPopover.directiveName, dPopover.t3Popover)
;
return ngModule; 