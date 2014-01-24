'use strict';

module App.Security.SignUpEmailForm {

    export interface Model {
        emailAddress?: string;
        isExpectingEmail?: boolean;
        purpose?: number;
    }

    export interface Form extends ng.IFormController {
        emailAddress: ng.INgModelController;
        isExpectingEmail: ng.INgModelController;
        purpose: ng.INgModelController;
    }

    export interface Contrib extends Directives.FormContrib.Controller {
        emailAddress: Directives.ModelContrib.Controller;
        isExpectingEmail: Directives.ModelContrib.Controller;
        purpose: Directives.ModelContrib.Controller;
    }

    export interface Scope extends ViewModelScope<Model> {
        signUpForm: Form;
        signUpCtrb: Contrib;
    }

    export class Controller implements Model {

        emailAddress: string = '';
        isExpectingEmail: boolean = false;
        purpose: number = 0;

        static $inject = ['$scope'];
        constructor(public scope: Scope) {
            scope.vm = this;
        }

        emailAddressInputGroupValidationAddOnCssClass(): string {
            return this.scope.signUpCtrb.emailAddress.hasFeedback() ? null : 'hide';
        }

        isEmailAddressRequiredError(): boolean {
            return this.scope.signUpForm.emailAddress.$error.required
                && this.scope.signUpCtrb.emailAddress.hasError;
        }

        isEmailAddressPatternError(): boolean {
            return this.scope.signUpForm.emailAddress.$error.email
                && this.scope.signUpCtrb.emailAddress.hasError;
        }

        isEmailAddressServerError(): boolean {
            return this.scope.signUpForm.emailAddress.$error.server
                && this.scope.signUpCtrb.emailAddress.hasError;
        }

        isExpectingEmailError(): boolean {
            return this.scope.signUpCtrb.isExpectingEmail.hasError;
        }

        isExpectingEmailRequiredError(): boolean {
            return this.scope.signUpForm.isExpectingEmail.$error.required
                && this.isExpectingEmailError();
        }

        isExpectingEmailServerError(): boolean {
            return this.scope.signUpForm.isExpectingEmail.$error.server
                && this.isExpectingEmailError();
        }

        isSubmitWaiting(): boolean {
            return this.scope.signUpCtrb.isSubmitWaiting;
        }

        isSubmitError(): boolean {
            return !this.isSubmitWaiting() && this.scope.signUpCtrb.hasError;
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

    export var moduleName = 'sign-up-form';

    export var ngModule = angular.module(moduleName, [Modules.Tripod.moduleName]);
}
