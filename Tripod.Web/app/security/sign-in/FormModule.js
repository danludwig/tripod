/// <amd-dependency path="angular"/>
'use strict';
define(["require", "exports", './FormController', '../../_ng/directives/SubmitActionDirective', '../../_ng/directives/InputPreFormatterDirective', '../../_ng/directives/RemoveCssClassDirective', '../../_ng/directives/BootstrapPopover', '../../_ng/directives/FormHelperDirective', '../../_ng/directives/ModelHelperDirective', '../../_ng/directives/ServerErrorDirective', "angular"], function(require, exports, cForm, dSubmitAction, dInputPreFormatter, dRemoveClass, dPopover, dFormHelper, dModelHelper, dServerError) {
    exports.ngModule = angular.module('SignInModule', ['ui.bootstrap']).controller(cForm.controllerName, cForm.FormController).directive(dSubmitAction.directiveName, dSubmitAction.ngT3SubmitAction).directive(dInputPreFormatter.directiveName, dInputPreFormatter.ngT3InputPreFormatter).directive(dRemoveClass.directiveName, dRemoveClass.ngT3RemoveClass).directive(dPopover.directiveName, dPopover.t3Popover).directive(dFormHelper.directiveName, dFormHelper.directive).directive(dModelHelper.directiveName, dModelHelper.directive).directive(dServerError.directiveName, dServerError.directive);
    return exports.ngModule;
});
