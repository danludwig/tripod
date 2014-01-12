'use strict';
var App;
(function (App) {
    (function (Directives) {
        (function (ModelHelper) {
            ModelHelper.directiveName = 'modelHelper';

            var Controller = (function () {
                function Controller(scope) {
                    this.isServerValidating = false;
                    this.isNoSuccess = false;
                }
                Controller.prototype.hasError = function () {
                    return !this.isServerValidating && this.modelController.$invalid && (this.modelController.$dirty || this.formController.submitAttempted);
                };

                Controller.prototype.hasSuccess = function () {
                    return !this.isNoSuccess && !this.isServerValidating && !this.hasError() && this.modelController.$valid && (this.modelController.$dirty || this.formController.submitAttempted);
                };

                Controller.prototype.hasFeedback = function () {
                    return this.hasError() || this.hasSuccess() || this.hasSpinner();
                };

                Controller.prototype.hasSpinner = function () {
                    return this.isServerValidating;
                };
                Controller.$inject = ['$scope'];
                return Controller;
            })();
            ModelHelper.Controller = Controller;

            var directiveFactory = function () {
                return function () {
                    var directive = {
                        name: ModelHelper.directiveName,
                        restrict: 'A',
                        require: [ModelHelper.directiveName, 'ngModel', '^formHelper'],
                        controller: Controller,
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

            ModelHelper.directive = directiveFactory();
        })(Directives.ModelHelper || (Directives.ModelHelper = {}));
        var ModelHelper = Directives.ModelHelper;
    })(App.Directives || (App.Directives = {}));
    var Directives = App.Directives;
})(App || (App = {}));
