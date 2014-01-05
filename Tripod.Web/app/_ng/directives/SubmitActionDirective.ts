'use strict';

interface ISubmitActionAttributes extends ng.IAttributes {
    ngT3SubmitAction?: string;
    ngT3SubmitActionAttempted?: string;
    name?: string;
}

interface ISubmitActionScope extends ng.IScope {
    t3?: any;
}

export var directiveName = 'ngT3SubmitAction';

export function ngT3SubmitAction($parse: ng.IParseService): ng.IDirective {
    var directive: ng.IDirective = {
        restrict: 'A',
        require: ['ngT3SubmitAction', '?form'],
        controller: [SubmitActionController],
        compile: (): any => {
            return {
                pre: (scope: ISubmitActionScope, formElement: JQuery, attributes: ISubmitActionAttributes, controllers: any[]): void => {

                    var submitController: SubmitActionController = controllers[0];

                    var formController: ng.IFormController = (controllers.length > 1) ?
                        controllers[1] : null;
                    submitController.formController = formController;

                    scope.t3 = scope.t3 || {};
                    scope.t3[attributes.name] = submitController;
                },
                post: (scope: ng.IScope, formElement: JQuery, attributes: ISubmitActionAttributes, controllers: any[]): void => {

                    var submitController: SubmitActionController = controllers[0];
                    var formController: ng.IFormController = (controllers.length > 1) ?
                        controllers[1] : null;

                    var fn = $parse(attributes.ngT3SubmitAction);

                    formElement.bind('submit', (): boolean => {
                        submitController.attempted = true;
                        if (!scope.$$phase) scope.$apply();

                        if (!formController.$valid) return false;

                        scope.$apply((): void => {
                            fn(scope, { $event: event });
                        });
                        return true;
                    });

                    if (attributes.ngT3SubmitActionAttempted)
                        submitController.attempted = true;
                }
            };
        }
    };
    return directive;
}

ngT3SubmitAction.$inject = ['$parse'];

class SubmitActionController {

    attempted = false;
    formController: ng.IFormController;

    needsAttention(fieldModelController: ng.INgModelController): boolean {
        if (!this.formController) return false;

        if (fieldModelController)
            return fieldModelController.$invalid &&
                (fieldModelController.$dirty || this.attempted);

        return this.formController && this.formController.$invalid &&
            (this.formController.$dirty || this.attempted);
    }

    isGoodToGo(fieldModelController: ng.INgModelController): boolean {
        if (!this.formController) return false;
        if (this.needsAttention(fieldModelController)) return false;

        if (fieldModelController)
            return fieldModelController.$valid &&
                (fieldModelController.$dirty || this.attempted);

        return this.formController && this.formController.$valid &&
            (this.formController.$dirty || this.attempted);
    }
}
