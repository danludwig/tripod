'use strict';

module App.Directives.FormContrib {

    export var directiveName = 'formContrib';

    export class Controller {

        // tells us whether or not a form submit has been attempted, even though it may be pristine
        isSubmitAttempted = false;

        // tells us whether the submit is waiting on something, and should not be clickable in the meantime
        isSubmitWaiting = false;

        hasError = false;
    }

    //#region Directive

    var directiveFactory = (): any[] => {
        // inject parse service
        return ['$parse', ($parse: ng.IParseService): ng.IDirective => {
            var d: ng.IDirective = {
                name: directiveName,
                restrict: 'A', // attribute only
                require: [directiveName, 'form'], // need both controllers when compiling
                controller: Controller,
                compile: (): any => {
                    return {
                        pre: (scope: ng.IScope, element: JQuery, attr: ng.IAttributes, ctrls: any[]): void => {

                            // get the required controllers based on directive order
                            var contribCtrl: Controller = ctrls[0];

                            // form may tell us that a submit has been attempted (rendered after full postback)
                            if (attr['formSubmitted']) contribCtrl.isSubmitAttempted = true;

                            // put the contrib controller on the scope
                            var alias = $.trim(attr[directiveName]);
                            if (alias) scope[alias] = contribCtrl;
                        },
                        post: (scope: ng.IScope, element: JQuery, attr: ng.IAttributes, ctrls: any[]): void => {

                            // get the required controllers based on directive order
                            var contribCtrl: Controller = ctrls[0];
                            var formCtrl: ng.IFormController = ctrls[1];

                            scope.$watch(
                                (): any[]=> {
                                    return [formCtrl.$valid, formCtrl.$dirty, contribCtrl.isSubmitAttempted];
                                },
                                (): void => {
                                    if (contribCtrl.isSubmitWaiting || formCtrl.$valid) {
                                        contribCtrl.hasError = false;
                                    }
                                    else if (formCtrl.$invalid && contribCtrl.isSubmitAttempted) {
                                        contribCtrl.hasError = true;
                                    }

                                }, true);

                            // this is in case the attribute has an angular method attached
                            var fn = $parse(attr['formSubmit']);

                            element.bind('submit', (): boolean => {

                                // record the fact that a form submission was attempted
                                contribCtrl.isSubmitAttempted = true;

                                // this submit may cause a full postback, in which case the button should be disabled
                                if (formCtrl.$valid)
                                    contribCtrl.isSubmitWaiting = true;
                                if (!scope.$$phase) scope.$apply();

                                // prevent the form from being submitted if not valid
                                if (!formCtrl.$valid) return false;

                                scope.$apply((): void => {
                                    // invoke the submit action on the scope
                                    fn(scope, { $event: event });
                                });

                                return true;
                            });
                        },
                    };
                },
            };
            return d;
        }];
    };

    //#endregion

    export var directive = directiveFactory();
}
