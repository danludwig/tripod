'use strict';

module App.Directives.ServerError {
    export var directiveName = 'serverError';

    //#region Directive

    var directiveFactory = (): () => ng.IDirective => {
        return (): ng.IDirective => {
            var directive: ng.IDirective = {
                name: directiveName,
                restrict: 'A', // attribute only
                require: ['ngModel', 'modelContrib'],
                link: (scope: ng.IScope, element: JQuery, attr: ng.IAttributes, ctrls: any[]): void => {
                    // don't initialize this unless there is a value in the attribute
                    var serverError = attr['serverError'];
                    if (!serverError) return;

                    // passwords may not be valid on server, but will come back with empty box
                    var inputType = attr['type'];
                    var isPassword = inputType && inputType.toLowerCase() == 'password';

                    var modelCtrl: ng.INgModelController = ctrls[0];
                    var helpCtrl: ModelContrib.Controller = ctrls[1];

                    // set the server error text on the model helper controller
                    helpCtrl.serverError = serverError;

                    // initial watch to remove required error and set server error
                    var initialValue;
                    var initWatch = scope.$watch(
                        (): any => { return modelCtrl.$error; },
                        (value: any): void => {
                            initialValue = modelCtrl.$viewValue;
                            if (value.required && isPassword) {
                                modelCtrl.$setValidity('required', true);
                            }
                            modelCtrl.$setValidity('server', false);
                            initWatch(); // remove this watch now
                        });

                    // watch for changes and hide / show server error accordingly
                    scope.$watch(
                        (): any => { return modelCtrl.$viewValue; },
                        (value: any): void => {
                            // always remove the error message when input becomes dirty
                            if (modelCtrl.$dirty) {
                                modelCtrl.$setValidity('server', true);
                            }

                            // restore the error message when input becomes pristine
                            // unless it is a password
                            if (!isPassword && value === initialValue) {
                                modelCtrl.$setValidity('server', false);
                            }
                        });
                },
            };
            return directive;
        };
    };

    //#endregion

    export var directive = directiveFactory();
}

