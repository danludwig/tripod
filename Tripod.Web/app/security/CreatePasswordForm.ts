'use strict';

module App.Security.CreatePasswordForm {

    export interface ServerModel {
        password: string;
        confirmPassword: string;
    }

    export interface ViewModelSettings {
        element?: Element;
        isPostBack: boolean;
        passwordValidateUrl: string;
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
            this.passwordValidation.observe(this.password);
            this.confirmPasswordValidation.observe(this.confirmPassword);

            ko.validation.rules['passwordServer'] = this._validatePassword();
            ko.validation.rules['confirmPasswordServer'] = this._validateConfirmPassword();
            ko.validation.registerExtenders();

            this.password.extend({
                passwordServer: this,
            });
            this.confirmPassword.extend({
                confirmPasswordServer: this,
            });

            ko.validation.configure({
                insertMessages: false,
                messagesOnModified: !this.settings.isPostBack,
            });
            ko.validation.group(this);

            if (this.settings.isPostBack) this.errors.showAllMessages();

            this.password.subscribe((): void => {
                this.confirmPassword.isValid.valueHasMutated();
            });
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
                this.errors.showAllMessages();
            }

            return this.isValid();
            //return true;
        }

    }
}
