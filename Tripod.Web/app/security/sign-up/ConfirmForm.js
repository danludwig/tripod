'use strict';
var App;
(function (App) {
    (function (Security) {
        (function (SignUp) {
            (function (ConfirmForm) {
                var Controller = (function () {
                    function Controller(scope) {
                        this.scope = scope;
                        this.secret = '';
                        this.ticket = '';
                        this.purpose = '';
                        scope.vm = this;
                    }
                    Controller.prototype.secretInputGroupValidationAddOnCssClass = function () {
                        return this.scope.confirmCtrb.secret.hasFeedback() ? null : 'hide';
                    };

                    Controller.prototype.isSecretRequiredError = function () {
                        return this.scope.confirmForm.secret.$error.required && this.scope.confirmCtrb.secret.hasError;
                    };

                    Controller.prototype.isSecretServerError = function () {
                        return this.scope.confirmForm.secret.$error.server && this.scope.confirmCtrb.secret.hasError;
                    };

                    Controller.prototype.isTicketError = function () {
                        return this.scope.confirmCtrb.ticket.hasError && !this.scope.confirmCtrb.secret.hasError;
                    };

                    Controller.prototype.isTicketRequiredError = function () {
                        return this.scope.confirmForm.ticket.$error.required && this.isTicketError();
                    };

                    Controller.prototype.isTicketServerError = function () {
                        return this.scope.confirmForm.ticket.$error.server && this.isTicketError();
                    };

                    Controller.prototype.isSubmitWaiting = function () {
                        return this.scope.confirmCtrb.isSubmitWaiting;
                    };

                    Controller.prototype.isSubmitError = function () {
                        return !this.isSubmitWaiting() && this.scope.confirmCtrb.hasError;
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
                ConfirmForm.Controller = Controller;

                ConfirmForm.moduleName = 'sign-up-confirm-form';

                ConfirmForm.ngModule = angular.module(ConfirmForm.moduleName, [App.Modules.Tripod.moduleName]);
            })(SignUp.ConfirmForm || (SignUp.ConfirmForm = {}));
            var ConfirmForm = SignUp.ConfirmForm;
        })(Security.SignUp || (Security.SignUp = {}));
        var SignUp = Security.SignUp;
    })(App.Security || (App.Security = {}));
    var Security = App.Security;
})(App || (App = {}));
