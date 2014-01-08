'use strict';

import dModelHelper = require('./ModelHelperDirective');

export var directiveName = 'serverError';

//#region Directive

var directiveFactory = () => {
    return (): ng.IDirective => {
        var directive: ng.IDirective = {
            name: directiveName,
            restrict: 'A', // attribute only
            require: ['ngModel', 'modelHelper'],
            link: (scope: ng.IScope, element: JQuery, attr: ng.IAttributes, ctrls: any[]): void => {
                // don't initialize this unless there is a value in the attribute
                var serverError = attr['serverError'];
                if (!serverError) return;

                // passwords may not be valid on server, but will come back with empty box
                var inputType = attr['type'];
                if (!inputType || inputType.toLowerCase() != 'password') return;

                var modelCtrl: ng.INgModelController = ctrls[0];
                var helpCtrl: dModelHelper.ModelHelperController = ctrls[1];

                // set the server error text on the model helper controller
                helpCtrl.serverError = serverError;

                // initial watch to remove required error and set server error
                var removeInitWatch = scope.$watch(
                    (): any => { return modelCtrl.$error; },
                    (value: any): void => {
                        if (value.required) {
                            modelCtrl.$setValidity('required', true);
                        }
                        modelCtrl.$setValidity('server', false);
                        removeInitWatch(); // remove this watch now
                    });

                // remove server error when view value becomes dirty
                var removeChangeWatch = scope.$watch(
                    (): any => { return modelCtrl.$viewValue; },
                    (): void => {
                        if (modelCtrl.$dirty) {
                            modelCtrl.$setValidity('server', true);
                            removeChangeWatch();
                        }
                    });
            },
        };
        return directive;
    };
};

//#endregion

export var directive = directiveFactory();