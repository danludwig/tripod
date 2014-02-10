'use strict';

module App.Security.CreatePasswordForm {

    export interface ServerModel {
        password: string;
        confirmPassword: string;
    }

    export interface ViewModelSettings {
        element?: Element;
        isPostBack: boolean;
        passwordRequiredMessage: string;
        passwordMinLength: number;
        passwordMaxLength: number;
        passwordMinLengthMessage: string;
        passwordMaxLengthMessage: string;
        passwordValidateUrl: string;
        confirmPasswordRequiredMessage: string;
        confirmPasswordEqualsMessage: string;
        confirmPasswordValidateUrl: string;
    }

    export class ViewModel implements KnockoutValidationGroup {

        static create(settings: ViewModelSettings): ViewModel {
            return new ViewModel(settings);
        }

        password = ko.observable<string>();
        confirmPassword = ko.observable<string>();

        isValid: () => boolean;
        errors: KnockoutValidationErrors;
        passwordCss: KnockoutComputed<string>;
        passwordValidation = new Widgets.ValidationState(this.settings.isPostBack);
        confirmPasswordValidation = new Widgets.ValidationState(this.settings.isPostBack);

        constructor(public settings: ViewModelSettings) {
            this.initValidation();
            if (this.settings.element)
                this.applyBindings(this.settings.element);
        }

        applyBindings(element: Element): void {
            this.settings.element = element;
            ko.applyBindings(this, element);
        }

        private initValidation(): void {

            ko.validation.rules['passwordServer'] = this._validatePassword();
            ko.validation.rules['confirmPasswordServer'] = this._validateConfirmPassword();
            ko.validation.registerExtenders();

            this.password.extend({
                required: {
                    params: true,
                    message: this.settings.passwordRequiredMessage,
                },
                minLengthCustom: {
                    params: this.settings.passwordMinLength,
                    messageTemplate: this.settings.passwordMinLengthMessage,
                },
                maxLengthCustom: {
                    params: this.settings.passwordMaxLength,
                    messageTemplate: this.settings.passwordMaxLengthMessage,
                },
                passwordServer: this,
            });
            this.confirmPassword.extend({
                required: {
                    params: true,
                    message: this.settings.confirmPasswordRequiredMessage,
                },
                equalTo: {
                    params: this.password,
                    message: this.settings.confirmPasswordEqualsMessage,
                },
                confirmPasswordServer: this,
            });

            ko.validation.configure({
                insertMessages: false,
                messagesOnModified: !this.settings.isPostBack,
            });
            ko.validation.group(this);

            if (this.settings.isPostBack) this.errors.showAllMessages();

            this.password.subscribe((): void => {
                this.confirmPassword.isValid.notifySubscribers(false);
            });

            this.passwordValidation.observe(this.password);
            this.confirmPasswordValidation.observe(this.confirmPassword);
        }

        private _validatePassword(): KnockoutValidationAsyncRuleDefinition {
            var ruleDefinition: KnockoutValidationAsyncRuleDefinition = {
                async: true,
                validator: (value: string, params: ViewModel, callback: KnockoutValidationAsyncCallback): void => {

                    if (this.passwordValidation.hasAsyncResult(value, callback)) return;

                    var asyncSettings: JQueryAjaxSettings = {
                        url: this.settings.passwordValidateUrl,
                        type: 'POST',
                        data: {
                            password: value,
                        },
                    };
                    this.passwordValidation.doAsync(asyncSettings, this.settings.element, 'password', value, callback);
                },
                message: 'Invalid.',
            };
            return ruleDefinition;
        }

        private _validateConfirmPassword(): KnockoutValidationAsyncRuleDefinition {
            var ruleDefinition: KnockoutValidationAsyncRuleDefinition = {
                async: true,
                validator: (value: string, params: ViewModel, callback: KnockoutValidationAsyncCallback): void => {

                    if (this.confirmPasswordValidation.hasAsyncResult(value, callback)) return;

                    var asyncSettings: JQueryAjaxSettings = {
                        url: this.settings.confirmPasswordValidateUrl,
                        type: 'POST',
                        data: {
                            password: params.password(),
                            confirmPassword: value,
                        },
                    };
                    this.confirmPasswordValidation.doAsync(asyncSettings, this.settings.element, 'confirmPassword', value, callback, false);
                },
                message: 'Invalid.',
            };
            return ruleDefinition;
        }

        onSubmit(): boolean {

            if (!this.isValid()) {
                this._isPostBack();
                this.errors.showAllMessages();
                return false;
            }

            return this.isValid();
            //return true;
        }

        private _isPostBack(): void {
            this.passwordValidation.isPostBack(true);
            this.confirmPasswordValidation.isPostBack(true);
        }

    }
}
