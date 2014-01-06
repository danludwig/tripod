/// <amd-dependency path="angular"/>
'use strict';
define(["require", "exports", './FormController', '../../_ng/directives/SubmitActionDirective', '../../_ng/directives/InputPreFormatterDirective', '../../_ng/directives/RemoveCssClassDirective', '../../_ng/directives/BootstrapPopover', "angular"], function(require, exports, cForm, dSubmitAction, dInputPreFormatter, dRemoveClass, dPopover) {
    exports.ngModule = angular.module('SignInModule', ['ui.bootstrap']).controller(cForm.controllerName, cForm.FormController).directive(dSubmitAction.directiveName, dSubmitAction.ngT3SubmitAction).directive(dInputPreFormatter.directiveName, dInputPreFormatter.ngT3InputPreFormatter).directive(dRemoveClass.directiveName, dRemoveClass.ngT3RemoveClass).directive(dPopover.directiveName, dPopover.t3Popover);
    return exports.ngModule;
});
