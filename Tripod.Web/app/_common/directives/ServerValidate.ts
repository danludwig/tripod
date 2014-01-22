'use strict';

module App.Directives.ServerValidate {

    export var directiveName = 'serverValidate';

    export interface ServerValidateAttempt {
        value?: any;
        result?: ValidatedField;
    }

    //#region Directive

    var directiveFactory = (): any[] => {
        // inject services
        return ['$http', '$timeout', '$interval', '$parse', ($http: ng.IHttpService, $timeout: ng.ITimeoutService, $interval: ng.IIntervalService, $parse: ng.IParseService): ng.IDirective => {
            var d: ng.IDirective = {
                name: directiveName,
                restrict: 'A', // attribute only
                require: ['modelContrib', 'ngModel', '^formContrib', '^form'],
                link: (scope: ng.IScope, element: JQuery, attr: ng.IAttributes, ctrls: any[]): void => {

                    //var validateCtrl: Controller = ctrls[0];
                    var modelContribCtrl: ModelContrib.Controller = ctrls[0];
                    var modelCtrl: ng.INgModelController = ctrls[1];
                    var formContribCtrl: FormContrib.Controller = ctrls[2];
                    var formCtrl: ng.IFormController = ctrls[3];

                    // get configuration attributes
                    var validateUrl = attr[directiveName];
                    var validateThrottleAttr: string = attr[directiveName + 'Throttle'];
                    var validateDataAttr: string = attr[directiveName + 'Data'];
                    var fieldName = attr['name'];

                    // set up variables
                    var throttle = isNaN(parseInt(validateThrottleAttr)) ? 0 : parseInt(validateThrottleAttr);
                    var throttlePromise: ng.IPromise<void>;
                    var lastAttempt: ServerValidateAttempt;
                    var initialValue: string;

                    var unexpectedError = 'An unexpected validation error has occurred.';
                    var configurationError = 'This field\'s remote validation is not properly configured.';
                    var attempts: ServerValidateAttempt[] = [];

                    //#region Helper Functions

                    var hasOtherError = (): boolean => {
                        for (var validationErrorKey in modelCtrl.$error) {
                            if (validationErrorKey != 'server' &&
                                modelCtrl.$error.hasOwnProperty(validationErrorKey) &&
                                modelCtrl.$error[validationErrorKey] === true)
                                return true;
                        }
                        return false;
                    };

                    var isInitialValue = (value: string): boolean => {
                        return initialValue === value;
                    };

                    var buildPostData = (value: string): any => {
                        var postData: any = null;

                        // when the field has no name and no data attribute, there is nothing to build
                        if (!validateDataAttr && !fieldName) {
                            return null;
                        }

                        // initialize the data with what is in the data attribute
                        if (validateDataAttr) {
                            postData = $parse(validateDataAttr)(scope);
                        }

                        // overlay the field name when it exists and not in data attribute
                        if (fieldName && (!validateDataAttr || validateDataAttr.indexOf(fieldName + ':') < 0)) {
                            postData = postData || {};
                            postData[fieldName] = value;
                        }

                        return postData;
                    };

                    var getAttempt = (value: string): ServerValidateAttempt => {
                        if (!attempts.length) return null;

                        for (var i = 0; i < attempts.length; i++)
                            if (attempts[i].value === value)
                                return attempts[i];
                        return null;
                    };

                    var setValidity = (attempt: ServerValidateAttempt): void => {
                        if (attempt && !attempt.result) return;
                        if (!attempt || attempt.result.isValid) {
                            modelContribCtrl.setValidity('server', null);
                        }
                        else {
                            var hasMessage = attempt.result && attempt.result.errors && attempt.result.errors.length && attempt.result.errors[0];
                            var message = hasMessage ? attempt.result.errors[0].message : unexpectedError;
                            modelContribCtrl.setValidity('server', message);
                        }
                    };

                    //#endregion
                    //#region Form Submission

                    var form = element.parents('form');
                    var foundAttempt: ServerValidateAttempt;
                    var formInterval: ng.IPromise<void>;
                    form.bind('submit', (e: JQueryEventObject): boolean => {

                        if (formInterval) $interval.cancel(formInterval);

                        // if the server error directive set an error, yield to it
                        if (isInitialValue(modelCtrl.$viewValue) && attr['serverError']) {
                            e.preventDefault();
                            return false;
                        }

                        // if there is another validation error, yield to it
                        if (hasOtherError()) {
                            modelContribCtrl.setValidity('server', null);
                            return true;
                        }

                        formContribCtrl.isSubmitWaiting = true;

                        // make sure the value is valid
                        foundAttempt = getAttempt(modelCtrl.$viewValue);
                        if (foundAttempt && foundAttempt.result) {
                            setValidity(foundAttempt);
                            if (!scope.$$phase) scope.$apply();
                            if (!foundAttempt.result.isValid) {
                                e.preventDefault();
                            }
                            if (formCtrl.$invalid) // keep submit disabled when the whole form is valid
                                formContribCtrl.isSubmitWaiting = false;
                            return foundAttempt.result.isValid;
                        };

                        modelContribCtrl.hasSpinner = true; // do this in case first attempt is blocking it
                        formInterval = $interval((): void => {
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

                    //#endregion

                    scope.$watch(
                        (): any => {
                            if (angular.isUndefined(initialValue))
                                initialValue = modelCtrl.$viewValue;
                            return modelCtrl.$viewValue;
                        },
                        (value: string): void => {

                            // cancel any previous promises to process
                            if (throttlePromise) $timeout.cancel(throttlePromise);

                            // if there is another validation error, yield to it
                            if (hasOtherError()) {
                                modelContribCtrl.setValidity('server', null);
                                return;
                            }

                            // if the server error directive set an error, yield to it
                            if (isInitialValue(value) && attr['serverError']) return;

                            // if this value has already been attempted and returned a result, skip promise
                            var attempt = getAttempt(value);
                            if (attempt && attempt.result) { // when the attempt has a server result
                                lastAttempt = attempt;
                                setValidity(attempt);
                                return;
                            }

                            // we want to pre-validate the initial value,
                            // but don't want to display the spinner when doing so
                            if (!isInitialValue(value)) modelContribCtrl.hasSpinner = true;

                            throttlePromise = $timeout((): void => {

                                var postData = buildPostData(value);
                                if (!validateUrl || !postData) {
                                    modelContribCtrl.setValidity('server', configurationError);
                                    return;
                                }

                                // this will run the very first time the directive is loaded
                                // many times this will mean that the field is empty, but not always
                                // either way, we don't want to initially display any validation messages the very first time
                                // we can go ahead and pre-validate to stash the result though, for display on submit

                                // record this attempt as we are about to send it to the server
                                if (!attempt) { // if it hasn't, stash the attempt
                                    attempt = { value: value, };
                                    attempts.push(attempt);
                                }
                                lastAttempt = attempt; // store this as the last attempt

                                $http.post(validateUrl, postData)
                                    .success((response: any): void => {

                                        // many different attempts can be initiated and returned in different orders
                                        // furthermore, the user's last attempt may not always be at the end of the array
                                        // user could have accidentlly typed something at the end, then deleted it
                                        // in those cases, the last attempt will not be at the end of the attempts array

                                        // expect the result to have a property with the same name as the validated field
                                        if (!response || !response[fieldName]) {
                                            modelContribCtrl.setValidity('server', unexpectedError);
                                            return;
                                        }

                                        // update the attempt's result
                                        attempt.result = response[fieldName];

                                        // if this is not the last attempt, just record the result and skip
                                        // also do this when model is pristine, avoids swapping model feedback
                                        if (attempt !== lastAttempt || modelCtrl.$pristine) {
                                            return;
                                        }

                                        setValidity(attempt);
                                    })
                                    .error((response: any, status: number): void => {

                                        // when status is zero, user probably refreshed before this returned
                                        if (status === 0) return;

                                        // otherwise, something went wrong that we weren't expecting
                                        modelContribCtrl.setValidity('server', unexpectedError);
                                        attempt.result = {
                                            isValid: false,
                                            errors: [{ message: unexpectedError }],
                                        };
                                    });

                                throttlePromise = null;
                            }, throttle);
                        });
                },
            };
            return d;
        }];

    };

    //#endregion

    export var directive = directiveFactory();

}
