'use strict';
define(["require", "exports"], function(require, exports) {
    function rcSubmit($parse) {
        return {
            restrict: 'A',
            require: 'form',
            link: function ($scope, formElement, attributes, formController) {
                var fn = $parse(attributes.rcSubmit);

                formElement.bind('submit', function (event) {
                    // if form is not valid cancel it.
                    alert('still rc-submitting...');
                    if (!formController.$valid)
                        return false;

                    $scope.$apply(function () {
                        fn($scope, { $event: event });
                    });
                    return true;
                });
            }
        };
    }
    exports.rcSubmit = rcSubmit;

    exports.rcSubmit.$inject = ['$parse'];
});
