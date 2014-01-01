'use strict';

export function rcSubmit($parse: ng.IParseService): ng.IDirective {
    return {
        restrict: 'A',
        require: 'form',
        link: ($scope: ng.IScope, formElement: JQuery, attributes: any, formController: ng.IFormController) => {
            var fn = $parse(attributes.rcSubmit);

            formElement.bind('submit', event => {
                // if form is not valid cancel it.
                alert('still rc-submitting...');
                if (!formController.$valid) return false;

                $scope.$apply(() => {
                    fn($scope, { $event: event });
                });
                return true;
            });
        }
    };
}

rcSubmit.$inject = ['$parse'];
