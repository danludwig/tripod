'use strict';
var App;
(function (App) {
    (function (Directives) {
        (function (ModelContrib) {
            ModelContrib.directiveName = 'modelContrib';

            var Controller = (function () {
                function Controller(scope) {
                    this.hasError = false;
                    this.hasSuccess = false;
                    this.hasSpinner = false;
                    this.error = {};
                }
                Controller.prototype.setValidity = function (validationErrorKey, validationErrorMessage) {
                    this.ngModelController.$setValidity(validationErrorKey, validationErrorMessage ? false : true);
                    this.error[validationErrorKey] = validationErrorMessage;
                    this.hasSpinner = false;
                };

                Controller.prototype.spinnerCssClass = function () {
                    return this.hasSpinner ? 'has-spinner' : null;
                };

                Controller.prototype.errorCssClass = function () {
                    return this.hasError ? 'has-error' : null;
                };

                Controller.prototype.successCssClass = function () {
                    return this.hasSuccess ? 'has-success' : null;
                };

                Controller.prototype.hasFeedback = function () {
                    return this.hasError || this.hasSuccess || this.hasSpinner;
                };

                Controller.prototype.feedbackCssClass = function () {
                    if (this.hasSpinner)
                        return this.spinnerCssClass();
                    if (this.hasError)
                        return this.errorCssClass();
                    if (this.hasSuccess)
                        return this.successCssClass();
                    return null;
                };

                Controller.prototype.inputGroupCssClass = function (size) {
                    if (!this.hasFeedback())
                        return null;
                    var cssClass = 'input-group';
                    if (size)
                        cssClass += ' input-group-' + size;
                    return cssClass;
                };
                Controller.$inject = ['$scope'];
                return Controller;
            })();
            ModelContrib.Controller = Controller;

            var directiveFactory = function () {
                return function () {
                    var d = {
                        name: ModelContrib.directiveName,
                        restrict: 'A',
                        require: [ModelContrib.directiveName, 'ngModel', '^formContrib'],
                        controller: Controller,
                        compile: function () {
                            return {
                                pre: function (scope, element, attr, ctrls) {
                                    var modelContribCtrl = ctrls[0];
                                    var modelCtrl = ctrls[1];
                                    var formContribCtrl = ctrls[2];
                                    modelContribCtrl.ngModelController = modelCtrl;

                                    var alias = $.trim(attr['name']);
                                    if (alias)
                                        formContribCtrl[alias] = modelContribCtrl;
                                },
                                post: function (scope, element, attr, ctrls) {
                                    var modelContribCtrl = ctrls[0];
                                    var modelCtrl = ctrls[1];
                                    var formContribCtrl = ctrls[2];

                                    scope.$watch(function () {
                                        return [modelCtrl.$valid, modelCtrl.$dirty, formContribCtrl.isSubmitAttempted, modelContribCtrl.hasSpinner];
                                    }, function () {
                                        var isDirtyOrSubmitAttempted = modelCtrl.$dirty || formContribCtrl.isSubmitAttempted;
                                        modelContribCtrl.hasError = !modelContribCtrl.hasSpinner && modelCtrl.$invalid && isDirtyOrSubmitAttempted;
                                        modelContribCtrl.hasSuccess = !modelContribCtrl.hasSpinner && modelCtrl.$valid && isDirtyOrSubmitAttempted;
                                    }, true);
                                }
                            };
                        }
                    };
                    return d;
                };
            };

            ModelContrib.directive = directiveFactory();
        })(Directives.ModelContrib || (Directives.ModelContrib = {}));
        var ModelContrib = Directives.ModelContrib;
    })(App.Directives || (App.Directives = {}));
    var Directives = App.Directives;
})(App || (App = {}));
