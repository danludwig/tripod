'use strict';
var App;
(function (App) {
    (function (Directives) {
        (function (FormContrib) {
            FormContrib.directiveName = 'formContrib';

            var Controller = (function () {
                function Controller() {
                    this.isSubmitAttempted = false;
                    this.isSubmitWaiting = false;
                }
                return Controller;
            })();
            FormContrib.Controller = Controller;

            var directiveFactory = function () {
                return [
                    '$parse', function ($parse) {
                        var directive = {
                            name: FormContrib.directiveName,
                            restrict: 'A',
                            require: [FormContrib.directiveName, 'form'],
                            controller: Controller,
                            compile: function () {
                                return {
                                    pre: function (scope, element, attr, ctrls) {
                                        var contribCtrl = ctrls[0];

                                        if (attr['formSubmitted'])
                                            contribCtrl.isSubmitAttempted = true;

                                        var alias = $.trim(attr[FormContrib.directiveName]);
                                        if (alias)
                                            scope[alias] = contribCtrl;
                                    },
                                    post: function (scope, element, attr, ctrls) {
                                        var contribCtrl = ctrls[0];
                                        var formCtrl = ctrls[1];

                                        var fn = $parse(attr['formSubmit']);

                                        element.bind('submit', function () {
                                            contribCtrl.isSubmitAttempted = true;

                                            if (formCtrl.$valid)
                                                contribCtrl.isSubmitWaiting = true;
                                            if (!scope.$$phase)
                                                scope.$apply();

                                            if (!formCtrl.$valid)
                                                return false;

                                            scope.$apply(function () {
                                                fn(scope, { $event: event });
                                            });

                                            return true;
                                        });
                                    }
                                };
                            }
                        };
                        return directive;
                    }];
            };

            FormContrib.directive = directiveFactory();
        })(Directives.FormContrib || (Directives.FormContrib = {}));
        var FormContrib = Directives.FormContrib;
    })(App.Directives || (App.Directives = {}));
    var Directives = App.Directives;
})(App || (App = {}));
