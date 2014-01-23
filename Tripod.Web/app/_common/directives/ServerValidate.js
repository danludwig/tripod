'use strict';
var App;
(function (App) {
    (function (Directives) {
        (function (ServerValidate) {
            ServerValidate.directiveName = 'serverValidate';

            var directiveFactory = function () {
                return [
                    '$http', '$timeout', '$interval', '$parse', function ($http, $timeout, $interval, $parse) {
                        var d = {
                            name: ServerValidate.directiveName,
                            restrict: 'A',
                            require: ['modelContrib', 'ngModel', '^formContrib', '^form'],
                            link: function (scope, element, attr, ctrls) {
                                var modelContribCtrl = ctrls[0];
                                var modelCtrl = ctrls[1];
                                var formContribCtrl = ctrls[2];
                                var formCtrl = ctrls[3];

                                var validateUrl = attr[ServerValidate.directiveName];
                                var validateThrottleAttr = attr[ServerValidate.directiveName + 'Throttle'];
                                var validateDataAttr = attr[ServerValidate.directiveName + 'Data'];
                                var validateCacheAttr = attr[ServerValidate.directiveName + 'Cache'];
                                var fieldName = attr['name'];

                                var throttle = isNaN(parseInt(validateThrottleAttr)) ? 0 : parseInt(validateThrottleAttr);
                                var validateCache = angular.lowercase(validateCacheAttr) === 'false' ? false : true;
                                var throttlePromise;
                                var lastAttempt;
                                var initialValue;

                                var unexpectedError = 'An unexpected validation error has occurred.';
                                var configurationError = 'This field\'s remote validation is not properly configured.';
                                var attempts = [];

                                var hasOtherError = function () {
                                    for (var validationErrorKey in modelCtrl.$error) {
                                        if (validationErrorKey != 'server' && modelCtrl.$error.hasOwnProperty(validationErrorKey) && modelCtrl.$error[validationErrorKey] === true)
                                            return true;
                                    }
                                    return false;
                                };

                                var isInitialValue = function (value) {
                                    return initialValue === value;
                                };

                                var buildPostData = function (value) {
                                    var postData = null;

                                    if (!validateDataAttr && !fieldName) {
                                        return null;
                                    }

                                    if (validateDataAttr) {
                                        postData = $parse(validateDataAttr)(scope);
                                    }

                                    if (fieldName && (!validateDataAttr || validateDataAttr.indexOf(fieldName + ':') < 0)) {
                                        postData = postData || {};
                                        postData[fieldName] = value;
                                    }

                                    return postData;
                                };

                                var getAttempt = function (value) {
                                    if (!attempts.length)
                                        return null;

                                    for (var i = 0; i < attempts.length; i++)
                                        if (attempts[i].value === value)
                                            return attempts[i];
                                    return null;
                                };

                                var setValidity = function (attempt) {
                                    if (attempt && !attempt.result)
                                        return;
                                    if (!attempt || attempt.result.isValid) {
                                        modelContribCtrl.setValidity('server', null);
                                    } else {
                                        var hasMessage = attempt.result && attempt.result.errors && attempt.result.errors.length && attempt.result.errors[0];
                                        var message = hasMessage ? attempt.result.errors[0].message : unexpectedError;
                                        modelContribCtrl.setValidity('server', message);
                                    }
                                };

                                var form = element.parents('form');
                                var foundAttempt;
                                var formInterval;
                                form.bind('submit', function (e) {
                                    if (formInterval)
                                        $interval.cancel(formInterval);

                                    if (isInitialValue(modelCtrl.$viewValue) && attr['serverError']) {
                                        e.preventDefault();
                                        return false;
                                    }

                                    if (hasOtherError()) {
                                        modelContribCtrl.setValidity('server', null);
                                        return true;
                                    }

                                    formContribCtrl.isSubmitWaiting = true;

                                    foundAttempt = getAttempt(modelCtrl.$viewValue);
                                    if (foundAttempt && foundAttempt.result) {
                                        setValidity(foundAttempt);
                                        if (!scope.$$phase)
                                            scope.$apply();
                                        if (!foundAttempt.result.isValid) {
                                            e.preventDefault();
                                        }
                                        if (formCtrl.$invalid)
                                            formContribCtrl.isSubmitWaiting = false;
                                        return foundAttempt.result.isValid;
                                    }
                                    ;

                                    modelContribCtrl.hasSpinner = true;
                                    formInterval = $interval(function () {
                                        foundAttempt = getAttempt(modelCtrl.$viewValue);
                                        if (foundAttempt && foundAttempt.result) {
                                            $interval.cancel(formInterval);
                                            formInterval = null;
                                            form.submit();
                                        }
                                    }, 10);

                                    e.preventDefault();
                                    return false;
                                });

                                scope.$watch(function () {
                                    if (angular.isUndefined(initialValue))
                                        initialValue = modelCtrl.$viewValue;
                                    return modelCtrl.$viewValue;
                                }, function (value) {
                                    if (throttlePromise)
                                        $timeout.cancel(throttlePromise);

                                    if (hasOtherError()) {
                                        modelContribCtrl.setValidity('server', null);
                                        return;
                                    }

                                    if (isInitialValue(value) && attr['serverError'])
                                        return;

                                    var attempt = getAttempt(value);
                                    if (attempt && attempt.result && validateCache) {
                                        lastAttempt = attempt;
                                        setValidity(attempt);
                                        return;
                                    }

                                    if (!isInitialValue(value))
                                        modelContribCtrl.hasSpinner = true;

                                    throttlePromise = $timeout(function () {
                                        var postData = buildPostData(value);
                                        if (!validateUrl || !postData) {
                                            modelContribCtrl.setValidity('server', configurationError);
                                            return;
                                        }

                                        if (!attempt) {
                                            attempt = { value: value };
                                            attempts.push(attempt);
                                        }
                                        lastAttempt = attempt;

                                        $http.post(validateUrl, postData).success(function (response) {
                                            if (!response || !response[fieldName]) {
                                                modelContribCtrl.setValidity('server', unexpectedError);
                                                return;
                                            }

                                            attempt.result = response[fieldName];

                                            if (attempt !== lastAttempt || modelCtrl.$pristine) {
                                                return;
                                            }

                                            setValidity(attempt);
                                        }).error(function (response, status) {
                                            if (status === 0)
                                                return;

                                            modelContribCtrl.setValidity('server', unexpectedError);
                                            attempt.result = {
                                                isValid: false,
                                                errors: [{ message: unexpectedError }]
                                            };
                                        });

                                        throttlePromise = null;
                                    }, throttle);
                                });
                            }
                        };
                        return d;
                    }];
            };

            ServerValidate.directive = directiveFactory();
        })(Directives.ServerValidate || (Directives.ServerValidate = {}));
        var ServerValidate = Directives.ServerValidate;
    })(App.Directives || (App.Directives = {}));
    var Directives = App.Directives;
})(App || (App = {}));
