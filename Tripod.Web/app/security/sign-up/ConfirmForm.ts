'use strict';

module App.Security.SignUp.ConfirmForm {

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
        confirmForm: Form;
        confirmCtrb: Contrib;
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
            return this.scope.confirmCtrb.secret.hasFeedback() ? null : 'hide';
        }

        isSecretRequiredError(): boolean {
            return this.scope.confirmForm.secret.$error.required
                && this.scope.confirmCtrb.secret.hasError;
        }

        isSecretServerError(): boolean {
            return this.scope.confirmForm.secret.$error.server
                && this.scope.confirmCtrb.secret.hasError;
        }

        isTicketError(): boolean {
            return this.scope.confirmCtrb.ticket.hasError
                && !this.scope.confirmCtrb.secret.hasError;
        }

        isTicketRequiredError(): boolean {
            return this.scope.confirmForm.ticket.$error.required
                && this.isTicketError();
        }

        isTicketServerError(): boolean {
            return this.scope.confirmForm.ticket.$error.server
                && this.isTicketError();
        }
    }

    export var moduleName = 'sign-up-confirm-form';

    export var ngModule = angular.module(moduleName, [Modules.Tripod.moduleName]);
}
