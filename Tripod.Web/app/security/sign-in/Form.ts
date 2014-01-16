'use strict';

module App.Security.SignIn.Form {

    export interface Model {
        userName?: string;
        password?: string;
        isPersistent?: boolean;
    }

    export interface Form extends ng.IFormController {
        userName: ng.INgModelController;
        password: ng.INgModelController;
    }

    export interface Contrib extends Directives.FormContrib.Controller {
        userName: Directives.ModelContrib.Controller;
        password: Directives.ModelContrib.Controller;
    }

    export interface Scope extends ViewModelScope<Model> {
        signInForm: Form;
        signInCtrb: Contrib;
    }

    export class Controller implements Model {

        userName: string = '';
        password: string = '';
        isPersistent: boolean = false;

        static $inject = ['$scope'];
        constructor(public scope: Scope) {
            scope.vm = this;
        }

        userNameInputGroupValidationAddOnCssClass(): string {
            return this.scope.signInCtrb.userName.hasFeedback() ? null : 'hide';
        }

        isUserNameRequiredError(): boolean {
            return this.scope.signInForm.userName.$error.required
                && this.scope.signInCtrb.userName.hasError;
        }

        isUserNameServerError(): boolean {
            return this.scope.signInForm.userName.$error.server
                && this.scope.signInCtrb.userName.hasError;
        }

        isPasswordError(): boolean {
            return this.scope.signInCtrb.password.hasError
                && !this.scope.signInCtrb.userName.hasError;
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
    }

    export var moduleName = 'sign-in-form';

    export var ngModule = angular.module(moduleName, [Modules.Tripod.moduleName]);
}
