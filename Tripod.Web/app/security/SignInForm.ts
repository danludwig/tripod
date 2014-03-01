'use strict';

module App.Security.SignInForm {

    export interface Model {
        userNameOrVerifiedEmail?: string;
        password?: string;
        isPersistent?: boolean;
    }

    export interface Form extends ng.IFormController {
        userNameOrVerifiedEmail: ng.INgModelController;
        password: ng.INgModelController;
    }

    export interface Contrib extends Directives.FormContrib.Controller {
        userNameOrVerifiedEmail: Directives.ModelContrib.Controller;
        password: Directives.ModelContrib.Controller;
    }

    export interface Scope extends ViewModelScope<Model> {
        signInForm: Form;
        signInCtrb: Contrib;
    }

    export class Controller implements Model {

        userNameOrVerifiedEmail: string = '';
        password: string = '';
        isPersistent: boolean = false;

        static $inject = ['$scope'];
        constructor(public scope: Scope) {
            scope.vm = this;
        }

        userNameInputGroupValidationAddOnCssClass(): string {
            return this.scope.signInCtrb.userNameOrVerifiedEmail.hasFeedback() ? null : 'hide';
        }

        isUserNameRequiredError(): boolean {
            return this.scope.signInForm.userNameOrVerifiedEmail.$error.required
                && this.scope.signInCtrb.userNameOrVerifiedEmail.hasError;
        }

        isUserNameServerError(): boolean {
            return this.scope.signInForm.userNameOrVerifiedEmail.$error.server
                && this.scope.signInCtrb.userNameOrVerifiedEmail.hasError;
        }

        isPasswordError(): boolean {
            return this.scope.signInCtrb.password.hasError
                && (!this.scope.signInCtrb.userNameOrVerifiedEmail.hasError
                || this.scope.signInForm.userNameOrVerifiedEmail.$error.required);
        }

        passwordCssClass(): string {
            return this.isPasswordError() ? 'has-error' : null;
        }

        passwordInputGroupCssClass(cssClass: string): string {
            return this.isPasswordError() ? cssClass : null;
        }

        passwordInputGroupValidationAddOnCssClass(): string {
            return this.isPasswordError() ? null : 'hide';
        }

        isPasswordRequiredError(): boolean {
            return this.scope.signInForm.password.$error.required
                && this.isPasswordError();
        }

        isPasswordServerError(): boolean {
            return this.scope.signInForm.password.$error.server
                && this.isPasswordError();
        }

        isSubmitWaiting(): boolean {
            return this.scope.signInCtrb.isSubmitWaiting;
        }

        isSubmitError(): boolean {
            return !this.isSubmitWaiting() && this.scope.signInCtrb.hasError;
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

    export var moduleName = 'sign-in-form';

    export var ngModule = angular.module(moduleName, [Modules.Tripod.moduleName]);
}
