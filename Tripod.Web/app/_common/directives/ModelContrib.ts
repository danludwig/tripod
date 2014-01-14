'use strict';

module App.Directives.ModelContrib {

    export var directiveName = 'modelContrib';

    export class Controller {

        hasError = false;
        hasSuccess = false;
        hasSpinner = false;
        error: any = {};
        ngModelController: ng.INgModelController;

        static $inject = ['$scope'];
        constructor(scope: ng.IScope) { }

        setValidity(validationErrorKey: string, validationErrorMessage: string): void {
            this.ngModelController.$setValidity(validationErrorKey, validationErrorMessage ? false : true);
            this.error[validationErrorKey] = validationErrorMessage;
        }

        spinnerCssClass(): string {
            return this.hasSpinner ? 'has-spinner' : null;
        }

        errorCssClass(): string {
            return this.hasError ? 'has-error' : null;
        }

        successCssClass(): string {
            return this.hasSuccess ? 'has-success' : null;
        }

        hasFeedback(): boolean {
            return this.hasError || this.hasSuccess || this.hasSpinner;
        }

        feedbackCssClass(): string {
            if (this.hasSpinner) return this.spinnerCssClass();
            if (this.hasError) return this.errorCssClass();
            if (this.hasSuccess) return this.successCssClass();
            return null;
        }

        inputGroupCssClass(size?: string): string {
            if (!this.hasFeedback()) return null;
            var cssClass = 'input-group';
            if (size) cssClass += ' input-group-' + size;
            return cssClass;
        }
    }

    //#region Directive

    var directiveFactory = (): () => ng.IDirective => {
        return (): ng.IDirective => {
            var directive: ng.IDirective = {
                name: directiveName,
                restrict: 'A', // attribute only
                require: [directiveName, 'ngModel', '^formContrib'],
                controller: Controller,
                compile: (): any => {
                    return {
                        pre: (scope: ng.IScope, element: JQuery, attr: ng.IAttributes, ctrls: any[]): void => {

                            // get the required controllers based on directive order
                            var modelContribCtrl: Controller = ctrls[0];
                            var modelCtrl: ng.INgModelController = ctrls[1];
                            var formContribCtrl: FormContrib.Controller = ctrls[2];
                            modelContribCtrl.ngModelController = modelCtrl;

                            // put this contrib controller on the scope
                            var alias = $.trim(attr['name']);
                            if (alias) formContribCtrl[alias] = modelContribCtrl;
                        },
                        post: (scope: ng.IScope, element: JQuery, attr: ng.IAttributes, ctrls: any[]): void => {

                            // get the required controllers based on directive order
                            var modelContribCtrl: Controller = ctrls[0];
                            var modelCtrl: ng.INgModelController = ctrls[1];
                            var formContribCtrl: FormContrib.Controller = ctrls[2];

                            // watch for form submissions, dirtiness, validity, and spinner
                            scope.$watch(
                                (): any[]=> {
                                    return [modelCtrl.$valid, modelCtrl.$dirty, formContribCtrl.isSubmitAttempted, modelContribCtrl.hasSpinner];
                                },
                                (): void => {
                                    // set error or success on model contrib controller
                                    var isDirtyOrSubmitAttempted = modelCtrl.$dirty || formContribCtrl.isSubmitAttempted;
                                    modelContribCtrl.hasError = !modelContribCtrl.hasSpinner && modelCtrl.$invalid && isDirtyOrSubmitAttempted;
                                    modelContribCtrl.hasSuccess = !modelContribCtrl.hasSpinner && modelCtrl.$valid && isDirtyOrSubmitAttempted;
                                }, true);
                        },
                    };
                },
            };
            return directive;
        };
    };

    //#endregion

    export var directive = directiveFactory();
}
