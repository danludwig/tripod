'use strict';

module App.Security.SignUp.PasswordForm {

    export interface Model {
        password?: string;
        confirmPassword?: string;
        userName?: string;
        token?: string;
    }

    export interface Form extends ng.IFormController {
        password: ng.INgModelController;
        confirmPassword: ng.INgModelController;
        userName: ng.INgModelController;
        token: ng.INgModelController;
    }

    export interface Contrib extends Directives.FormContrib.Controller {
        password: Directives.ModelContrib.Controller;
        confirmPassword: Directives.ModelContrib.Controller;
        userName: Directives.ModelContrib.Controller;
        token: Directives.ModelContrib.Controller;
    }

    export interface Scope extends ViewModelScope<Model> {
        createLocalForm: Form;
        createLocalCtrb: Contrib;
    }

    export class Controller implements Model {

        password: string = '';
        confirmPassword: string = '';
        userName: string = '';
        token: string = '';

        static $inject = ['$scope'];
        constructor(public scope: Scope) {
            scope.vm = this;
        }

        passwordInputGroupValidationAddOnCssClass(): string {
            return this.scope.createLocalCtrb.password.hasFeedback() ? null : 'hide';
        }

        isPasswordRequiredError(): boolean {
            return this.scope.createLocalForm.password.$error.required
                && this.scope.createLocalCtrb.password.hasError;
        }

        isPasswordMinLengthError(): boolean {
            return this.scope.createLocalForm.password.$error.minlength
                && this.scope.createLocalCtrb.password.hasError;
        }

        isPasswordMaxLengthError(): boolean {
            return this.scope.createLocalForm.password.$error.maxlength
                && this.scope.createLocalCtrb.password.hasError;
        }

        isPasswordServerError(): boolean {
            return this.scope.createLocalForm.password.$error.server
                && this.scope.createLocalCtrb.password.hasError;
        }

        confirmPasswordInputGroupValidationAddOnCssClass(): string {
            return this.scope.createLocalCtrb.confirmPassword.hasFeedback() ? null : 'hide';
        }

        isConfirmPasswordRequiredError(): boolean {
            return this.scope.createLocalForm.confirmPassword.$error.required
                && this.scope.createLocalCtrb.confirmPassword.hasError;
        }

        isConfirmPasswordEqualError(): boolean {
            return this.scope.createLocalForm.confirmPassword.$error.equal
                && !this.scope.createLocalForm.confirmPassword.$error.required
                && !this.scope.createLocalForm.confirmPassword.$error.server
                && this.scope.createLocalCtrb.confirmPassword.hasError;
        }

        isConfirmPasswordServerError(): boolean {
            return this.scope.createLocalForm.confirmPassword.$error.server
                && this.scope.createLocalCtrb.confirmPassword.hasError;
        }

        userNameInputGroupValidationAddOnCssClass(): string {
            return this.scope.createLocalCtrb.userName.hasFeedback() ? null : 'hide';
        }

        isUserNameRequiredError(): boolean {
            return this.scope.createLocalForm.userName.$error.required
                && this.scope.createLocalCtrb.userName.hasError;
        }

        isUserNameServerError(): boolean {
            return this.scope.createLocalForm.userName.$error.server
                && this.scope.createLocalCtrb.userName.hasError;
        }

        tokenInputGroupValidationAddOnCssClass(): string {
            return this.scope.createLocalCtrb.token.hasFeedback() ? null : 'hide';
        }

        isTokenRequiredError(): boolean {
            return this.scope.createLocalForm.token.$error.required
                && this.scope.createLocalCtrb.token.hasError;
        }

        isTokenServerError(): boolean {
            return this.scope.createLocalForm.token.$error.server
                && this.scope.createLocalCtrb.token.hasError;
        }

        isSubmitWaiting(): boolean {
            return this.scope.createLocalCtrb.isSubmitWaiting;
        }

        isSubmitError(): boolean {
            return !this.isSubmitWaiting() && this.scope.createLocalCtrb.hasError;
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

    export var moduleName = 'sign-up-password-form';

    export var ngModule = angular.module(moduleName, [Modules.Tripod.moduleName]);
}
