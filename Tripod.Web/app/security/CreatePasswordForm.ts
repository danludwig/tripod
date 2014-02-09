'use strict';

module App.Security.CreatePasswordForm {

    export interface ServerModel {
        password: string;
        confirmPassword: string;
    }

    export interface ViewModelSettings {
        element?: Element;
        isPostBack: boolean;
        passwordMinLength: number;
        passwordMaxLength: number;
        passwordValidateUrl: string;
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

        constructor(public settings: ViewModelSettings) {
            this.initValidation();
            if (this.settings.element)
                this.applyBindings(this.settings.element);
        }

        applyBindings(element: Element): void {
            ko.applyBindings(this, element);
        }

        private initValidation(): void {
            this.password.extend({
                required: true,
                minLength: this.settings.passwordMinLength,
                maxLength: this.settings.passwordMaxLength,
            });

            ko.validation.configure({
                insertMessages: false,
            });
            ko.validation.group(this);

            this.passwordValidation.observe(this.password);
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
