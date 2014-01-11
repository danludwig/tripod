'use strict';
define(["require", "exports"], function(require, exports) {
    exports.directiveName = 'serverValidate';

    var ServerValidateController = (function () {
        function ServerValidateController() {
            this.attempts = [];
        }
        ServerValidateController.prototype.isFirstAttempt = function () {
            return !this.attempts.length;
        };

        ServerValidateController.prototype.getAttempt = function (value) {
            if (!this.attempts.length)
                return null;

            for (var i = 0; i < this.attempts.length; i++)
                if (this.attempts[i].value === value)
                    return this.attempts[i];
            return null;
        };

        ServerValidateController.prototype.getLastAttempt = function () {
            return this.attempts[this.attempts.length - 1];
        };

        ServerValidateController.prototype.setValidity = function (attempt) {
            this.modelController.$setValidity('server', attempt.result.isValid);
            if (attempt.result.isValid) {
                this.helpController.serverError = null;
            } else {
                this.helpController.serverError = attempt.result.errors[0].message;
            }
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
                        validateCtrl.helpController = helpCtrl;
                        validateCtrl.modelController = modelCtrl;

                        scope.$watch(function () {
                            return modelCtrl.$viewValue;
                        }, function (value) {
                            // the first time this watch executes, record the value as the first attempt
                            // and stop, to prevent the server from validating when first loaded
                            if (validateCtrl.isFirstAttempt()) {
                                validateCtrl.attempts.push({
                                    value: value,
                                    result: {
                                        isValid: true,
                                        errors: [{ message: null }]
                                    }
                                });
                                return;
                            }

                            // check to see if this value has already been attempted
                            // and if it has, skip hitting the server
                            var attempt = validateCtrl.getAttempt(value);
                            if (attempt && attempt.result) {
                                validateCtrl.setValidity(attempt);
                                return;
                            }

                            // record all value attempts in this directive's controller
                            attempt = { value: value };
                            validateCtrl.attempts.push(attempt);

                            // tell the controller there is validation progress
                            // this should be throttled for cases when the server returns very quickly
                            var spinnerTimeoutPromise = $timeout(function () {
                                helpCtrl.isServerValidating = true;
                            }, 20);

                            // set validity to true while we are validating
                            validateCtrl.setValidity({ result: { isValid: true } });

                            var url = attr[exports.directiveName];
                            $http.post(url, { userName: value }).success(function (data) {
                                // expect the result to have a property with the same name as the validated field
                                var fieldName = attr['name'];
                                if (!fieldName || !data[fieldName])
                                    failUnexpectedly(modelCtrl, helpCtrl);

                                // load the result from the response data and store with attempt
                                var result = data[fieldName];
                                attempt.result = result;
                                $timeout.cancel(spinnerTimeoutPromise);

                                // if this is not the last attempt, just record the result and skip
                                if (validateCtrl.getLastAttempt() !== attempt) {
                                    return;
                                }

                                helpCtrl.isServerValidating = false;
                                validateCtrl.setValidity(attempt);
                            }).error(function (data, status) {
                                $timeout.cancel(spinnerTimeoutPromise);
                                helpCtrl.isServerValidating = false;

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
