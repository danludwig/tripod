'use strict';
var App;
(function (App) {
    (function (Security) {
        (function (SignInForm) {
            var Controller = (function () {
                function Controller(scope) {
                    this.scope = scope;
                    this.userNameOrVerifiedEmail = '';
                    this.password = '';
                    this.isPersistent = false;
                    scope.vm = this;
                }
                Controller.prototype.userNameInputGroupValidationAddOnCssClass = function () {
                    return this.scope.signInCtrb.userNameOrVerifiedEmail.hasFeedback() ? null : 'hide';
                };

                Controller.prototype.isUserNameRequiredError = function () {
                    return this.scope.signInForm.userNameOrVerifiedEmail.$error.required && this.scope.signInCtrb.userNameOrVerifiedEmail.hasError;
                };

                Controller.prototype.isUserNameServerError = function () {
                    return this.scope.signInForm.userNameOrVerifiedEmail.$error.server && this.scope.signInCtrb.userNameOrVerifiedEmail.hasError;
                };

                Controller.prototype.isPasswordError = function () {
                    return this.scope.signInCtrb.password.hasError && (!this.scope.signInCtrb.userNameOrVerifiedEmail.hasError || this.scope.signInForm.userNameOrVerifiedEmail.$error.required);
                };

                Controller.prototype.passwordCssClass = function () {
                    return this.isPasswordError() ? 'has-error' : null;
                };

                Controller.prototype.passwordInputGroupCssClass = function (cssClass) {
                    return this.isPasswordError() ? cssClass : null;
                };

                Controller.prototype.passwordInputGroupValidationAddOnCssClass = function () {
                    return this.isPasswordError() ? null : 'hide';
                };

                Controller.prototype.isPasswordRequiredError = function () {
                    return this.scope.signInForm.password.$error.required && this.isPasswordError();
                };

                Controller.prototype.isPasswordServerError = function () {
                    return this.scope.signInForm.password.$error.server && this.isPasswordError();
                };

                Controller.prototype.isSubmitWaiting = function () {
                    return this.scope.signInCtrb.isSubmitWaiting;
                };

                Controller.prototype.isSubmitError = function () {
                    return !this.isSubmitWaiting() && this.scope.signInCtrb.hasError;
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
            SignInForm.Controller = Controller;

            SignInForm.moduleName = 'sign-in-form';

            SignInForm.ngModule = angular.module(SignInForm.moduleName, [App.Modules.Tripod.moduleName]);
        })(Security.SignInForm || (Security.SignInForm = {}));
        var SignInForm = Security.SignInForm;
    })(App.Security || (App.Security = {}));
    var Security = App.Security;
})(App || (App = {}));
