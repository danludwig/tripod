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
        signUpEmailForm: Form;
        signUpEmailCtrb: Contrib;
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
            return this.scope.signUpEmailCtrb.emailAddress.hasFeedback() ? null : 'hide';
        }

        isEmailAddressRequiredError(): boolean {
            return this.scope.signUpEmailForm.emailAddress.$error.required
                && this.scope.signUpEmailCtrb.emailAddress.hasError;
        }

        isEmailAddressPatternError(): boolean {
            return this.scope.signUpEmailForm.emailAddress.$error.email
                && this.scope.signUpEmailCtrb.emailAddress.hasError;
        }

        isEmailAddressServerError(): boolean {
            return this.scope.signUpEmailForm.emailAddress.$error.server
                && this.scope.signUpEmailCtrb.emailAddress.hasError;
        }

        isExpectingEmailError(): boolean {
            return this.scope.signUpEmailCtrb.isExpectingEmail.hasError;
        }

        isExpectingEmailRequiredError(): boolean {
            return this.scope.signUpEmailForm.isExpectingEmail.$error.required
                && this.isExpectingEmailError();
        }

        isExpectingEmailServerError(): boolean {
            return this.scope.signUpEmailForm.isExpectingEmail.$error.server
                && this.isExpectingEmailError();
        }

        isSubmitWaiting(): boolean {
            return this.scope.signUpEmailCtrb.isSubmitWaiting;
        }

        isSubmitError(): boolean {
            return !this.isSubmitWaiting() && this.scope.signUpEmailCtrb.hasError;
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

    angular.bootstrap($('[ng-module=' + moduleName + ']'), [moduleName]);
}
