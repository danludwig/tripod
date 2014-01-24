'use strict';
var App;
(function (App) {
    (function (Directives) {
        (function (MustEqual) {
            MustEqual.directiveName = 'mustEqual';

            var directiveFactory = function () {
                return [
                    '$parse', function ($parse) {
                        var d = {
                            name: MustEqual.directiveName,
                            restrict: 'A',
                            require: 'ngModel',
                            link: function (scope, element, attr, ctrl) {
                                if (!ctrl || !attr[MustEqual.directiveName])
                                    return;

                                var other = $parse(attr[MustEqual.directiveName]);
                                var condition = $parse(attr[MustEqual.directiveName + 'When']);
                                var validator = function (value) {
                                    var conditionTest = condition(scope);
                                    var otherValue = conditionTest ? other(scope) : value;
                                    var isValid = value === otherValue || ctrl.$error.required;
                                    ctrl.$setValidity('equal', isValid);
                                    return value;
                                };

                                ctrl.$parsers.unshift(validator);
                                ctrl.$formatters.push(validator);
                                attr.$observe(MustEqual.directiveName, function () {
                                    validator(ctrl.$viewValue);
                                });
                                scope.$watch(attr[MustEqual.directiveName + 'When'], function () {
                                    validator(ctrl.$viewValue);
                                }, true);
                            }
                        };
                        return d;
                    }];
            };

            MustEqual.directive = directiveFactory();
        })(Directives.MustEqual || (Directives.MustEqual = {}));
        var MustEqual = Directives.MustEqual;
    })(App.Directives || (App.Directives = {}));
    var Directives = App.Directives;
})(App || (App = {}));
