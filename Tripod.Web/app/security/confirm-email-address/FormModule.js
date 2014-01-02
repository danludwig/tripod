/// <amd-dependency path="angular"/>
'use strict';
define(["require", "exports", './FormController', '../../ng/directives/SubmitActionDirective', '../../ng/directives/InputPreFormatterDirective', '../../ng/directives/RemoveCssClassDirective', "angular"], function(require, exports, cForm, dSubmitAction, dInputPreFormatter, dRemoveClass) {
    exports.ngModule = angular.module('test', []).controller(cForm.controllerName, cForm.FormController).directive(dSubmitAction.directiveName, dSubmitAction.ngT3SubmitAction).directive(dInputPreFormatter.directiveName, dInputPreFormatter.ngT3InputPreFormatter).directive(dRemoveClass.directiveName, dRemoveClass.ngT3RemoveClass);
    return exports.ngModule;
});
