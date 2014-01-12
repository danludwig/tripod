'use strict';

module App.Directives.ServerValidate {

    export var directiveName = 'serverValidate';

    export interface ServerValidateAttempt {
        value?: any;
        result?: ValidatedField;
    }

    export class ServerValidateController {

        static unexpectedError = 'An unexpected validation error has occurred.';
        modelController: ng.INgModelController;
        helpController: ModelHelper.Controller;

        constructor() { }

        attempts: ServerValidateAttempt[] = [];

        getAttempt(value: string): ServerValidateAttempt {
            if (!this.attempts.length) return null;

            for (var i = 0; i < this.attempts.length; i++)
                if (this.attempts[i].value === value)
                    return this.attempts[i];
            return null;
        }

        setError(validateAttempt: ServerValidateAttempt): void {

            if (!validateAttempt) {
                this.helpController.isNoSuccess = false;
                this.helpController.serverError = null;
                this.modelController.$setValidity('server', true);
            }
            else {
                var hasMessage = validateAttempt.result && validateAttempt.result.errors && validateAttempt.result.errors.length && validateAttempt.result.errors[0];
                var message = hasMessage ? validateAttempt.result.errors[0].message : ServerValidateController.unexpectedError;
                this.helpController.serverError = message;
                this.modelController.$setValidity('server', false);
            }
        }

        setUnexpectedError(): void {
            this.setError({
                result: {
                    errors: [{
                        message: ServerValidateController.unexpectedError,
                    }],
                }
            });
        }

    }

    //#region Directive

    var directiveFactory = (): any[]=> {
        // inject services
        return ['$http', '$timeout', '$interval', '$parse', ($http: ng.IHttpService, $timeout: ng.ITimeoutService, $interval: ng.IIntervalService, $parse: ng.IParseService): ng.IDirective => {
            var directive: ng.IDirective = {
                name: directiveName,
                restrict: 'A', // attribute only
                require: [directiveName, 'modelHelper', 'ngModel', '^formHelper', '^form'],
                controller: ServerValidateController,
                link: (scope: ng.IScope, element: JQuery, attr: ng.IAttributes, ctrls: any[]): void => {

                    // unload controllers from array and wire dependencies to validation controller
                    var validateCtrl: ServerValidateController = ctrls[0];
                    var modelHelpCtrl: ModelHelper.Controller = ctrls[1];
                    var modelCtrl: ng.INgModelController = ctrls[2];
                    var formHelpCtrl: FormHelper.Controller = ctrls[3];
                    var formCtrl: ng.IFormController = ctrls[4];
                    validateCtrl.helpController = modelHelpCtrl;
                    validateCtrl.modelController = modelCtrl;

                    // get configuration attributes
                    var validateUrl = attr[directiveName];
                    var validateThrottleAttr: string = attr['serverValidateThrottle'];
                    var validateNoSuccessAttr: string = attr['serverValidateNoSuccess'];
                    var throttle = isNaN(parseInt(validateThrottleAttr)) ? 0 : parseInt(validateThrottleAttr);
                    var validateDataAttr: string = attr['serverValidateData'];

                    var throttlePromise: ng.IPromise<void>, lastAttempt: ServerValidateAttempt;

                    // find parent form to prevent submissions
                    var form = element.parents('form'), foundAttempt: ServerValidateAttempt, formInterval: ng.IPromise<void>;
                    form.bind('submit', (e: JQueryEventObject): boolean => {

                        if (formInterval) $interval.cancel(formInterval);

                        formHelpCtrl.isSubmitDisabled = true;

                        // make sure the value is valid
                        foundAttempt = validateCtrl.getAttempt(modelCtrl.$viewValue);
                        if (foundAttempt && foundAttempt.result) {
                            validateCtrl.setError(foundAttempt.result.isValid ? null : foundAttempt);
                            if (!scope.$$phase) scope.$apply();
                            if (!foundAttempt.result.isValid) {
                                e.preventDefault();
                            }
                            if (formCtrl.$invalid) // keep submit disabled when the whole form is valid
                                formHelpCtrl.isSubmitDisabled = false;
                            return foundAttempt.result.isValid;
                        };

                        modelHelpCtrl.isServerValidating = true; // do this in case first attempt is blocking it
                        formInterval = $interval((): void => {
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

                    scope.$watch(
                        (): any => {
                            return modelCtrl.$viewValue;
                        }, // watch the ngModel $viewValue
                        (value: string): void => {

                            // cancel any previous promises to process because we will process anew now
                            if (throttlePromise) $timeout.cancel(throttlePromise);

                            // if this value has already been attempted and returned a result, skip promise
                            var attempt = validateCtrl.getAttempt(value);
                            if (attempt && attempt.result) { // when the attempt has a server result
                                lastAttempt = attempt;
                                modelHelpCtrl.isServerValidating = false;
                                // the first attempt may have been pre-evaluated, but may not be ready for display
                                if (attempt.result.isValid || attempt == validateCtrl.attempts[0]) { // and the attempt was valid
                                    validateCtrl.setError(null); // clear the server error
                                    if (attempt == validateCtrl.attempts[0] && !attempt.result.isValid && validateNoSuccessAttr) {
                                        modelHelpCtrl.isNoSuccess = true;
                                    }
                                } else { // but when the attempt was not valid
                                    validateCtrl.setError(attempt); // set the server error
                                }
                                return;
                            }

                            // will not be able to do anything unless there is data to send
                            // we handle 3 different cases here
                            // 1.) the element has a name attribute, which is used for the value being sent
                            // 2.) the element has a server-validate-data attribute which can be parsed
                            // 3.) the element has both a name and a server-validate-data attribute
                            var fieldName = attr['name'], postData: any;

                            // when the field has no name and no data attribute, fail with unexpected error
                            if (!validateDataAttr && !fieldName) {
                                validateCtrl.setUnexpectedError();
                                return;
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

                            // if there is any other validator on this field that has an error, yield to it
                            var previousServerMessage = modelHelpCtrl.serverError; // stash any current server error
                            modelCtrl.$setValidity('server', true);
                            modelHelpCtrl.serverError = null;
                            if (!modelCtrl.$valid) {
                                if (!validateCtrl.attempts.length) {
                                    validateCtrl.attempts.push({
                                        value: value,
                                        result: {
                                            isValid: true,
                                            errors: [],
                                        },
                                    });
                                }
                                return; // another validator is telling us the field is invalid, yield to it
                            } else { // restore the stash
                                modelCtrl.$setValidity('server', previousServerMessage ? false : true);
                                modelHelpCtrl.serverError = previousServerMessage;
                            }

                            // tell the help controller that there is no success, don't want to fool/confuse the user
                            // by showing a checkmark if we are going to display the spinner immediately after
                            if (validateNoSuccessAttr)
                                modelHelpCtrl.isNoSuccess = true;

                            throttlePromise = $timeout((): void => {

                                // this will run the very first time the directive is loaded
                                // many times this will mean that the field is empty, but not always
                                // either way, we don't want to initially display any validation messages the very first time
                                // we can go ahead and pre-validate to stash the result though, for display on submit

                                // record this attempt as we are about to send it to the server
                                if (!attempt) { // if it hasn't, stash the attempt
                                    attempt = { value: value, };
                                    validateCtrl.attempts.push(attempt);
                                }
                                lastAttempt = attempt; // store this as the last attempt

                                // tell the controller there is validation progress
                                // this should be throttled for cases when the server returns very quickly
                                var spinnerTimeoutPromise = $timeout((): void => {
                                    // don't want to show a spinner when this runs the first time
                                    if (attempt != validateCtrl.attempts[0]) {
                                        modelHelpCtrl.isServerValidating = true;
                                    }
                                }, 20);

                                $http.post(validateUrl, postData)
                                    .success((response: any): void => {

                                        // many different attempts can be initiated and returned in different orders
                                        // furthermore, the user's last attempt may not always be at the end of the array
                                        // user could have accidentlly typed something at the end, then deleted it
                                        // in those cases, the last attempt will not be at the end of the attempts array

                                        $timeout.cancel(spinnerTimeoutPromise); // skip showing a spinner

                                        // expect the result to have a property with the same name as the validated field
                                        if (!response || !response[fieldName]) {
                                            validateCtrl.setUnexpectedError();
                                            return;
                                        }

                                        // update the attempt's result
                                        attempt.result = response[fieldName];

                                        // if this is not the last attempt, just record the result and skip
                                        if (attempt !== lastAttempt || attempt === validateCtrl.attempts[0]) {
                                            return;
                                        }

                                        modelHelpCtrl.isServerValidating = false;
                                        if (attempt.result.isValid) {
                                            validateCtrl.setError(null);
                                        }
                                        else {
                                            validateCtrl.setError(attempt);
                                        }
                                    })
                                    .error((data: any, status: number): void => {
                                        $timeout.cancel(spinnerTimeoutPromise);
                                        modelHelpCtrl.isServerValidating = false;

                                        // when status is zero, user probably refreshed before this returned
                                        if (status === 0) return;

                                        // otherwise, something went wrong that we weren't expecting
                                        validateCtrl.setUnexpectedError();
                                    });

                            }, throttle);
                        });
                },
            };
            return directive;
        }];

    };

    //#endregion

    export var directive = directiveFactory();

}
