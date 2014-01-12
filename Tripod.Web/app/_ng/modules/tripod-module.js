'use strict';
var App;
(function (App) {
    (function (Directives) {
        (function (SubmitAction) {
            SubmitAction.directiveName = 'ngT3SubmitAction';

            var Controller = (function () {
                function Controller() {
                    this.attempted = false;
                }
                Controller.prototype.needsAttention = function (fieldModelController) {
                    if (!this.formController)
                        return false;

                    if (fieldModelController)
                        return fieldModelController.$invalid && (fieldModelController.$dirty || this.attempted);

                    return this.formController && this.formController.$invalid && (this.formController.$dirty || this.attempted);
                };

                Controller.prototype.isGoodToGo = function (fieldModelController) {
                    if (!this.formController)
                        return false;
                    if (this.needsAttention(fieldModelController))
                        return false;

                    if (fieldModelController)
                        return fieldModelController.$valid && (fieldModelController.$dirty || this.attempted);

                    return this.formController && this.formController.$valid && (this.formController.$dirty || this.attempted);
                };
                return Controller;
            })();

            var directiveFactory = function () {
                return [
                    '$parse', function ($parse) {
                        var directive = {
                            name: SubmitAction.directiveName,
                            restrict: 'A',
                            require: ['ngT3SubmitAction', '?form'],
                            controller: Controller,
                            compile: function () {
                                return {
                                    pre: function (scope, formElement, attributes, controllers) {
                                        var submitController = controllers[0];

                                        var formController = (controllers.length > 1) ? controllers[1] : null;
                                        submitController.formController = formController;

                                        scope['t3'] = scope['t3'] || {};
                                        scope['t3'][attributes['name']] = submitController;
                                    },
                                    post: function (scope, formElement, attributes, controllers) {
                                        var submitController = controllers[0];
                                        var formController = (controllers.length > 1) ? controllers[1] : null;

                                        var fn = $parse(attributes[SubmitAction.directiveName]);

                                        formElement.bind('submit', function () {
                                            submitController.attempted = true;
                                            if (!scope.$$phase)
                                                scope.$apply();

                                            if (!formController.$valid)
                                                return false;

                                            scope.$apply(function () {
                                                fn(scope, { $event: event });
                                            });
                                            return true;
                                        });

                                        if (attributes['ngT3SubmitActionAttempted'])
                                            submitController.attempted = true;
                                    }
                                };
                            }
                        };
                        return directive;
                    }];
            };

            SubmitAction.directive = directiveFactory();
        })(Directives.SubmitAction || (Directives.SubmitAction = {}));
        var SubmitAction = Directives.SubmitAction;
    })(App.Directives || (App.Directives = {}));
    var Directives = App.Directives;
})(App || (App = {}));

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
                        this.helpController.isNoSuccess = false;
                        this.helpController.serverError = null;
                        this.modelController.$setValidity('server', true);
                    } else {
                        var hasMessage = validateAttempt.result && validateAttempt.result.errors && validateAttempt.result.errors.length && validateAttempt.result.errors[0];
                        var message = hasMessage ? validateAttempt.result.errors[0].message : ServerValidateController.unexpectedError;
                        this.helpController.serverError = message;
                        this.modelController.$setValidity('server', false);
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
                            require: [ServerValidate.directiveName, 'modelHelper', 'ngModel', '^formHelper', '^form'],
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

                                    formHelpCtrl.isSubmitDisabled = true;

                                    foundAttempt = validateCtrl.getAttempt(modelCtrl.$viewValue);
                                    if (foundAttempt && foundAttempt.result) {
                                        validateCtrl.setError(foundAttempt.result.isValid ? null : foundAttempt);
                                        if (!scope.$$phase)
                                            scope.$apply();
                                        if (!foundAttempt.result.isValid) {
                                            e.preventDefault();
                                        }
                                        if (formCtrl.$invalid)
                                            formHelpCtrl.isSubmitDisabled = false;
                                        return foundAttempt.result.isValid;
                                    }
                                    ;

                                    modelHelpCtrl.isServerValidating = true;
                                    formInterval = $interval(function () {
                                        foundAttempt = validateCtrl.getAttempt(modelCtrl.$viewValue);
                                        if (foundAttempt && foundAttempt.result) {
                                            $interval.cancel(formInterval);
                                            modelHelpCtrl.isServerValidating = false;
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
                                        modelHelpCtrl.isServerValidating = false;

                                        if (attempt.result.isValid || attempt == validateCtrl.attempts[0]) {
                                            validateCtrl.setError(null);
                                            if (attempt == validateCtrl.attempts[0] && !attempt.result.isValid && validateNoSuccessAttr) {
                                                modelHelpCtrl.isNoSuccess = true;
                                            }
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

                                    var previousServerMessage = modelHelpCtrl.serverError;
                                    modelCtrl.$setValidity('server', true);
                                    modelHelpCtrl.serverError = null;
                                    if (!modelCtrl.$valid) {
                                        if (!validateCtrl.attempts.length) {
                                            validateCtrl.attempts.push({
                                                value: value,
                                                result: {
                                                    isValid: true,
                                                    errors: []
                                                }
                                            });
                                        }
                                        return;
                                    } else {
                                        modelCtrl.$setValidity('server', previousServerMessage ? false : true);
                                        modelHelpCtrl.serverError = previousServerMessage;
                                    }

                                    if (validateNoSuccessAttr)
                                        modelHelpCtrl.isNoSuccess = true;

                                    throttlePromise = $timeout(function () {
                                        if (!attempt) {
                                            attempt = { value: value };
                                            validateCtrl.attempts.push(attempt);
                                        }
                                        lastAttempt = attempt;

                                        var spinnerTimeoutPromise = $timeout(function () {
                                            if (attempt != validateCtrl.attempts[0]) {
                                                modelHelpCtrl.isServerValidating = true;
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

                                            modelHelpCtrl.isServerValidating = false;
                                            if (attempt.result.isValid) {
                                                validateCtrl.setError(null);
                                            } else {
                                                validateCtrl.setError(attempt);
                                            }
                                        }).error(function (data, status) {
                                            $timeout.cancel(spinnerTimeoutPromise);
                                            modelHelpCtrl.isServerValidating = false;

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
                        require: ['ngModel', 'modelHelper'],
                        link: function (scope, element, attr, ctrls) {
                            var serverError = attr['serverError'];
                            if (!serverError)
                                return;

                            var inputType = attr['type'];
                            var isPassword = inputType && inputType.toLowerCase() == 'password';

                            var modelCtrl = ctrls[0];
                            var helpCtrl = ctrls[1];

                            helpCtrl.serverError = serverError;

                            var initialValue;
                            var initWatch = scope.$watch(function () {
                                return modelCtrl.$error;
                            }, function (value) {
                                initialValue = modelCtrl.$viewValue;
                                if (value.required && isPassword) {
                                    modelCtrl.$setValidity('required', true);
                                }
                                modelCtrl.$setValidity('server', false);
                                initWatch();
                            });

                            scope.$watch(function () {
                                return modelCtrl.$viewValue;
                            }, function (value) {
                                if (modelCtrl.$dirty) {
                                    modelCtrl.$setValidity('server', true);
                                }

                                if (!isPassword && value === initialValue) {
                                    modelCtrl.$setValidity('server', false);
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

'use strict';
var App;
(function (App) {
    (function (Directives) {
        (function (RemoveCssClass) {
            RemoveCssClass.directiveName = 'removeClass';

            var directiveFactory = function () {
                return function () {
                    var directive = {
                        restrict: 'A',
                        link: function (scope, element, attrs) {
                            element.removeClass(attrs[RemoveCssClass.directiveName]);
                        }
                    };
                    return directive;
                };
            };

            RemoveCssClass.directive = directiveFactory();
        })(Directives.RemoveCssClass || (Directives.RemoveCssClass = {}));
        var RemoveCssClass = Directives.RemoveCssClass;
    })(App.Directives || (App.Directives = {}));
    var Directives = App.Directives;
})(App || (App = {}));

'use strict';
var App;
(function (App) {
    (function (Directives) {
        (function (Popover) {
            Popover.directiveName = 't3Popover';

            var directiveFactory = function () {
                return [
                    '$parse', '$timeout', function ($parse, $timeout) {
                        var directive = {
                            name: Popover.directiveName,
                            restrict: 'A',
                            link: function (scope, element, attrs) {
                                var options = {
                                    content: $parse(attrs[Popover.directiveName])(scope),
                                    trigger: 'manual',
                                    animation: typeof attrs['t3PopoverAnimation'] === 'string' ? attrs['t3PopoverAnimation'].toLowerCase() !== 'false' : true
                                };

                                var initPopup = function () {
                                    var data = element.data(Popover.directiveName);
                                    if (data) {
                                        element.popover('destroy');
                                    }
                                    element.popover(options);
                                    element.data(Popover.directiveName, true);
                                };

                                initPopup();

                                var isVisible = false;
                                scope.$watch(attrs[Popover.directiveName], function (value) {
                                    if (value != options.content) {
                                        options.content = value;
                                        initPopup();
                                        if (isVisible)
                                            element.popover('show');
                                    }
                                });

                                var redrawPromise;
                                $(window).on('resize', function () {
                                    if (redrawPromise)
                                        $timeout.cancel(redrawPromise);
                                    redrawPromise = $timeout(function () {
                                        if (!isVisible)
                                            return;
                                        element.popover('hide');
                                        element.popover('show');
                                    }, 100);
                                });

                                scope.$watch(attrs['t3PopoverSwitch'], function (value) {
                                    if (value) {
                                        isVisible = true;
                                        element.popover('show');
                                    } else {
                                        isVisible = false;
                                        element.popover('hide');
                                    }
                                });
                            }
                        };
                        return directive;
                    }];
            };

            Popover.directive = directiveFactory();
        })(Directives.Popover || (Directives.Popover = {}));
        var Popover = Directives.Popover;
    })(App.Directives || (App.Directives = {}));
    var Directives = App.Directives;
})(App || (App = {}));

'use strict';
var App;
(function (App) {
    (function (Directives) {
        (function (ModelHelper) {
            ModelHelper.directiveName = 'modelHelper';

            var Controller = (function () {
                function Controller(scope) {
                    this.isServerValidating = false;
                    this.isNoSuccess = false;
                }
                Controller.prototype.hasError = function () {
                    return !this.isServerValidating && this.modelController.$invalid && (this.modelController.$dirty || this.formController.submitAttempted);
                };

                Controller.prototype.hasSuccess = function () {
                    return !this.isNoSuccess && !this.isServerValidating && !this.hasError() && this.modelController.$valid && (this.modelController.$dirty || this.formController.submitAttempted);
                };

                Controller.prototype.hasFeedback = function () {
                    return this.hasError() || this.hasSuccess() || this.hasSpinner();
                };

                Controller.prototype.hasSpinner = function () {
                    return this.isServerValidating;
                };
                Controller.$inject = ['$scope'];
                return Controller;
            })();
            ModelHelper.Controller = Controller;

            var directiveFactory = function () {
                return function () {
                    var directive = {
                        name: ModelHelper.directiveName,
                        restrict: 'A',
                        require: [ModelHelper.directiveName, 'ngModel', '^formHelper'],
                        controller: Controller,
                        link: function (scope, element, attr, ctrls) {
                            var helpCtrl = ctrls[0];
                            var modelCtrl = ctrls[1];
                            var formCtrl = ctrls[2];

                            helpCtrl.modelController = modelCtrl;
                            helpCtrl.formController = formCtrl;

                            var alias = $.trim(attr['name']);
                            if (alias)
                                formCtrl[alias] = helpCtrl;
                        }
                    };
                    return directive;
                };
            };

            ModelHelper.directive = directiveFactory();
        })(Directives.ModelHelper || (Directives.ModelHelper = {}));
        var ModelHelper = Directives.ModelHelper;
    })(App.Directives || (App.Directives = {}));
    var Directives = App.Directives;
})(App || (App = {}));

'use strict';
var App;
(function (App) {
    (function (Directives) {
        (function (InputPreFormatter) {
            InputPreFormatter.directiveName = 'input';

            var directiveFactory = function () {
                return function () {
                    var directive = {
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
                    return directive;
                };
            };

            InputPreFormatter.directive = directiveFactory();
        })(Directives.InputPreFormatter || (Directives.InputPreFormatter = {}));
        var InputPreFormatter = Directives.InputPreFormatter;
    })(App.Directives || (App.Directives = {}));
    var Directives = App.Directives;
})(App || (App = {}));

'use strict';
var App;
(function (App) {
    (function (Directives) {
        (function (FormHelper) {
            FormHelper.directiveName = 'formHelper';

            var Controller = (function () {
                function Controller() {
                    this.submitAttempted = false;
                    this.isSubmitDisabled = false;
                }
                return Controller;
            })();
            FormHelper.Controller = Controller;

            var directiveFactory = function () {
                return [
                    '$parse', function ($parse) {
                        var directive = {
                            name: FormHelper.directiveName,
                            restrict: 'A',
                            require: [FormHelper.directiveName, 'form'],
                            controller: Controller,
                            compile: function () {
                                return {
                                    pre: function (scope, element, attr, ctrls) {
                                        var helpCtrl = ctrls[0];
                                        var formCtrl = ctrls[1];

                                        helpCtrl.formController = formCtrl;

                                        if (attr['submitAttempted'])
                                            helpCtrl.submitAttempted = true;

                                        var alias = $.trim(attr[FormHelper.directiveName]);
                                        if (alias)
                                            scope[alias] = helpCtrl;
                                    },
                                    post: function (scope, element, attr, ctrls) {
                                        var helpCtrl = ctrls[0];
                                        var formCtrl = ctrls[1];

                                        element.bind('submit', function () {
                                            helpCtrl.submitAttempted = true;
                                            if (formCtrl.$valid)
                                                helpCtrl.isSubmitDisabled = true;
                                            if (!scope.$$phase)
                                                scope.$apply();

                                            if (!formCtrl.$valid)
                                                return false;

                                            return true;
                                        });
                                    }
                                };
                            }
                        };
                        return directive;
                    }];
            };

            FormHelper.directive = directiveFactory();
        })(Directives.FormHelper || (Directives.FormHelper = {}));
        var FormHelper = Directives.FormHelper;
    })(App.Directives || (App.Directives = {}));
    var Directives = App.Directives;
})(App || (App = {}));

'use strict';
var App;
(function (App) {
    (function (Modules) {
        (function (Tripod) {
            Tripod.moduleName = 'tripod';

            Tripod.ngModule = angular.module(Tripod.moduleName, []).directive(App.Directives.InputPreFormatter.directiveName, App.Directives.InputPreFormatter.directive).directive(App.Directives.RemoveCssClass.directiveName, App.Directives.RemoveCssClass.directive).directive(App.Directives.Popover.directiveName, App.Directives.Popover.directive).directive(App.Directives.FormHelper.directiveName, App.Directives.FormHelper.directive).directive(App.Directives.ModelHelper.directiveName, App.Directives.ModelHelper.directive).directive(App.Directives.ServerError.directiveName, App.Directives.ServerError.directive).directive(App.Directives.ServerValidate.directiveName, App.Directives.ServerValidate.directive).directive(App.Directives.SubmitAction.directiveName, App.Directives.SubmitAction.directive);
        })(Modules.Tripod || (Modules.Tripod = {}));
        var Tripod = Modules.Tripod;
    })(App.Modules || (App.Modules = {}));
    var Modules = App.Modules;
})(App || (App = {}));

