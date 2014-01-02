'use strict';
define(["require", "exports"], function(require, exports) {
    function ngT3SubmitAction($parse) {
        return {
            restrict: 'A',
            require: ['ngT3SubmitAction', '?form'],
            controller: [
                '$scope', function ($scope) {
                    var formController = null;

                    this.setFormController = function (controller) {
                        formController = controller;
                    };

                    this.needsAttention = function (fieldModelController) {
                        if (!formController)
                            return false;

                        if (fieldModelController) {
                            return fieldModelController.$invalid && (fieldModelController.$dirty || this.attempted);
                        } else {
                            return formController && formController.$invalid && (formController.$dirty || this.attempted);
                        }
                    };

                    this.isGoodToGo = function (fieldModelController) {
                        if (!formController)
                            return false;
                        if (this.needsAttention(fieldModelController))
                            return false;

                        if (fieldModelController) {
                            return fieldModelController.$valid && (fieldModelController.$dirty || this.attempted);
                        } else {
                            return formController && formController.$valid && (formController.$dirty || this.attempted);
                        }
                    };

                    this.attempted = false;

                    this.setAttempted = function () {
                        this.attempted = true;
                    };
                }],
            compile: function (cElement, cAttributes, transclude) {
                return {
                    pre: function (scope, formElement, attributes, controllers) {
                        var submitController = controllers[0];

                        var formController = (controllers.length > 1) ? controllers[1] : null;
                        submitController.setFormController(formController);

                        scope.t3 = scope.t3 || {};
                        scope.t3[attributes.name] = submitController;
                    },
                    post: function (scope, formElement, attributes, controllers) {
                        var submitController = controllers[0];
                        var formController = (controllers.length > 1) ? controllers[1] : null;

                        var fn = $parse(attributes.ngT3SubmitAction);

                        formElement.bind('submit', function () {
                            submitController.setAttempted();
                            if (!scope.$$phase)
                                scope.$apply();

                            if (!formController.$valid)
                                return false;

                            scope.$apply(function () {
                                fn(scope, { $event: event });
                            });
                        });

                        if (attributes.ngT3SubmitActionAttempted)
                            submitController.setAttempted();
                    }
                };
            }
        };
    }
    exports.ngT3SubmitAction = ngT3SubmitAction;

    exports.ngT3SubmitAction.$inject = ['$parse'];
});
