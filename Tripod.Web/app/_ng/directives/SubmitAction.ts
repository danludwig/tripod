'use strict';

module App.Directives.SubmitAction {

    export var directiveName = 'ngT3SubmitAction';

    class Controller {

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

    var directiveFactory = (): any[]=> {
        // inject services
        return ['$parse', ($parse: ng.IParseService): ng.IDirective => {
            var directive: ng.IDirective = {
                name: directiveName,
                restrict: 'A',
                require: ['ngT3SubmitAction', '?form'],
                controller: Controller,
                compile: (): any => {
                    return {
                        pre: (scope: ng.IScope, formElement: JQuery, attributes: ng.IAttributes, controllers: any[]): void => {

                            var submitController: Controller = controllers[0];

                            var formController: ng.IFormController = (controllers.length > 1) ?
                                controllers[1] : null;
                            submitController.formController = formController;

                            scope['t3'] = scope['t3'] || {};
                            scope['t3'][attributes['name']] = submitController;
                        },
                        post: (scope: ng.IScope, formElement: JQuery, attributes: ng.IAttributes, controllers: any[]): void => {

                            var submitController: Controller = controllers[0];
                            var formController: ng.IFormController = (controllers.length > 1) ?
                                controllers[1] : null;

                            var fn = $parse(attributes[directiveName]);

                            formElement.bind('submit', (): boolean => {
                                submitController.attempted = true;
                                if (!scope.$$phase) scope.$apply();

                                if (!formController.$valid) return false;

                                scope.$apply((): void => {
                                    fn(scope, { $event: event });
                                });
                                return true;
                            });

                            if (attributes['ngT3SubmitActionAttempted'])
                                submitController.attempted = true;
                        }
                    };
                }
            };
            return directive;
        }];
    };

    //#endregion

    export var directive = directiveFactory();
}
