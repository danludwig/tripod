'use strict';

module App.Directives.ModelHelper {

    export var directiveName = 'modelHelper';

    export class Controller {

        // keep a reference to the angular model controller
        modelController: ng.INgModelController;

        // keep a reference to the form helper controller
        formController: FormContrib.Controller;

        static $inject = ['$scope'];
        constructor(scope: ng.IScope) { }

        hasError(): boolean {
            return !this.isServerValidating && this.modelController.$invalid &&
                (this.modelController.$dirty || this.formController.isSubmitAttempted);
        }

        hasSuccess(): boolean {
            return !this.isNoSuccess && !this.isServerValidating && !this.hasError() && this.modelController.$valid &&
                (this.modelController.$dirty || this.formController.isSubmitAttempted);
        }

        hasFeedback(): boolean {
            return this.hasError() || this.hasSuccess() || this.hasSpinner();
        }

        hasSpinner(): boolean {
            return this.isServerValidating;
        }

        isServerValidating = false;
        isNoSuccess = false;

        serverError: string;
    }

    //#region Directive

    var directiveFactory = (): () => ng.IDirective => {
        return (): ng.IDirective => {
            var directive: ng.IDirective = {
                name: directiveName,
                restrict: 'A', // attribute only
                require: [directiveName, 'ngModel', '^formContrib'],
                controller: Controller,
                link: (scope: ng.IScope, element: JQuery, attr: ng.IAttributes, ctrls: any[]): void => {

                    // get the required controllers based on directive order
                    var helpCtrl: Controller = ctrls[0];
                    var modelCtrl: ng.INgModelController = ctrls[1];
                    var formCtrl: FormContrib.Controller = ctrls[2];

                    // give the helper controller access to the other controllers
                    helpCtrl.modelController = modelCtrl;
                    helpCtrl.formController = formCtrl;

                    // put the helper controller on the scope
                    var alias = $.trim(attr['name']);
                    if (alias) formCtrl[alias] = helpCtrl;
                },
            };
            return directive;
        };
    };

    //#endregion

    export var directive = directiveFactory();
}
