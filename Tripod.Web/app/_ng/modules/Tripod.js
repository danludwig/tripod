'use strict';
var App;
(function (App) {
    (function (Modules) {
        (function (Tripod) {
            Tripod.moduleName = 'tripod';

            Tripod.ngModule = angular.module(Tripod.moduleName, []).directive(App.Directives.Input.directiveName, App.Directives.Input.directive).directive(App.Directives.RemoveCssClass.directiveName, App.Directives.RemoveCssClass.directive).directive(App.Directives.Popover.directiveName, App.Directives.Popover.directive).directive(App.Directives.FormHelper.directiveName, App.Directives.FormHelper.directive).directive(App.Directives.ModelHelper.directiveName, App.Directives.ModelHelper.directive).directive(App.Directives.ServerError.directiveName, App.Directives.ServerError.directive).directive(App.Directives.ServerValidate.directiveName, App.Directives.ServerValidate.directive).directive(App.Directives.SubmitAction.directiveName, App.Directives.SubmitAction.directive);
        })(Modules.Tripod || (Modules.Tripod = {}));
        var Tripod = Modules.Tripod;
    })(App.Modules || (App.Modules = {}));
    var Modules = App.Modules;
})(App || (App = {}));
