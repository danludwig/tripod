'use strict';

import dFormHelper = require('./FormHelperDirective');

export var directiveName = 'modelHelper';

export class ModelHelperController {

    // keep a reference to the angular model controller
    modelController: ng.INgModelController;

    // keep a reference to the form helper controller
    formController: dFormHelper.FormHelperController;

    static $inject = ['$scope'];
    constructor(scope: ng.IScope) { }

    hasError(): boolean {
        return !this.serverValidating && this.modelController.$invalid &&
            (this.modelController.$dirty || this.formController.submitAttempted);
    }

    hasSuccess(): boolean {
        return !this.serverValidating && !this.hasError() && this.modelController.$valid &&
            (this.modelController.$dirty || this.formController.submitAttempted);
    }

    hasFeedback(): boolean {
        return this.hasError() || this.hasSuccess();
    }

    serverValidating = false;

    serverError: string;
}

//#region Directive

var directiveFactory = (): () => ng.IDirective => {
    return (): ng.IDirective => {
        var directive: ng.IDirective = {
            name: directiveName,
            restrict: 'A', // attribute only
            scope: true,
            require: [directiveName, 'ngModel', '^formHelper'],
            controller: ModelHelperController,
            link: (scope: ng.IScope, element: JQuery, attr: ng.IAttributes, ctrls: any[]): void => {

                // get the required controllers based on directive order
                var helpCtrl: ModelHelperController = ctrls[0];
                var modelCtrl: ng.INgModelController = ctrls[1];
                var formCtrl: dFormHelper.FormHelperController = ctrls[2];

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