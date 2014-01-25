'use strict';

module App.Security.SignUpUserForm {

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
        signUpUserForm: Form;
        signUpUserCtrb: Contrib;
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
            return this.scope.signUpUserCtrb.password.hasFeedback() ? null : 'hide';
        }

        isPasswordRequiredError(): boolean {
            return this.scope.signUpUserForm.password.$error.required
                && this.scope.signUpUserCtrb.password.hasError;
        }

        isPasswordMinLengthError(): boolean {
            return this.scope.signUpUserForm.password.$error.minlength
                && this.scope.signUpUserCtrb.password.hasError;
        }

        isPasswordMaxLengthError(): boolean {
            return this.scope.signUpUserForm.password.$error.maxlength
                && this.scope.signUpUserCtrb.password.hasError;
        }

        isPasswordServerError(): boolean {
            return this.scope.signUpUserForm.password.$error.server
                && this.scope.signUpUserCtrb.password.hasError;
        }

        confirmPasswordInputGroupValidationAddOnCssClass(): string {
            return this.scope.signUpUserCtrb.confirmPassword.hasFeedback() ? null : 'hide';
        }

        isConfirmPasswordRequiredError(): boolean {
            return this.scope.signUpUserForm.confirmPassword.$error.required
                && this.scope.signUpUserCtrb.confirmPassword.hasError;
        }

        isConfirmPasswordEqualError(): boolean {
            return this.scope.signUpUserForm.confirmPassword.$error.equal
                && !this.scope.signUpUserForm.confirmPassword.$error.required
                && !this.scope.signUpUserForm.confirmPassword.$error.server
                && this.scope.signUpUserCtrb.confirmPassword.hasError;
        }

        isConfirmPasswordServerError(): boolean {
            return this.scope.signUpUserForm.confirmPassword.$error.server
                && this.scope.signUpUserCtrb.confirmPassword.hasError;
        }

        userNameInputGroupValidationAddOnCssClass(): string {
            return this.scope.signUpUserCtrb.userName.hasFeedback() ? null : 'hide';
        }

        isUserNameRequiredError(): boolean {
            return this.scope.signUpUserForm.userName.$error.required
                && this.scope.signUpUserCtrb.userName.hasError;
        }

        isUserNameServerError(): boolean {
            return this.scope.signUpUserForm.userName.$error.server
                && this.scope.signUpUserCtrb.userName.hasError;
        }

        tokenInputGroupValidationAddOnCssClass(): string {
            return this.scope.signUpUserCtrb.token.hasFeedback() ? null : 'hide';
        }

        isTokenRequiredError(): boolean {
            return this.scope.signUpUserForm.token.$error.required
                && this.scope.signUpUserCtrb.token.hasError;
        }

        isTokenServerError(): boolean {
            return this.scope.signUpUserForm.token.$error.server
                && this.scope.signUpUserCtrb.token.hasError;
        }

        isSubmitWaiting(): boolean {
            return this.scope.signUpUserCtrb.isSubmitWaiting;
        }

        isSubmitError(): boolean {
            return !this.isSubmitWaiting() && this.scope.signUpUserCtrb.hasError;
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

    export var moduleName = 'sign-up-register-form';

    export var ngModule = angular.module(moduleName, [Modules.Tripod.moduleName]);
}
