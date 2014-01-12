'use strict';

module App.Directives.Input {

    export var directiveName = 'input';

    var directiveFactory = (): () => ng.IDirective => {
        return (): ng.IDirective => {
            var directive: ng.IDirective = {
                restrict: 'E',
                require: '?ngModel',
                link: (scope: ng.IScope, element: JQuery, attr: ng.IAttributes, ctrl: ng.INgModelController): any => {

                    var inputType = angular.lowercase(attr['type']);

                    if (!ctrl || inputType === 'radio' || inputType === 'checkbox')
                        return;

                    ctrl.$formatters.unshift((value: string): string => {
                        if (ctrl.$invalid && angular.isUndefined(value) && typeof ctrl.$modelValue === 'string') {
                            return ctrl.$modelValue;
                        } else {
                            return value;
                        }
                    });
                }
            };
            return directive;
        };
    };

    export var directive = directiveFactory();
}
