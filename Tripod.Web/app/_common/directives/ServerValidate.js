'use strict';
var App;
(function (App) {
    (function (Directives) {
        (function (ServerValidate) {
            ServerValidate.directiveName = 'serverValidate';

            var ServerValidateController = (function () {
                function ServerValidateController() {
                    this.attempts = [];
                }
                ServerValidateController.prototype.getAttempt = function (value) {
                    if (!this.attempts.length)
                        return null;

                    for (var i = 0; i < this.attempts.length; i++)
                        if (this.attempts[i].value === value)
                            return this.attempts[i];
                    return null;
                };

                ServerValidateController.prototype.setError = function (validateAttempt) {
                    if (!validateAttempt) {
                        this.helpController.setValidity('server', null);
                    } else {
                        var hasMessage = validateAttempt.result && validateAttempt.result.errors && validateAttempt.result.errors.length && validateAttempt.result.errors[0];
                        var message = hasMessage ? validateAttempt.result.errors[0].message : ServerValidateController.unexpectedError;

                        this.helpController.setValidity('server', message);
                    }
                };

                ServerValidateController.prototype.setUnexpectedError = function () {
                    this.setError({
                        result: {
                            errors: [{
                                    message: ServerValidateController.unexpectedError
                                }]
                        }
                    });
                };
                ServerValidateController.unexpectedError = 'An unexpected validation error has occurred.';
                return ServerValidateController;
            })();
            ServerValidate.ServerValidateController = ServerValidateController;

            var directiveFactory = function () {
                return [
                    '$http', '$timeout', '$interval', '$parse', function ($http, $timeout, $interval, $parse) {
                        var directive = {
                            name: ServerValidate.directiveName,
                            restrict: 'A',
                            require: [ServerValidate.directiveName, 'modelContrib', 'ngModel', '^formContrib', '^form'],
                            controller: ServerValidateController,
                            link: function (scope, element, attr, ctrls) {
                                var validateCtrl = ctrls[0];
                                var modelHelpCtrl = ctrls[1];
                                var modelCtrl = ctrls[2];
                                var formHelpCtrl = ctrls[3];
                                var formCtrl = ctrls[4];
                                validateCtrl.helpController = modelHelpCtrl;
                                validateCtrl.modelController = modelCtrl;

                                var validateUrl = attr[ServerValidate.directiveName];
                                var validateThrottleAttr = attr['serverValidateThrottle'];
                                var validateNoSuccessAttr = attr['serverValidateNoSuccess'];
                                var throttle = isNaN(parseInt(validateThrottleAttr)) ? 0 : parseInt(validateThrottleAttr);
                                var validateDataAttr = attr['serverValidateData'];

                                var throttlePromise, lastAttempt;

                                var form = element.parents('form'), foundAttempt, formInterval;
                                form.bind('submit', function (e) {
                                    if (formInterval)
                                        $interval.cancel(formInterval);

                                    formHelpCtrl.isSubmitWaiting = true;

                                    foundAttempt = validateCtrl.getAttempt(modelCtrl.$viewValue);
                                    if (foundAttempt && foundAttempt.result) {
                                        validateCtrl.setError(foundAttempt.result.isValid ? null : foundAttempt);
                                        if (!scope.$$phase)
                                            scope.$apply();
                                        if (!foundAttempt.result.isValid) {
                                            e.preventDefault();
                                        }
                                        if (formCtrl.$invalid)
                                            formHelpCtrl.isSubmitWaiting = false;
                                        return foundAttempt.result.isValid;
                                    }
                                    ;

                                    modelHelpCtrl.hasSpinner = true;
                                    formInterval = $interval(function () {
                                        foundAttempt = validateCtrl.getAttempt(modelCtrl.$viewValue);
                                        if (foundAttempt && foundAttempt.result) {
                                            $interval.cancel(formInterval);
                                            modelHelpCtrl.hasSpinner = false;
                                            form.submit();
                                        }
                                    }, 10);

                                    e.preventDefault();
                                    return false;
                                });

                                scope.$watch(function () {
                                    return modelCtrl.$viewValue;
                                }, function (value) {
                                    if (throttlePromise)
                                        $timeout.cancel(throttlePromise);

                                    var attempt = validateCtrl.getAttempt(value);
                                    if (attempt && attempt.result) {
                                        lastAttempt = attempt;
                                        modelHelpCtrl.hasSpinner = false;

                                        if (attempt.result.isValid || attempt == validateCtrl.attempts[0]) {
                                            validateCtrl.setError(null);
                                        } else {
                                            validateCtrl.setError(attempt);
                                        }
                                        return;
                                    }

                                    var fieldName = attr['name'], postData;

                                    if (!validateDataAttr && !fieldName) {
                                        validateCtrl.setUnexpectedError();
                                        return;
                                    }

                                    if (validateDataAttr) {
                                        postData = $parse(validateDataAttr)(scope);
                                    }

                                    if (fieldName && (!validateDataAttr || validateDataAttr.indexOf(fieldName + ':') < 0)) {
                                        postData = postData || {};
                                        postData[fieldName] = value;
                                    }

                                    throttlePromise = $timeout(function () {
                                        if (!attempt) {
                                            attempt = { value: value };
                                            validateCtrl.attempts.push(attempt);
                                        }
                                        lastAttempt = attempt;

                                        var spinnerTimeoutPromise = $timeout(function () {
                                            if (attempt != validateCtrl.attempts[0]) {
                                                modelHelpCtrl.hasSpinner = true;
                                            }
                                        }, 20);

                                        $http.post(validateUrl, postData).success(function (response) {
                                            $timeout.cancel(spinnerTimeoutPromise);

                                            if (!response || !response[fieldName]) {
                                                validateCtrl.setUnexpectedError();
                                                return;
                                            }

                                            attempt.result = response[fieldName];

                                            if (attempt !== lastAttempt || attempt === validateCtrl.attempts[0]) {
                                                return;
                                            }

                                            modelHelpCtrl.hasSpinner = false;
                                            if (attempt.result.isValid) {
                                                validateCtrl.setError(null);
                                            } else {
                                                validateCtrl.setError(attempt);
                                            }
                                        }).error(function (data, status) {
                                            $timeout.cancel(spinnerTimeoutPromise);
                                            modelHelpCtrl.hasSpinner = false;

                                            if (status === 0)
                                                return;

                                            validateCtrl.setUnexpectedError();
                                        });
                                    }, throttle);
                                });
                            }
                        };
                        return directive;
                    }];
            };

            ServerValidate.directive = directiveFactory();
        })(Directives.ServerValidate || (Directives.ServerValidate = {}));
        var ServerValidate = Directives.ServerValidate;
    })(App.Directives || (App.Directives = {}));
    var Directives = App.Directives;
})(App || (App = {}));
