'use strict';

import dModelHelper = require('./ModelHelperDirective');

export var directiveName = 'serverValidate';

export class ServerValidateController {

    constructor() { }
}

//#region Directive

var directiveFactory = (): any[] => {
    // inject services
    return ['$http', '$timeout', ($http: ng.IHttpService, $timeout: ng.ITimeoutService): ng.IDirective => {
        var directive: ng.IDirective = {
            name: directiveName,
            restrict: 'A', // attribute only
            scope: true,
            require: [directiveName, 'modelHelper', 'ngModel'], // need both controllers when compiling
            controller: ServerValidateController,
            link: (scope: ng.IScope, element: JQuery, attr: ng.IAttributes, ctrls: any[]): void => {

                var validateCtrl: ServerValidateController = ctrls[0];
                var helpCtrl: dModelHelper.ModelHelperController = ctrls[1];
                var modelCtrl: ng.INgModelController = ctrls[2];

                var initialValue: any;
                scope.$watch(
                    (): any => {
                        initialValue = typeof initialValue === 'undefined' ? modelCtrl.$viewValue : initialValue;
                        return modelCtrl.$viewValue;
                    },
                    (value: any): void => {
                        if (modelCtrl.$pristine || modelCtrl.$viewValue == initialValue) {
                            modelCtrl.$setValidity('server', true);
                            return;
                        }

                        helpCtrl.serverValidating = true;
                        var url = attr[directiveName];
                        $http.post(url, { userName: value, }, {})
                            .success((data: any, status: number, headers: (headerName: string) => string, config: ng.IRequestConfig): void => {
                                helpCtrl.serverValidating = false;
                                if (!data || !data.length) {
                                    modelCtrl.$setValidity('server', true);
                                    helpCtrl.serverError = null;
                                } else {
                                    helpCtrl.serverError = 'You should have shown the message returned by the server.';
                                    modelCtrl.$setValidity('server', false);
                                }
                            })
                            .error((data: any, status: number, headers: (headerName: string) => string, config: ng.IRequestConfig): void => {
                                helpCtrl.serverValidating = false;
                                helpCtrl.serverError = 'An unexpected validation error has occurred.';
                                modelCtrl.$setValidity('server', false);
                            });
                    });
            },
        };
        return directive;
    }];

};

//#endregion

export var directive = directiveFactory();