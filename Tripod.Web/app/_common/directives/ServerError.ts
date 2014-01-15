'use strict';

// For displaying server-side error messages that are initially rendered by the server.
// This will usually be used when the page is rendered as the result of a postback.
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
                    if (!attr[directiveName]) return;

                    // passwords may not be valid on server, but will come back with empty box
                    var inputType = attr['type'];
                    var isPassword = inputType && inputType.toLowerCase() == 'password';

                    var modelCtrl: ng.INgModelController = ctrls[0];
                    var modelContribCtrl: ModelContrib.Controller = ctrls[1];

                    // set the server error text on the model contrib controller
                    modelContribCtrl.setValidity('server', attr[directiveName]);

                    var setValidity = (error: string): void => {
                        if (!error) {
                            modelContribCtrl.setValidity('server', null);
                        }
                        else {
                            modelContribCtrl.setValidity('server', error);

                            // clear out all other validators
                            for (var validationErrorKey in modelCtrl.$error) {
                                if (validationErrorKey === 'server') continue;
                                if (!modelCtrl.$error.hasOwnProperty(validationErrorKey)) continue;
                                modelCtrl.$setValidity(validationErrorKey, true);
                            }
                        }
                    };

                    // initial watch to remove required error and set server error
                    var initialValue: string;
                    var initWatch = scope.$watch(
                        (): any => { return modelCtrl.$viewValue; },
                        (value: string): void => {
                            // stash the initial value so we can redisplay server error if input returns to this state
                            initialValue = value;
                            setValidity(attr[directiveName]);
                            initWatch(); // remove this watch now
                        });

                    // watch for changes and hide / show server error accordingly
                    scope.$watch(
                        (): any => { return modelCtrl.$viewValue; },
                        (value: any): void => {
                            // always remove the error message when input becomes dirty
                            if (modelCtrl.$dirty && modelContribCtrl.error.server == attr[directiveName] && !modelContribCtrl.hasSpinner) {
                                setValidity(null);
                            }

                            // restore the error message when input reverts to initial value
                            // unless it is a password
                            if (!isPassword && value === initialValue) {
                                setValidity(attr[directiveName]);
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

