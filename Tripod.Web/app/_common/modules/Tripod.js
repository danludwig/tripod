'use strict';
var App;
(function (App) {
    (function (Modules) {
        (function (Tripod) {
            Tripod.moduleName = 'tripod';

            Tripod.ngModule = angular.module(Tripod.moduleName, ['ui.bootstrap.popover', 'ui.bootstrap.tpls']).directive(App.Directives.InputPreFormatter.directiveName, App.Directives.InputPreFormatter.directive).directive(App.Directives.RemoveCssClass.directiveName, App.Directives.RemoveCssClass.directive).directive(App.Directives.PopoverToggle.directiveName, App.Directives.PopoverToggle.directive).config(App.Directives.PopoverToggle.directiveConfig).directive(App.Directives.TooltipToggle.directiveName, App.Directives.TooltipToggle.directive).config(App.Directives.TooltipToggle.directiveConfig).directive(App.Directives.FormContrib.directiveName, App.Directives.FormContrib.directive).directive(App.Directives.ModelContrib.directiveName, App.Directives.ModelContrib.directive).directive(App.Directives.ServerError.directiveName, App.Directives.ServerError.directive).directive(App.Directives.ServerValidate.directiveName, App.Directives.ServerValidate.directive).directive(App.Directives.SubmitAction.directiveName, App.Directives.SubmitAction.directive);
        })(Modules.Tripod || (Modules.Tripod = {}));
        var Tripod = Modules.Tripod;
    })(App.Modules || (App.Modules = {}));
    var Modules = App.Modules;
})(App || (App = {}));
