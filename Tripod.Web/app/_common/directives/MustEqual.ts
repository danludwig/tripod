'use strict';

module App.Directives.MustEqual {

    export var directiveName = 'mustEqual';

    var directiveFactory = (): any[] => {
        return ['$parse', ($parse: ng.IParseService): ng.IDirective => {
            var d: ng.IDirective = {
                name: directiveName,
                restrict: 'A',
                require: 'ngModel',
                link: (scope: ng.IScope, element: JQuery, attr: ng.IAttributes, ctrl: ng.INgModelController) => {
                    if (!ctrl || !attr[directiveName]) return;

                    var other = $parse(attr[directiveName]);
                    var condition = $parse(attr[directiveName + 'When']);
                    var validator = (value: any): any => {
                        var conditionTest = condition(scope);
                        var otherValue = conditionTest ? other(scope) : value;
                        var isValid = value === otherValue || ctrl.$error.required;
                        ctrl.$setValidity('equal', isValid);
                        return value;
                    };

                    ctrl.$parsers.unshift(validator);
                    ctrl.$formatters.push(validator);
                    attr.$observe(directiveName, (): void => {
                        validator(ctrl.$viewValue);
                    });
                    scope.$watch(attr[directiveName + 'When'], () => {
                        validator(ctrl.$viewValue);
                    }, true);
                }
            };
            return d;
        }];
    };

    export var directive = directiveFactory();
}