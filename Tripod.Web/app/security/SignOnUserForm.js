'use strict';
var App;
(function (App) {
    (function (Security) {
        (function (SignOnUserForm) {
            var Controller = (function () {
                function Controller(scope) {
                    this.scope = scope;
                    this.userName = '';
                    this.token = '';
                    scope.vm = this;
                }
                Controller.prototype.userNameInputGroupValidationAddOnCssClass = function () {
                    return this.scope.signOnUserCtrb.userName.hasFeedback() ? null : 'hide';
                };

                Controller.prototype.isUserNameRequiredError = function () {
                    return this.scope.signOnUserForm.userName.$error.required && this.scope.signOnUserCtrb.userName.hasError;
                };

                Controller.prototype.isUserNameServerError = function () {
                    return this.scope.signOnUserForm.userName.$error.server && this.scope.signOnUserCtrb.userName.hasError;
                };

                Controller.prototype.tokenInputGroupValidationAddOnCssClass = function () {
                    return this.scope.signOnUserCtrb.token.hasFeedback() ? null : 'hide';
                };

                Controller.prototype.isTokenRequiredError = function () {
                    return this.scope.signOnUserForm.token.$error.required && this.scope.signOnUserCtrb.token.hasError;
                };

                Controller.prototype.isTokenServerError = function () {
                    return this.scope.signOnUserForm.token.$error.server && this.scope.signOnUserCtrb.token.hasError;
                };

                Controller.prototype.isSubmitWaiting = function () {
                    return this.scope.signOnUserCtrb.isSubmitWaiting;
                };

                Controller.prototype.isSubmitError = function () {
                    return !this.isSubmitWaiting() && this.scope.signOnUserCtrb.hasError;
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
            SignOnUserForm.Controller = Controller;

            SignOnUserForm.moduleName = 'sign-on-register-form';

            SignOnUserForm.ngModule = angular.module(SignOnUserForm.moduleName, [App.Modules.Tripod.moduleName]);
        })(Security.SignOnUserForm || (Security.SignOnUserForm = {}));
        var SignOnUserForm = Security.SignOnUserForm;
    })(App.Security || (App.Security = {}));
    var Security = App.Security;
})(App || (App = {}));
