'use strict';

interface IInputPreFormatterAttributes extends ng.IAttributes {
    type?: string;
}

export function ngT3InputPreFormatter(): ng.IDirective {
    var directive: ng.IDirective = {
        restrict: 'E',
        require: '?ngModel',
        link: ($scope: ng.IScope, $element: JQuery,
            $attrs: IInputPreFormatterAttributes,
            ngModelController: ng.INgModelController): any => {

            var inputType = angular.lowercase($attrs.type);

            if (!ngModelController || inputType === 'radio' || inputType === 'checkbox')
                return;

            ngModelController.$formatters.unshift((value: string): string => {
                if (ngModelController.$invalid && angular.isUndefined(value)
                    && typeof ngModelController.$modelValue === 'string') {
                    return ngModelController.$modelValue;
                } else {
                    return value;
                }
            });
        }
    };
    return directive;
}

