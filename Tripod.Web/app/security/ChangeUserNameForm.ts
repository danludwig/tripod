'use strict';

module App.Security.ChangeUserNameForm {

    export interface Model {
        userName?: string;
    }

    export interface Form extends ng.IFormController {
        userName: ng.INgModelController;
    }

    export interface Contrib extends Directives.FormContrib.Controller {
        userName: Directives.ModelContrib.Controller;
    }

    export interface Scope extends ViewModelScope<Model> {
        changeUserNameForm: Form;
        changeUserNameCtrb: Contrib;
    }

    export class Controller implements Model {

        userName: string = '';
        originalUserName: string = '';

        static $inject = ['$scope'];
        constructor(public scope: Scope) {
            scope.vm = this;
        }

        restoreOrigialUserName(e: ng.IAngularEvent): boolean {
            this.userName = this.originalUserName;
            e.preventDefault();
            return false;
        }

        userNameInputGroupValidationAddOnCssClass(): string {
            return this.scope.changeUserNameCtrb.userName.hasFeedback() ? null : 'hide';
        }

        isUserNameRequiredError(): boolean {
            return this.scope.changeUserNameForm.userName.$error.required
                && this.scope.changeUserNameCtrb.userName.hasError;
        }

        isUserNameServerError(): boolean {
            return this.scope.changeUserNameForm.userName.$error.server
                && this.scope.changeUserNameCtrb.userName.hasError;
        }

        isSubmitWaiting(): boolean {
            return this.scope.changeUserNameCtrb.isSubmitWaiting;
        }

        isSubmitError(): boolean {
            return !this.isSubmitWaiting() && this.scope.changeUserNameCtrb.hasError;
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

    export var moduleName = 'change-username-form';

    export var ngModule = angular.module(moduleName, [Modules.Tripod.moduleName]);
}
