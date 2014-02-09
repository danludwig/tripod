module App.Widgets {

    export class ValidationState {

        private _field = ko.observable<KnockoutObservable<any>>();
        isSpinning = ko.observable<boolean>();

        constructor(public isPostBack: boolean) {}

        observe(field: KnockoutObservable<any>): void {
            this._field(field);
        }

        hasError = ko.computed((): boolean => {
            var field = this._field();
            if (!field || !field.isModified || !field.isValid) return false;
            return !this.isSpinning() && (field.isModified() || this.isPostBack) && !field.isValid();
        });

        hasSuccess = ko.computed((): boolean => {
            var field = this._field();
            if (!field || !field.isModified || !field.isValid) return false;
            return !this.isSpinning() && (field.isModified() || this.isPostBack) && field.isValid();
        });

        fieldCss = ko.computed((): string => {
            if (this.hasError()) return 'has-error';
            if (this.hasSuccess()) return 'has-success';
            return null;
        });

        hasNoAddOn = ko.computed((): boolean => {
            return !this.hasError() && !this.hasSuccess() && !this.isSpinning();
        });

        inputGroupCss(css: string): string {
            return this.hasError() || this.hasSuccess() || this.isSpinning() ? css : null;
        }

    }
}