'use strict';

module App.Modules.Tripod {

    export var moduleName = 'tripod';

    export var ngModule = angular.module(moduleName, [])
        .directive(Directives.InputPreFormatter.directiveName, Directives.InputPreFormatter.directive)
        .directive(Directives.RemoveCssClass.directiveName, Directives.RemoveCssClass.directive)
        .directive(Directives.Popover.directiveName, Directives.Popover.directive)
        .directive(Directives.FormContrib.directiveName, Directives.FormContrib.directive)
        .directive(Directives.ModelHelper.directiveName, Directives.ModelHelper.directive)
        .directive(Directives.ServerError.directiveName, Directives.ServerError.directive)
        .directive(Directives.ServerValidate.directiveName, Directives.ServerValidate.directive)
        .directive(Directives.SubmitAction.directiveName, Directives.SubmitAction.directive)
    ;
}
