'use strict';
var App;
(function (App) {
    (function (Security) {
        (function (SignUpSecretForm) {
            var Controller = (function () {
                function Controller(scope) {
                    this.scope = scope;
                    this.secret = '';
                    this.ticket = '';
                    this.purpose = '';
                    scope.vm = this;
                }
                Controller.prototype.secretInputGroupValidationAddOnCssClass = function () {
                    return this.scope.signUpSecretCtrb.secret.hasFeedback() ? null : 'hide';
                };

                Controller.prototype.isSecretRequiredError = function () {
                    return this.scope.signUpSecretForm.secret.$error.required && this.scope.signUpSecretCtrb.secret.hasError;
                };

                Controller.prototype.isSecretServerError = function () {
                    return this.scope.signUpSecretForm.secret.$error.server && this.scope.signUpSecretCtrb.secret.hasError;
                };

                Controller.prototype.isTicketError = function () {
                    return this.scope.signUpSecretCtrb.ticket.hasError && !this.scope.signUpSecretCtrb.secret.hasError;
                };

                Controller.prototype.isTicketRequiredError = function () {
                    return this.scope.signUpSecretForm.ticket.$error.required && this.isTicketError();
                };

                Controller.prototype.isTicketServerError = function () {
                    return this.scope.signUpSecretForm.ticket.$error.server && this.isTicketError();
                };

                Controller.prototype.isSubmitWaiting = function () {
                    return this.scope.signUpSecretCtrb.isSubmitWaiting;
                };

                Controller.prototype.isSubmitError = function () {
                    return !this.isSubmitWaiting() && this.scope.signUpSecretCtrb.hasError;
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
            SignUpSecretForm.Controller = Controller;

            SignUpSecretForm.moduleName = 'sign-up-confirm-form';

            SignUpSecretForm.ngModule = angular.module(SignUpSecretForm.moduleName, [App.Modules.Tripod.moduleName]);
        })(Security.SignUpSecretForm || (Security.SignUpSecretForm = {}));
        var SignUpSecretForm = Security.SignUpSecretForm;
    })(App.Security || (App.Security = {}));
    var Security = App.Security;
})(App || (App = {}));
