'use strict';
var App;
(function (App) {
    (function (Directives) {
        (function (InputPreFormatter) {
            InputPreFormatter.directiveName = 'input';

            var directiveFactory = function () {
                return function () {
                    var d = {
                        restrict: 'E',
                        require: '?ngModel',
                        link: function (scope, element, attr, ctrl) {
                            var inputType = angular.lowercase(attr['type']);

                            if (!ctrl || inputType === 'radio' || inputType === 'checkbox')
                                return;

                            ctrl.$formatters.unshift(function (value) {
                                if (ctrl.$invalid && angular.isUndefined(value) && typeof ctrl.$modelValue === 'string') {
                                    return ctrl.$modelValue;
                                } else {
                                    return value;
                                }
                            });
                        }
                    };
                    return d;
                };
            };

            InputPreFormatter.directive = directiveFactory();
        })(Directives.InputPreFormatter || (Directives.InputPreFormatter = {}));
        var InputPreFormatter = Directives.InputPreFormatter;
    })(App.Directives || (App.Directives = {}));
    var Directives = App.Directives;
})(App || (App = {}));
