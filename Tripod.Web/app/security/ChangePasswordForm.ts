'use strict';

module App.Security.ChangePasswordForm {

    export interface ServerModel {
        oldPassword: string
        newPassword: string;
        confirmPassword: string;
    }

    export interface ViewModelSettings {
        element?: Element;
        isPostBack: boolean;
        oldPasswordRequiredMessage: string;
        newPasswordRequiredMessage: string;
        newPasswordMinLength: number;
        newPasswordMaxLength: number;
        newPasswordMinLengthMessage: string;
        newPasswordMaxLengthMessage: string;
        newPasswordValidateUrl: string;
        confirmPasswordRequiredMessage: string;
        confirmPasswordEqualsMessage: string;
        confirmPasswordValidateUrl: string;
    }

    export class ViewModel implements KnockoutValidationGroup {

        static create(settings: ViewModelSettings): ViewModel {
            return new ViewModel(settings);
        }

        oldPassword = ko.observable<string>();
        newPassword = ko.observable<string>();
        confirmPassword = ko.observable<string>();

        isValid: () => boolean;
        errors: KnockoutValidationErrors;
        newPasswordCss: KnockoutComputed<string>;
        oldPasswordValidation = new Widgets.ValidationState(this.settings.isPostBack);
        newPasswordValidation = new Widgets.ValidationState(this.settings.isPostBack);
        confirmPasswordValidation = new Widgets.ValidationState(this.settings.isPostBack);
        isValidating: KnockoutComputed<boolean>;

        oldPasswordTooltip: Widgets.BootstrapTooltip;
        $oldPasswordTooltip: JQuery;
        newPasswordTooltip: Widgets.BootstrapTooltip;
        $newPasswordTooltip: JQuery;
        confirmPasswordTooltip: Widgets.BootstrapTooltip;
        $confirmPasswordTooltip: JQuery;

        isSubmitWaiting = ko.observable(false);
        isSubmitDisabled: KnockoutComputed<boolean>;
        isSubmitError: KnockoutComputed<boolean>;
        isSubmitReady: KnockoutComputed<boolean>;
        submitCss: KnockoutComputed<string>;

        constructor(public settings: ViewModelSettings) {
            this.initValidation();
            if (this.settings.element)
                this.applyBindings(this.settings.element);
        }

        applyBindings(element: Element): void {
            this.settings.element = element;
            ko.applyBindings(this, element);
            this._initTooltips();
        }

        private initValidation(): void {

            ko.validation.rules['newPasswordServer'] = this._validateNewPassword();
            ko.validation.rules['confirmPasswordServer'] = this._validateConfirmPassword();
            ko.validation.registerExtenders();

            this.oldPassword.extend({
                required: {
                    params: true,
                    message: this.settings.oldPasswordRequiredMessage,
                },
                //minLengthCustom: {
                //    params: this.settings.newPasswordMinLength,
                //    messageTemplate: this.settings.newPasswordMinLengthMessage,
                //},
                //maxLengthCustom: {
                //    params: this.settings.newPasswordMaxLength,
                //    messageTemplate: this.settings.newPasswordMaxLengthMessage,
                //},
                //newPasswordServer: this,
            });
            this.newPassword.extend({
                //required: {
                //    params: true,
                //    message: this.settings.newPasswordRequiredMessage,
                //},
                //minLengthCustom: {
                //    params: this.settings.newPasswordMinLength,
                //    messageTemplate: this.settings.newPasswordMinLengthMessage,
                //},
                //maxLengthCustom: {
                //    params: this.settings.newPasswordMaxLength,
                //    messageTemplate: this.settings.newPasswordMaxLengthMessage,
                //},
                newPasswordServer: this,
            });
            this.confirmPassword.extend({
                //required: {
                //    params: true,
                //    message: this.settings.confirmPasswordRequiredMessage,
                //},
                //equalTo: {
                //    params: this.newPassword,
                //    message: this.settings.confirmPasswordEqualsMessage,
                //},
                confirmPasswordServer: this,
            });

            ko.validation.configure({
                insertMessages: false,
                messagesOnModified: !this.settings.isPostBack,
            });
            ko.validation.group(this);

            if (this.settings.isPostBack) this.errors.showAllMessages();

            this.newPassword.subscribe((): void => {
                this.confirmPassword.isValid.notifySubscribers(false);
            });

            this.oldPasswordValidation.observe(this.oldPassword);
            this.newPasswordValidation.observe(this.newPassword);
            this.confirmPasswordValidation.observe(this.confirmPassword);

            this.isValidating = ko.computed((): boolean => {
                return this.newPassword.isValidating() || this.confirmPassword.isValidating();
            });

            this.isSubmitError = ko.computed((): boolean => {
                return !this.isSubmitWaiting() && !this.isValid() && this.isPostBack();
            });

            this.isSubmitReady = ko.computed((): boolean => {
                return !this.isSubmitWaiting() && !this.isSubmitError();
            });

            this.isSubmitDisabled = ko.computed((): boolean => {
                return this.isSubmitWaiting() || this.isSubmitError();
            });

            this.submitCss = ko.computed((): string => {
                return this.isSubmitError() ? 'btn-danger' : '';
            });
        }

        private _initTooltips(): void {
            var tooltipOptions: TooltipOptions = {
                animation: false,
                trigger: 'manual',
                placement: 'right',
            };
            this.oldPasswordTooltip = new Widgets.BootstrapTooltip(this.$oldPasswordTooltip, tooltipOptions);
            ko.computed((): void => {
                this.oldPassword();
                if (this.oldPasswordValidation.hasSuccess() && !this.oldPasswordValidation.hasError()) {
                    this.oldPasswordTooltip.hide();
                }
                else if (!this.oldPasswordValidation.hasSuccess() && this.oldPasswordValidation.hasError()) {
                    this.oldPasswordTooltip.title(this.oldPassword.error);
                }
            });
            this.newPasswordTooltip = new Widgets.BootstrapTooltip(this.$newPasswordTooltip, tooltipOptions);
            ko.computed((): void => {
                this.newPassword();
                if (this.newPasswordValidation.hasSuccess() && !this.newPasswordValidation.hasError()) {
                    this.newPasswordTooltip.hide();
                }
                else if (!this.newPasswordValidation.hasSuccess() && this.newPasswordValidation.hasError()) {
                    this.newPasswordTooltip.title(this.newPassword.error);
                }
            });
            this.confirmPasswordTooltip = new Widgets.BootstrapTooltip(this.$confirmPasswordTooltip, tooltipOptions);
            ko.computed((): void => {
                this.confirmPassword();
                if (this.confirmPasswordValidation.hasSuccess() && !this.confirmPasswordValidation.hasError()) {
                    this.confirmPasswordTooltip.hide();
                }
                else if (!this.confirmPasswordValidation.hasSuccess() && this.confirmPasswordValidation.hasError()) {
                    this.confirmPasswordTooltip.title(this.confirmPassword.error);
                }
            });
        }

        private _validateNewPassword(): KnockoutValidationAsyncRuleDefinition {
            var ruleDefinition: KnockoutValidationAsyncRuleDefinition = {
                async: true,
                validator: (value: string, params: ViewModel, callback: KnockoutValidationAsyncCallback): void => {

                    if (this.newPasswordValidation.hasAsyncResult(value, callback)) return;

                    var asyncSettings: JQueryAjaxSettings = {
                        url: this.settings.newPasswordValidateUrl,
                        type: 'POST',
                        data: {
                            newPassword: value,
                        },
                    };
                    this.newPasswordValidation.doAsync(asyncSettings, this.settings.element, 'newPassword', value, callback);
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
                            newPassword: params.newPassword(),
                            confirmPassword: value,
                        },
                    };
                    this.confirmPasswordValidation.doAsync(asyncSettings, this.settings.element, 'confirmPassword', value, callback, false);
                },
                message: 'Invalid.',
            };
            return ruleDefinition;
        }

        onSubmit(formElement: Element): boolean {

            if (this.isValidating()) {
                this.isSubmitWaiting(true);
                setTimeout((): void => {
                    $(formElement).submit();
                }, 10);
                return false;
            }

            if (!this.isValid()) {
                this._isPostBack();
                this.errors.showAllMessages();
                this.isSubmitWaiting(false);
                return false;
            }

            //if (!formElement) return true;
            //this.onSubmit(null);
            //return false;
            return this.isValid();
            //return true;
        }

        isPostBack = ko.observable(this.settings.isPostBack);
        private _isPostBack(): void {
            this.newPasswordValidation.isPostBack(true);
            this.confirmPasswordValidation.isPostBack(true);
            this.isPostBack(true);
        }
    }
}
