'use strict';
var App;
(function (App) {
    (function (Directives) {
        (function (SubmitAction) {
            SubmitAction.directiveName = 'ngT3SubmitAction';

            var Controller = (function () {
                function Controller() {
                    this.attempted = false;
                }
                Controller.prototype.needsAttention = function (fieldModelController) {
                    if (!this.formController)
                        return false;

                    if (fieldModelController)
                        return fieldModelController.$invalid && (fieldModelController.$dirty || this.attempted);

                    return this.formController && this.formController.$invalid && (this.formController.$dirty || this.attempted);
                };

                Controller.prototype.isGoodToGo = function (fieldModelController) {
                    if (!this.formController)
                        return false;
                    if (this.needsAttention(fieldModelController))
                        return false;

                    if (fieldModelController)
                        return fieldModelController.$valid && (fieldModelController.$dirty || this.attempted);

                    return this.formController && this.formController.$valid && (this.formController.$dirty || this.attempted);
                };
                return Controller;
            })();

            var directiveFactory = function () {
                return [
                    '$parse', function ($parse) {
                        var d = {
                            name: SubmitAction.directiveName,
                            restrict: 'A',
                            require: ['ngT3SubmitAction', '?form'],
                            controller: Controller,
                            compile: function () {
                                return {
                                    pre: function (scope, formElement, attributes, controllers) {
                                        var submitController = controllers[0];

                                        var formController = (controllers.length > 1) ? controllers[1] : null;
                                        submitController.formController = formController;

                                        scope['t3'] = scope['t3'] || {};
                                        scope['t3'][attributes['name']] = submitController;
                                    },
                                    post: function (scope, formElement, attributes, controllers) {
                                        var submitController = controllers[0];
                                        var formController = (controllers.length > 1) ? controllers[1] : null;

                                        var fn = $parse(attributes[SubmitAction.directiveName]);

                                        formElement.bind('submit', function () {
                                            submitController.attempted = true;
                                            if (!scope.$$phase)
                                                scope.$apply();

                                            if (!formController.$valid)
                                                return false;

                                            scope.$apply(function () {
                                                fn(scope, { $event: event });
                                            });
                                            return true;
                                        });

                                        if (attributes['ngT3SubmitActionAttempted'])
                                            submitController.attempted = true;
                                    }
                                };
                            }
                        };
                        return d;
                    }];
            };

            SubmitAction.directive = directiveFactory();
        })(Directives.SubmitAction || (Directives.SubmitAction = {}));
        var SubmitAction = Directives.SubmitAction;
    })(App.Directives || (App.Directives = {}));
    var Directives = App.Directives;
})(App || (App = {}));
