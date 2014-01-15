'use strict';
var App;
(function (App) {
    (function (Directives) {
        (function (ServerError) {
            ServerError.directiveName = 'serverError';

            var directiveFactory = function () {
                return function () {
                    var directive = {
                        name: ServerError.directiveName,
                        restrict: 'A',
                        require: ['ngModel', 'modelContrib'],
                        link: function (scope, element, attr, ctrls) {
                            if (!attr[ServerError.directiveName])
                                return;

                            var inputType = attr['type'];
                            var isPassword = inputType && inputType.toLowerCase() == 'password';

                            var modelCtrl = ctrls[0];
                            var modelContribCtrl = ctrls[1];

                            modelContribCtrl.setValidity('server', attr[ServerError.directiveName]);

                            var setValidity = function (error) {
                                if (!error) {
                                    modelContribCtrl.setValidity('server', null);
                                } else {
                                    modelContribCtrl.setValidity('server', error);

                                    for (var validationErrorKey in modelCtrl.$error) {
                                        if (validationErrorKey === 'server')
                                            continue;
                                        if (!modelCtrl.$error.hasOwnProperty(validationErrorKey))
                                            continue;
                                        modelCtrl.$setValidity(validationErrorKey, true);
                                    }
                                }
                            };

                            var initialValue;
                            var initWatch = scope.$watch(function () {
                                return modelCtrl.$viewValue;
                            }, function (value) {
                                initialValue = value;
                                setValidity(attr[ServerError.directiveName]);
                                initWatch();
                            });

                            scope.$watch(function () {
                                return modelCtrl.$viewValue;
                            }, function (value) {
                                if (modelCtrl.$dirty && modelContribCtrl.error.server == attr[ServerError.directiveName] && !modelContribCtrl.hasSpinner) {
                                    setValidity(null);
                                }

                                if (!isPassword && value === initialValue) {
                                    setValidity(attr[ServerError.directiveName]);
                                }
                            });
                        }
                    };
                    return directive;
                };
            };

            ServerError.directive = directiveFactory();
        })(Directives.ServerError || (Directives.ServerError = {}));
        var ServerError = Directives.ServerError;
    })(App.Directives || (App.Directives = {}));
    var Directives = App.Directives;
})(App || (App = {}));
