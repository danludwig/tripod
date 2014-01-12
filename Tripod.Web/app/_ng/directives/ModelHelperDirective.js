'use strict';
define(["require", "exports"], function(require, exports) {
    exports.directiveName = 'modelHelper';

    var ModelHelperController = (function () {
        function ModelHelperController(scope) {
            this.isServerValidating = false;
            this.isNoSuccess = false;
        }
        ModelHelperController.prototype.hasError = function () {
            return !this.isServerValidating && this.modelController.$invalid && (this.modelController.$dirty || this.formController.submitAttempted);
        };

        ModelHelperController.prototype.hasSuccess = function () {
            return !this.isNoSuccess && !this.isServerValidating && !this.hasError() && this.modelController.$valid && (this.modelController.$dirty || this.formController.submitAttempted);
        };

        ModelHelperController.prototype.hasFeedback = function () {
            return this.hasError() || this.hasSuccess() || this.hasSpinner();
        };

        ModelHelperController.prototype.hasSpinner = function () {
            return this.isServerValidating;
        };
        ModelHelperController.$inject = ['$scope'];
        return ModelHelperController;
    })();
    exports.ModelHelperController = ModelHelperController;

    var directiveFactory = function () {
        return function () {
            var directive = {
                name: exports.directiveName,
                restrict: 'A',
                require: [exports.directiveName, 'ngModel', '^formHelper'],
                controller: ModelHelperController,
                link: function (scope, element, attr, ctrls) {
                    var helpCtrl = ctrls[0];
                    var modelCtrl = ctrls[1];
                    var formCtrl = ctrls[2];

                    helpCtrl.modelController = modelCtrl;
                    helpCtrl.formController = formCtrl;

                    var alias = $.trim(attr['name']);
                    if (alias)
                        formCtrl[alias] = helpCtrl;
                }
            };
            return directive;
        };
    };

    exports.directive = directiveFactory();
});
