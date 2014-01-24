'use strict';
var App;
(function (App) {
    (function (Security) {
        (function (SignUpUserForm) {
            var Controller = (function () {
                function Controller(scope) {
                    this.scope = scope;
                    this.password = '';
                    this.confirmPassword = '';
                    this.userName = '';
                    this.token = '';
                    scope.vm = this;
                }
                Controller.prototype.passwordInputGroupValidationAddOnCssClass = function () {
                    return this.scope.signUpUserCtrb.password.hasFeedback() ? null : 'hide';
                };

                Controller.prototype.isPasswordRequiredError = function () {
                    return this.scope.signUpUserForm.password.$error.required && this.scope.signUpUserCtrb.password.hasError;
                };

                Controller.prototype.isPasswordMinLengthError = function () {
                    return this.scope.signUpUserForm.password.$error.minlength && this.scope.signUpUserCtrb.password.hasError;
                };

                Controller.prototype.isPasswordMaxLengthError = function () {
                    return this.scope.signUpUserForm.password.$error.maxlength && this.scope.signUpUserCtrb.password.hasError;
                };

                Controller.prototype.isPasswordServerError = function () {
                    return this.scope.signUpUserForm.password.$error.server && this.scope.signUpUserCtrb.password.hasError;
                };

                Controller.prototype.confirmPasswordInputGroupValidationAddOnCssClass = function () {
                    return this.scope.signUpUserCtrb.confirmPassword.hasFeedback() ? null : 'hide';
                };

                Controller.prototype.isConfirmPasswordRequiredError = function () {
                    return this.scope.signUpUserForm.confirmPassword.$error.required && this.scope.signUpUserCtrb.confirmPassword.hasError;
                };

                Controller.prototype.isConfirmPasswordEqualError = function () {
                    return this.scope.signUpUserForm.confirmPassword.$error.equal && !this.scope.signUpUserForm.confirmPassword.$error.required && !this.scope.signUpUserForm.confirmPassword.$error.server && this.scope.signUpUserCtrb.confirmPassword.hasError;
                };

                Controller.prototype.isConfirmPasswordServerError = function () {
                    return this.scope.signUpUserForm.confirmPassword.$error.server && this.scope.signUpUserCtrb.confirmPassword.hasError;
                };

                Controller.prototype.userNameInputGroupValidationAddOnCssClass = function () {
                    return this.scope.signUpUserCtrb.userName.hasFeedback() ? null : 'hide';
                };

                Controller.prototype.isUserNameRequiredError = function () {
                    return this.scope.signUpUserForm.userName.$error.required && this.scope.signUpUserCtrb.userName.hasError;
                };

                Controller.prototype.isUserNameServerError = function () {
                    return this.scope.signUpUserForm.userName.$error.server && this.scope.signUpUserCtrb.userName.hasError;
                };

                Controller.prototype.tokenInputGroupValidationAddOnCssClass = function () {
                    return this.scope.signUpUserCtrb.token.hasFeedback() ? null : 'hide';
                };

                Controller.prototype.isTokenRequiredError = function () {
                    return this.scope.signUpUserForm.token.$error.required && this.scope.signUpUserCtrb.token.hasError;
                };

                Controller.prototype.isTokenServerError = function () {
                    return this.scope.signUpUserForm.token.$error.server && this.scope.signUpUserCtrb.token.hasError;
                };

                Controller.prototype.isSubmitWaiting = function () {
                    return this.scope.signUpUserCtrb.isSubmitWaiting;
                };

                Controller.prototype.isSubmitError = function () {
                    return !this.isSubmitWaiting() && this.scope.signUpUserCtrb.hasError;
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
            SignUpUserForm.Controller = Controller;

            SignUpUserForm.moduleName = 'sign-up-password-form';

            SignUpUserForm.ngModule = angular.module(SignUpUserForm.moduleName, [App.Modules.Tripod.moduleName]);
        })(Security.SignUpUserForm || (Security.SignUpUserForm = {}));
        var SignUpUserForm = Security.SignUpUserForm;
    })(App.Security || (App.Security = {}));
    var Security = App.Security;
})(App || (App = {}));
