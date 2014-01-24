'use strict';

module App.Security.SignUpSecretForm {

    export interface Model {
        secret?: string;
        ticket?: string;
        purpose?: string;
    }

    export interface Form extends ng.IFormController {
        secret: ng.INgModelController;
        ticket: ng.INgModelController;
        purpose: ng.INgModelController;
    }

    export interface Contrib extends Directives.FormContrib.Controller {
        secret: Directives.ModelContrib.Controller;
        ticket: Directives.ModelContrib.Controller;
        purpose: Directives.ModelContrib.Controller;
    }

    export interface Scope extends ViewModelScope<Model> {
        signUpSecretForm: Form;
        signUpSecretCtrb: Contrib;
    }

    export class Controller implements Model {

        secret: string = '';
        ticket: string = '';
        purpose: string = '';

        static $inject = ['$scope'];
        constructor(public scope: Scope) {
            scope.vm = this;
        }

        secretInputGroupValidationAddOnCssClass(): string {
            return this.scope.signUpSecretCtrb.secret.hasFeedback() ? null : 'hide';
        }

        isSecretRequiredError(): boolean {
            return this.scope.signUpSecretForm.secret.$error.required
                && this.scope.signUpSecretCtrb.secret.hasError;
        }

        isSecretServerError(): boolean {
            return this.scope.signUpSecretForm.secret.$error.server
                && this.scope.signUpSecretCtrb.secret.hasError;
        }

        isTicketError(): boolean {
            return this.scope.signUpSecretCtrb.ticket.hasError
                && !this.scope.signUpSecretCtrb.secret.hasError;
        }

        isTicketRequiredError(): boolean {
            return this.scope.signUpSecretForm.ticket.$error.required
                && this.isTicketError();
        }

        isTicketServerError(): boolean {
            return this.scope.signUpSecretForm.ticket.$error.server
                && this.isTicketError();
        }

        isSubmitWaiting(): boolean {
            return this.scope.signUpSecretCtrb.isSubmitWaiting;
        }

        isSubmitError(): boolean {
            return !this.isSubmitWaiting() && this.scope.signUpSecretCtrb.hasError;
        }

        isSubmitReady(): boolean {
            return !this.isSubmitWaiting() && !this.isSubmitError();
        }

        isSubmitDisabled(): boolean {
            return this.isSubmitWaiting() || this.isSubmitError();
        }

        submitCssClass(): string {
            return this.isSubmitError() ? 'btn-danger' : null;
        }
    }

    export var moduleName = 'sign-up-confirm-form';

    export var ngModule = angular.module(moduleName, [Modules.Tripod.moduleName]);
}
