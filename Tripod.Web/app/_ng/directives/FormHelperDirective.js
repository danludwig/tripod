'use strict';
var App;
(function (App) {
    (function (Directives) {
        (function (FormHelper) {
            FormHelper.directiveName = 'formHelper';

            var Controller = (function () {
                function Controller() {
                    this.submitAttempted = false;
                    this.isSubmitDisabled = false;
                }
                return Controller;
            })();
            FormHelper.Controller = Controller;

            var directiveFactory = function () {
                return [
                    '$parse', function ($parse) {
                        var directive = {
                            name: FormHelper.directiveName,
                            restrict: 'A',
                            require: [FormHelper.directiveName, 'form'],
                            controller: Controller,
                            compile: function () {
                                return {
                                    pre: function (scope, element, attr, ctrls) {
                                        var helpCtrl = ctrls[0];
                                        var formCtrl = ctrls[1];

                                        helpCtrl.formController = formCtrl;

                                        if (attr['submitAttempted'])
                                            helpCtrl.submitAttempted = true;

                                        var alias = $.trim(attr[FormHelper.directiveName]);
                                        if (alias)
                                            scope[alias] = helpCtrl;
                                    },
                                    post: function (scope, element, attr, ctrls) {
                                        var helpCtrl = ctrls[0];
                                        var formCtrl = ctrls[1];

                                        element.bind('submit', function () {
                                            helpCtrl.submitAttempted = true;
                                            if (formCtrl.$valid)
                                                helpCtrl.isSubmitDisabled = true;
                                            if (!scope.$$phase)
                                                scope.$apply();

                                            if (!formCtrl.$valid)
                                                return false;

                                            return true;
                                        });
                                    }
                                };
                            }
                        };
                        return directive;
                    }];
            };

            FormHelper.directive = directiveFactory();
        })(Directives.FormHelper || (Directives.FormHelper = {}));
        var FormHelper = Directives.FormHelper;
    })(App.Directives || (App.Directives = {}));
    var Directives = App.Directives;
})(App || (App = {}));
