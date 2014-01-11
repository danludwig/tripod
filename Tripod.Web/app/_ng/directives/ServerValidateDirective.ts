'use strict';

import dModelHelper = require('./ModelHelperDirective');

export var directiveName = 'serverValidate';

export interface ServerValidateAttempt {
    value?: any;
    result?: ValidatedField;
}

export class ServerValidateController {

    modelController: ng.INgModelController;
    helpController: dModelHelper.ModelHelperController;

    constructor() { }

    attempts: ServerValidateAttempt[] = [];

    isFirstAttempt(): boolean {
        return !this.attempts.length;
    }

    getAttempt(value: string): ServerValidateAttempt {
        if (!this.attempts.length) return null;

        for (var i = 0; i < this.attempts.length; i++)
            if (this.attempts[i].value === value)
                return this.attempts[i];
        return null;
    }

    getLastAttempt(): ServerValidateAttempt {
        return this.attempts[this.attempts.length - 1];
    }

    setValidity(attempt: ServerValidateAttempt): void {
        this.modelController.$setValidity('server', attempt.result.isValid);
        if (attempt.result.isValid) {
            this.helpController.serverError = null;
        }
        else {
            this.helpController.serverError = attempt.result.errors[0].message;
        }
    }

}

//#region Directive

function failUnexpectedly(modelCtrl: ng.INgModelController, helpCtrl: dModelHelper.ModelHelperController): void {
    helpCtrl.serverError = 'An unexpected validation error has occurred.';
    modelCtrl.$setValidity('server', false);
}

var directiveFactory = (): any[]=> {
    // inject services
    return ['$http', '$timeout', ($http: ng.IHttpService, $timeout: ng.ITimeoutService): ng.IDirective => {
        var directive: ng.IDirective = {
            name: directiveName,
            restrict: 'A', // attribute only
            require: [directiveName, 'modelHelper', 'ngModel'], // need both controllers when compiling
            controller: ServerValidateController,
            link: (scope: ng.IScope, element: JQuery, attr: ng.IAttributes, ctrls: any[]): void => {

                var validateCtrl: ServerValidateController = ctrls[0];
                var helpCtrl: dModelHelper.ModelHelperController = ctrls[1];
                var modelCtrl: ng.INgModelController = ctrls[2];
                validateCtrl.helpController = helpCtrl;
                validateCtrl.modelController = modelCtrl;

                scope.$watch(
                    (): any => { return modelCtrl.$viewValue; }, // watch the ngModel $viewValue
                    (value: string): void => {

                        // the first time this watch executes, record the value as the first attempt
                        // and stop, to prevent the server from validating when first loaded
                        if (validateCtrl.isFirstAttempt()) {
                            validateCtrl.attempts.push({
                                value: value,
                                result: {
                                    isValid: true,
                                    errors: [{ message: null }],
                                },
                            });
                            return;
                        }

                        // check to see if this value has already been attempted
                        // and if it has, skip hitting the server
                        var attempt = validateCtrl.getAttempt(value);
                        if (attempt && attempt.result) {
                            validateCtrl.setValidity(attempt);
                            return;
                        }

                        // record all value attempts in this directive's controller
                        attempt = { value: value };
                        validateCtrl.attempts.push(attempt);

                        // tell the controller there is validation progress
                        // this should be throttled for cases when the server returns very quickly
                        var spinnerTimeoutPromise = $timeout((): void => {
                            helpCtrl.isServerValidating = true;
                        }, 20);

                        // set validity to true while we are validating
                        validateCtrl.setValidity({ result: { isValid: true } });

                        var url = attr[directiveName];
                        $http.post(url, { userName: value, })
                            .success((data: any): void => {

                                // expect the result to have a property with the same name as the validated field
                                var fieldName = attr['name'];
                                if (!fieldName || !data[fieldName])
                                    failUnexpectedly(modelCtrl, helpCtrl);

                                // load the result from the response data and store with attempt
                                var result: ValidatedField = data[fieldName];
                                attempt.result = result;
                                $timeout.cancel(spinnerTimeoutPromise);

                                // if this is not the last attempt, just record the result and skip
                                if (validateCtrl.getLastAttempt() !== attempt) {
                                    return;
                                }

                                helpCtrl.isServerValidating = false;
                                validateCtrl.setValidity(attempt);
                            })
                            .error((data: any, status: number): void => {
                                $timeout.cancel(spinnerTimeoutPromise);
                                helpCtrl.isServerValidating = false;

                                // when status is zero, user probably refreshed before this returned
                                if (status === 0) return;

                                // otherwise, something went wrong that we weren't expecting
                                failUnexpectedly(modelCtrl, helpCtrl);
                            });
                    });
            },
        };
        return directive;
    }];

};

//#endregion

export var directive = directiveFactory();