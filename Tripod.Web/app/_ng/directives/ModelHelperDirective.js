'use strict';
define(["require", "exports"], function(require, exports) {
    exports.directiveName = 'modelHelper';

    var ModelHelperController = (function () {
        function ModelHelperController(scope) {
            this.serverValidating = false;
        }
        ModelHelperController.prototype.hasError = function () {
            return !this.serverValidating && this.modelController.$invalid && (this.modelController.$dirty || this.formController.submitAttempted);
        };

        ModelHelperController.prototype.hasSuccess = function () {
            return !this.serverValidating && !this.hasError() && this.modelController.$valid && (this.modelController.$dirty || this.formController.submitAttempted);
        };

        ModelHelperController.prototype.hasFeedback = function () {
            return this.hasError() || this.hasSuccess();
        };
        ModelHelperController.$inject = ['$scope'];
        return ModelHelperController;
    })();
    exports.ModelHelperController = ModelHelperController;

    //#region Directive
    var directiveFactory = function () {
        return function () {
            var directive = {
                name: exports.directiveName,
                restrict: 'A',
                scope: true,
                require: [exports.directiveName, 'ngModel', '^formHelper'],
                controller: ModelHelperController,
                link: function (scope, element, attr, ctrls) {
                    // get the required controllers based on directive order
                    var helpCtrl = ctrls[0];
                    var modelCtrl = ctrls[1];
                    var formCtrl = ctrls[2];

                    // give the helper controller access to the other controllers
                    helpCtrl.modelController = modelCtrl;
                    helpCtrl.formController = formCtrl;

                    // put the helper controller on the scope
                    var alias = $.trim(attr['name']);
                    if (alias)
                        formCtrl[alias] = helpCtrl;
                }
            };
            return directive;
        };
    };

    //#endregion
    exports.directive = directiveFactory();
});
