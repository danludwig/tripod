'use strict';

export var directiveName = 'formHelper';

export class FormHelperController {

    // tells us whether or not a form submit has been attempted
    submitAttempted = false;

    isSubmitDisabled = false;

    // keep a reference to the angular form controller
    formController: ng.IFormController;

    constructor() { }
}

//#region Directive

var directiveFactory = (): any[] => {
    // inject parse service
    return ['$parse', ($parse: ng.IParseService): ng.IDirective => {
        var directive: ng.IDirective = {
            name: directiveName,
            restrict: 'A', // attribute only
            require: [directiveName, 'form'], // need both controllers when compiling
            controller: [FormHelperController],
            compile: (): any => {
                return {
                    pre: (scope: ng.IScope, element: JQuery, attr: ng.IAttributes, ctrls: any[]): void => {

                        // get the required controllers based on directive order
                        var helpCtrl: FormHelperController = ctrls[0];
                        var formCtrl: ng.IFormController = ctrls[1];

                        // give the helper controller access to the form controller
                        helpCtrl.formController = formCtrl;

                        // initialize the form submission
                        if (attr['submitAttempted']) helpCtrl.submitAttempted = true;

                        // put the helper controller on the scope
                        var alias = $.trim(attr[directiveName]);
                        if (alias) scope[alias] = helpCtrl;
                    },
                    post: (scope: ng.IScope, element: JQuery, attr: ng.IAttributes, ctrls: any[]): void => {

                        // get the required controllers based on directive order
                        var helpCtrl: FormHelperController = ctrls[0];
                        var formCtrl: ng.IFormController = ctrls[1];

                        // this is in case the attribute has a submit action
                        //var fn = $parse(attr[directiveName]);

                        element.bind('submit', (): boolean => {

                            // record the fact that a form submission was attempted
                            helpCtrl.submitAttempted = true;
                            if (formCtrl.$valid)
                                helpCtrl.isSubmitDisabled = true;
                            if (!scope.$$phase) scope.$apply();

                            // prevent the form from being submitted if not valid
                            if (!formCtrl.$valid) return false;

                            //scope.$apply((): void => {
                            //    // invoke the submit action on the scope
                            //    fn(scope, { $event: event });
                            //});
                            return true;
                        });
                    },
                };
            },
        };
        return directive;
    }];

};

//#endregion

export var directive = directiveFactory();