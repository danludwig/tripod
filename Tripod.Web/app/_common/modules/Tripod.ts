'use strict';

module App.Modules.Tripod {

    export var moduleName = 'tripod';

    export var ngModule = angular.module(moduleName, ['ui.bootstrap.popover', 'ui.bootstrap.tpls'])
        .directive(Directives.InputPreFormatter.directiveName, Directives.InputPreFormatter.directive)
        .directive(Directives.RemoveCssClass.directiveName, Directives.RemoveCssClass.directive)
        .directive(Directives.PopoverToggle.directiveName, Directives.PopoverToggle.directive).config(Directives.PopoverToggle.directiveConfig)
        .directive(Directives.TooltipToggle.directiveName, Directives.TooltipToggle.directive).config(Directives.TooltipToggle.directiveConfig)
        .directive(Directives.FormContrib.directiveName, Directives.FormContrib.directive)
        .directive(Directives.ModelContrib.directiveName, Directives.ModelContrib.directive)
        .directive(Directives.ServerError.directiveName, Directives.ServerError.directive)
        .directive(Directives.ServerValidate.directiveName, Directives.ServerValidate.directive)
        .directive(Directives.SubmitAction.directiveName, Directives.SubmitAction.directive)
    ;
}
