'use strict';
var App;
(function (App) {
    (function (Security) {
        (function (ChangeUserNameForm) {
            var Controller = (function () {
                function Controller(scope) {
                    this.scope = scope;
                    this.userName = '';
                    scope.vm = this;
                }
                Controller.prototype.userNameInputGroupValidationAddOnCssClass = function () {
                    return this.scope.changeUserNameCtrb.userName.hasFeedback() ? null : 'hide';
                };

                Controller.prototype.isUserNameRequiredError = function () {
                    return this.scope.changeUserNameForm.userName.$error.required && this.scope.changeUserNameCtrb.userName.hasError;
                };

                Controller.prototype.isUserNameServerError = function () {
                    return this.scope.changeUserNameForm.userName.$error.server && this.scope.changeUserNameCtrb.userName.hasError;
                };

                Controller.prototype.isSubmitWaiting = function () {
                    return this.scope.changeUserNameCtrb.isSubmitWaiting;
                };

                Controller.prototype.isSubmitError = function () {
                    return !this.isSubmitWaiting() && this.scope.changeUserNameCtrb.hasError;
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
            ChangeUserNameForm.Controller = Controller;

            ChangeUserNameForm.moduleName = 'change-username-form';

            ChangeUserNameForm.ngModule = angular.module(ChangeUserNameForm.moduleName, [App.Modules.Tripod.moduleName]);
        })(Security.ChangeUserNameForm || (Security.ChangeUserNameForm = {}));
        var ChangeUserNameForm = Security.ChangeUserNameForm;
    })(App.Security || (App.Security = {}));
    var Security = App.Security;
})(App || (App = {}));
