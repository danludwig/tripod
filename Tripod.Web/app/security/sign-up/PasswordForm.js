'use strict';
var App;
(function (App) {
    (function (Security) {
        (function (SignUp) {
            (function (PasswordForm) {
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
                        return this.scope.createLocalCtrb.password.hasFeedback() ? null : 'hide';
                    };

                    Controller.prototype.isPasswordRequiredError = function () {
                        return this.scope.createLocalForm.password.$error.required && this.scope.createLocalCtrb.password.hasError;
                    };

                    Controller.prototype.isPasswordMinLengthError = function () {
                        return this.scope.createLocalForm.password.$error.minlength && this.scope.createLocalCtrb.password.hasError;
                    };

                    Controller.prototype.isPasswordMaxLengthError = function () {
                        return this.scope.createLocalForm.password.$error.maxlength && this.scope.createLocalCtrb.password.hasError;
                    };

                    Controller.prototype.isPasswordServerError = function () {
                        return this.scope.createLocalForm.password.$error.server && this.scope.createLocalCtrb.password.hasError;
                    };

                    Controller.prototype.confirmPasswordInputGroupValidationAddOnCssClass = function () {
                        return this.scope.createLocalCtrb.confirmPassword.hasFeedback() ? null : 'hide';
                    };

                    Controller.prototype.isConfirmPasswordRequiredError = function () {
                        return this.scope.createLocalForm.confirmPassword.$error.required && this.scope.createLocalCtrb.confirmPassword.hasError;
                    };

                    Controller.prototype.isConfirmPasswordEqualError = function () {
                        return this.scope.createLocalForm.confirmPassword.$error.equal && !this.scope.createLocalForm.confirmPassword.$error.required && !this.scope.createLocalForm.confirmPassword.$error.server && this.scope.createLocalCtrb.confirmPassword.hasError;
                    };

                    Controller.prototype.isConfirmPasswordServerError = function () {
                        return this.scope.createLocalForm.confirmPassword.$error.server && this.scope.createLocalCtrb.confirmPassword.hasError;
                    };

                    Controller.prototype.userNameInputGroupValidationAddOnCssClass = function () {
                        return this.scope.createLocalCtrb.userName.hasFeedback() ? null : 'hide';
                    };

                    Controller.prototype.isUserNameRequiredError = function () {
                        return this.scope.createLocalForm.userName.$error.required && this.scope.createLocalCtrb.userName.hasError;
                    };

                    Controller.prototype.isUserNameServerError = function () {
                        return this.scope.createLocalForm.userName.$error.server && this.scope.createLocalCtrb.userName.hasError;
                    };

                    Controller.prototype.tokenInputGroupValidationAddOnCssClass = function () {
                        return this.scope.createLocalCtrb.token.hasFeedback() ? null : 'hide';
                    };

                    Controller.prototype.isTokenRequiredError = function () {
                        return this.scope.createLocalForm.token.$error.required && this.scope.createLocalCtrb.token.hasError;
                    };

                    Controller.prototype.isTokenServerError = function () {
                        return this.scope.createLocalForm.token.$error.server && this.scope.createLocalCtrb.token.hasError;
                    };

                    Controller.prototype.isSubmitWaiting = function () {
                        return this.scope.createLocalCtrb.isSubmitWaiting;
                    };

                    Controller.prototype.isSubmitError = function () {
                        return !this.isSubmitWaiting() && this.scope.createLocalCtrb.hasError;
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
                PasswordForm.Controller = Controller;

                PasswordForm.moduleName = 'sign-up-password-form';

                PasswordForm.ngModule = angular.module(PasswordForm.moduleName, [App.Modules.Tripod.moduleName]);
            })(SignUp.PasswordForm || (SignUp.PasswordForm = {}));
            var PasswordForm = SignUp.PasswordForm;
        })(Security.SignUp || (Security.SignUp = {}));
        var SignUp = Security.SignUp;
    })(App.Security || (App.Security = {}));
    var Security = App.Security;
})(App || (App = {}));
