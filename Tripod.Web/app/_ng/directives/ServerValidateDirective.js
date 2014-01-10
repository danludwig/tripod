'use strict';
define(["require", "exports"], function(require, exports) {
    exports.directiveName = 'serverValidate';

    var ServerValidateController = (function () {
        //modelController: ng.INgModelController;
        //helpController: dModelHelper.ModelHelperController;
        function ServerValidateController() {
            this.attempts = [];
        }
        ServerValidateController.prototype.lastAttempt = function () {
            return this.attempts[this.attempts.length - 1];
        };
        return ServerValidateController;
    })();
    exports.ServerValidateController = ServerValidateController;

    //#region Directive
    function failUnexpectedly(modelCtrl, helpCtrl) {
        helpCtrl.serverError = 'An unexpected validation error has occurred.';
        modelCtrl.$setValidity('server', false);
    }

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
                            var attempt = { viewValue: value };
                            validateCtrl.attempts.push(attempt);

                            // set server validity to true when model is pristine or equal to its initial value..?
                            if (modelCtrl.$pristine || modelCtrl.$viewValue == initialValue) {
                                helpCtrl.serverError = null;
                                modelCtrl.$setValidity('server', true);
                                attempt.result = {
                                    isValid: true,
                                    attemptedValue: value,
                                    attemptedString: value,
                                    errors: []
                                };
                                helpCtrl.serverValidating = false;
                                return;
                            }

                            // tell the controller there is validation progress
                            // this needs throttled for cases when the server returns very quickly
                            var spinnerTimeoutPromise = $timeout(function () {
                                helpCtrl.serverValidating = true;
                            }, 20);

                            helpCtrl.serverError = null;
                            modelCtrl.$setValidity('server', true);
                            var url = attr[exports.directiveName];
                            $http.post(url, { userName: value }, {}).success(function (data) {
                                // if this is not the last attempt, skip silently
                                if (validateCtrl.lastAttempt() !== attempt)
                                    return;

                                $timeout.cancel(spinnerTimeoutPromise);
                                helpCtrl.serverValidating = false;

                                // expect the result to have a property with the same name as the validated field
                                var fieldName = attr['name'];
                                if (!fieldName || !data[fieldName])
                                    failUnexpectedly(modelCtrl, helpCtrl);

                                var result = data[fieldName];
                                attempt.result = result;
                                if (result.isValid) {
                                    helpCtrl.serverError = null;
                                    modelCtrl.$setValidity('server', true);
                                } else {
                                    helpCtrl.serverError = result.errors[0].message;
                                    modelCtrl.$setValidity('server', false);
                                }
                            }).error(function (data, status) {
                                $timeout.cancel(spinnerTimeoutPromise);
                                helpCtrl.serverValidating = false;

                                // when status is zero, user probably refreshed before this returned
                                if (status === 0)
                                    return;

                                // otherwise, something went wrong that we weren't expecting
                                failUnexpectedly(modelCtrl, helpCtrl);
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
