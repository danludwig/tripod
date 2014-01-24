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
                    return this.scope.signUpCtrb.emailAddress.hasFeedback() ? null : 'hide';
                };

                Controller.prototype.isEmailAddressRequiredError = function () {
                    return this.scope.signUpForm.emailAddress.$error.required && this.scope.signUpCtrb.emailAddress.hasError;
                };

                Controller.prototype.isEmailAddressPatternError = function () {
                    return this.scope.signUpForm.emailAddress.$error.email && this.scope.signUpCtrb.emailAddress.hasError;
                };

                Controller.prototype.isEmailAddressServerError = function () {
                    return this.scope.signUpForm.emailAddress.$error.server && this.scope.signUpCtrb.emailAddress.hasError;
                };

                Controller.prototype.isExpectingEmailError = function () {
                    return this.scope.signUpCtrb.isExpectingEmail.hasError;
                };

                Controller.prototype.isExpectingEmailRequiredError = function () {
                    return this.scope.signUpForm.isExpectingEmail.$error.required && this.isExpectingEmailError();
                };

                Controller.prototype.isExpectingEmailServerError = function () {
                    return this.scope.signUpForm.isExpectingEmail.$error.server && this.isExpectingEmailError();
                };

                Controller.prototype.isSubmitWaiting = function () {
                    return this.scope.signUpCtrb.isSubmitWaiting;
                };

                Controller.prototype.isSubmitError = function () {
                    return !this.isSubmitWaiting() && this.scope.signUpCtrb.hasError;
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
        })(Security.SignUpEmailForm || (Security.SignUpEmailForm = {}));
        var SignUpEmailForm = Security.SignUpEmailForm;
    })(App.Security || (App.Security = {}));
    var Security = App.Security;
})(App || (App = {}));
