'use strict';

import dModelHelper = require('./ModelHelperDirective');

export var directiveName = 'serverValidate';

export class ServerValidateController {

    //modelController: ng.INgModelController;
    //helpController: dModelHelper.ModelHelperController;

    constructor() { }
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

                //var validateCtrl: ServerValidateController = ctrls[0];
                var helpCtrl: dModelHelper.ModelHelperController = ctrls[1];
                var modelCtrl: ng.INgModelController = ctrls[2];
                //validateCtrl.modelController = modelCtrl;
                //validateCtrl.helpController = helpCtrl;

                var initialValue: any;
                scope.$watch(
                    (): any => {
                        initialValue = typeof initialValue === 'undefined' ? modelCtrl.$viewValue : initialValue;
                        return modelCtrl.$viewValue;
                    },
                    (value: any): void => {
                        // set server validity to true when model is pristine or equal to its initial value..?
                        if (modelCtrl.$pristine || modelCtrl.$viewValue == initialValue) {
                            modelCtrl.$setValidity('server', true);
                            return;
                        }

                        // tell the controller there is validation progress
                        // this needs throttled for cases when the server returns very quickly
                        var spinnerTimeoutPromise = $timeout((): void => {
                            helpCtrl.serverValidating = true;
                        }, 20);

                        //helpCtrl.serverValidating = false;
                        //helpCtrl.serverError = null;
                        //modelCtrl.$setValidity('server', true);
                        var url = attr[directiveName];
                        $http.post(url, { userName: value, }, {})
                            .success((data: any, status: number, headers: (headerName: string) => string, config: ng.IRequestConfig): void => {
                                $timeout.cancel(spinnerTimeoutPromise);
                                helpCtrl.serverValidating = false;

                                // expect the result to have a property with the same name as the validated field
                                var fieldName = attr['name'];
                                if (!fieldName || !data[fieldName])
                                    failUnexpectedly(modelCtrl, helpCtrl);

                                var result: ValidatedField = data[fieldName];
                                if (result.isValid) {
                                    helpCtrl.serverError = null;
                                    modelCtrl.$setValidity('server', true);
                                } else {
                                    helpCtrl.serverError = result.errors[0].message;
                                    modelCtrl.$setValidity('server', false);
                                }
                            })
                            .error((data: any, status: number, headers: (headerName: string) => string, config: ng.IRequestConfig): void => {
                                $timeout.cancel(spinnerTimeoutPromise);
                                helpCtrl.serverValidating = false;

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