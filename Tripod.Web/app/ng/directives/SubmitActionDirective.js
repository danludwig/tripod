'use strict';
define(["require", "exports"], function(require, exports) {
    function ngT3SubmitAction($parse) {
        var directive = {
            restrict: 'A',
            require: ['ngT3SubmitAction', '?form'],
            controller: [SubmitActionController],
            compile: function () {
                return {
                    pre: function (scope, formElement, attributes, controllers) {
                        var submitController = controllers[0];

                        var formController = (controllers.length > 1) ? controllers[1] : null;
                        submitController.formController = formController;

                        scope['t3'] = scope['t3'] || {};
                        scope['t3'][attributes.name] = submitController;
                    },
                    post: function (scope, formElement, attributes, controllers) {
                        var submitController = controllers[0];
                        var formController = (controllers.length > 1) ? controllers[1] : null;

                        var fn = $parse(attributes.ngT3SubmitAction);

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

                        if (attributes.ngT3SubmitActionAttempted)
                            submitController.attempted = true;
                    }
                };
            }
        };
        return directive;
    }
    exports.ngT3SubmitAction = ngT3SubmitAction;

    exports.ngT3SubmitAction.$inject = ['$parse'];

    var SubmitActionController = (function () {
        function SubmitActionController() {
            this.attempted = false;
        }
        SubmitActionController.prototype.needsAttention = function (fieldModelController) {
            if (!this.formController)
                return false;

            if (fieldModelController)
                return fieldModelController.$invalid && (fieldModelController.$dirty || this.attempted);

            return this.formController && this.formController.$invalid && (this.formController.$dirty || this.attempted);
        };

        SubmitActionController.prototype.isGoodToGo = function (fieldModelController) {
            if (!this.formController)
                return false;
            if (this.needsAttention(fieldModelController))
                return false;

            if (fieldModelController)
                return fieldModelController.$valid && (fieldModelController.$dirty || this.attempted);

            return this.formController && this.formController.$valid && (this.formController.$dirty || this.attempted);
        };
        return SubmitActionController;
    })();
});
