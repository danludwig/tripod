'use strict';

module App.Modules.Tripod {

    export var moduleName = 'tripod';

    var tooltipToggle = Directives.TooltipToggle.directiveSettings();
    var popoverToggle = Directives.TooltipToggle.directiveSettings('popover');

    export var ngModule = angular.module(moduleName, ['ui.bootstrap.popover', 'ui.bootstrap.tpls'])
        .directive(Directives.InputPreFormatter.directiveName, Directives.InputPreFormatter.directive)
        .directive(Directives.RemoveCssClass.directiveName, Directives.RemoveCssClass.directive)
        .directive(tooltipToggle.directiveName, tooltipToggle.directive).config(tooltipToggle.directiveConfig)
        .directive(popoverToggle.directiveName, popoverToggle.directive).config(popoverToggle.directiveConfig)
        .directive(Directives.FormContrib.directiveName, Directives.FormContrib.directive)
        .directive(Directives.ModelContrib.directiveName, Directives.ModelContrib.directive)
        .directive(Directives.ServerError.directiveName, Directives.ServerError.directive)
        .directive(Directives.ServerValidate.directiveName, Directives.ServerValidate.directive)
        .directive(Directives.MustEqual.directiveName, Directives.MustEqual.directive)
    ;
}
