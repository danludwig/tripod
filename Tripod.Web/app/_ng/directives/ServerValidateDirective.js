'use strict';
define(["require", "exports"], function(require, exports) {
    exports.directiveName = 'serverValidate';

    var ServerValidateController = (function () {
        function ServerValidateController() {
        }
        return ServerValidateController;
    })();
    exports.ServerValidateController = ServerValidateController;

    //#region Directive
    var directiveFactory = function () {
        // inject services
        return [
            '$http', '$timeout', function ($http, $timeout) {
                var directive = {
                    name: exports.directiveName,
                    restrict: 'A',
                    require: [exports.directiveName, 'modelHelper', 'ngModel'],
                    controller: ServerValidateController,
                    link: function (scope, element, attr, ctrls) {
                        var validateCtrl = ctrls[0];
                        var helpCtrl = ctrls[1];
                        var modelCtrl = ctrls[2];

                        var initialValue;
                        scope.$watch(function () {
                            initialValue = typeof initialValue === 'undefined' ? modelCtrl.$viewValue : initialValue;
                            return modelCtrl.$viewValue;
                        }, function (value) {
                            if (modelCtrl.$pristine || modelCtrl.$viewValue == initialValue) {
                                modelCtrl.$setValidity('server', true);
                                return;
                            }

                            helpCtrl.serverValidating = true;
                            var url = attr[exports.directiveName];
                            $http.post(url, { userName: value }, {}).success(function (data, status, headers, config) {
                                helpCtrl.serverValidating = false;
                                if (!data || !data.length) {
                                    modelCtrl.$setValidity('server', true);
                                    helpCtrl.serverError = null;
                                } else {
                                    helpCtrl.serverError = 'You should have shown the message returned by the server.';
                                    modelCtrl.$setValidity('server', false);
                                }
                            }).error(function (data, status, headers, config) {
                                // when status is zero, user probably refreshed before this returned
                                if (status === 0)
                                    return;
                                helpCtrl.serverValidating = false;
                                helpCtrl.serverError = 'An unexpected validation error has occurred.';
                                modelCtrl.$setValidity('server', false);
                            });
                        });
                    }
                };
                return directive;
            }];
    };

    //#endregion
    exports.directive = directiveFactory();
});
