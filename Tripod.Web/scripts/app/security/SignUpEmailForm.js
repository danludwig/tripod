'use strict';
var App;
(function (App) {
    (function (Security) {
        (function (SignUpEmailForm) {
            var Controller = (function () {
                function Controller(scope) {
                    this.scope = scope;
                    this.emailAddress = '';
                    this.isExpectingEmail = false;
                    this.purpose = 0;
                    scope.vm = this;
                }
                Controller.prototype.emailAddressInputGroupValidationAddOnCssClass = function () {
                    return this.scope.signUpEmailCtrb.emailAddress.hasFeedback() ? null : 'hide';
                };

                Controller.prototype.isEmailAddressRequiredError = function () {
                    return this.scope.signUpEmailForm.emailAddress.$error.required && this.scope.signUpEmailCtrb.emailAddress.hasError;
                };

                Controller.prototype.isEmailAddressPatternError = function () {
                    return this.scope.signUpEmailForm.emailAddress.$error.email && this.scope.signUpEmailCtrb.emailAddress.hasError;
                };

                Controller.prototype.isEmailAddressServerError = function () {
                    return this.scope.signUpEmailForm.emailAddress.$error.server && this.scope.signUpEmailCtrb.emailAddress.hasError;
                };

                Controller.prototype.isExpectingEmailError = function () {
                    return this.scope.signUpEmailCtrb.isExpectingEmail.hasError;
                };

                Controller.prototype.isExpectingEmailRequiredError = function () {
                    return this.scope.signUpEmailForm.isExpectingEmail.$error.required && this.isExpectingEmailError();
                };

                Controller.prototype.isExpectingEmailServerError = function () {
                    return this.scope.signUpEmailForm.isExpectingEmail.$error.server && this.isExpectingEmailError();
                };

                Controller.prototype.isSubmitWaiting = function () {
                    return this.scope.signUpEmailCtrb.isSubmitWaiting;
                };

                Controller.prototype.isSubmitError = function () {
                    return !this.isSubmitWaiting() && this.scope.signUpEmailCtrb.hasError;
                };

                Controller.prototype.isSubmitReady = function () {
                    return !this.isSubmitWaiting() && !this.isSubmitError();
                };

                Controller.prototype.isSubmitDisabled = function () {
                    return this.isSubmitWaiting() || this.isSubmitError();
                };

                Controller.prototype.submitCssClass = function () {
                    return this.isSubmitError() ? 'btn-danger' : null;
                };
                Controller.$inject = ['$scope'];
                return Controller;
            })();
            SignUpEmailForm.Controller = Controller;

            SignUpEmailForm.moduleName = 'sign-up-form';

            SignUpEmailForm.ngModule = angular.module(SignUpEmailForm.moduleName, [App.Modules.Tripod.moduleName]);

            angular.bootstrap($('[ng-module=' + SignUpEmailForm.moduleName + ']'), [SignUpEmailForm.moduleName]);
        })(Security.SignUpEmailForm || (Security.SignUpEmailForm = {}));
        var SignUpEmailForm = Security.SignUpEmailForm;
    })(App.Security || (App.Security = {}));
    var Security = App.Security;
})(App || (App = {}));
