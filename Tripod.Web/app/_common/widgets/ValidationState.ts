module App.Widgets {

    export class ValidationState {

        private _field = ko.observable<KnockoutObservable<any>>();
        private _lastState = '';
        spinner = new Spinner({ delay: 200 });
        isPostBack = ko.observable<boolean>();

        constructor(isPostBack: boolean) {
            this.isPostBack(isPostBack);
        }

        observe(field: KnockoutObservable<any>): void {
            this._field(field);
        }

        private _has(errorOrSuccess: string): boolean {
            var field = this._field();
            var isSpinnerRunning = this.spinner.isRunning();
            var isSpinnerVisible = this.spinner.isVisible();
            var isPostBack = this.isPostBack();

            if (!field || !field.isModified || !field.isValid) return false;

            var isFieldModified = field.isModified() || isPostBack;
            var isValidating = field.isValidating();
            var isValid = field.isValid();

            if (isSpinnerRunning && !isSpinnerVisible && this._lastState == errorOrSuccess) return true;

            var has = !isSpinnerVisible && isFieldModified && !isValidating;
            if (errorOrSuccess == 'error') has = has && !isValid;
            if (errorOrSuccess == 'success') has = has && isValid;
            if (has) this._lastState = errorOrSuccess;
            return has;
        }

        hasError = ko.computed((): boolean => {
            return this._has('error');
        });

        hasSuccess = ko.computed((): boolean => {
            return this._has('success');
        });

        fieldCss = ko.computed((): any => {
            return {
                'has-error': this.hasError(),
                'has-success': this.hasSuccess(),
                'has-spinner': this.spinner.isVisible(),
            };
        });

        hasNoAddOn = ko.computed((): boolean => {
            return !this.hasError() && !this.hasSuccess() && !this.spinner.isVisible();
        });

        inputGroupCss(css: string): string {
            return this.hasError() || this.hasSuccess() || this.spinner.isVisible() ? css : '';
        }

        private _asyncResults: any[] = [];

        asyncResults(value: any, validation: ValidatedField): void {
            this._asyncResults.push({
                value: value,
                validation: validation,
            });
        }
        asyncResult(value: any): ValidatedField {
            for (var i = 0; i < this._asyncResults.length; i++) {
                var result = this._asyncResults[i];
                if (result.value === value) return result.validation;
            }
            return null;
        }

        hasAsyncResult(value: any, callback: KnockoutValidationAsyncCallback): boolean {
            var field = this.asyncResult(value);
            if (field) {
                this.asyncXhr.abort('stale');
                this.spinner.stop();
                this.setAsyncValidity(field, callback);
                return true;
            }
            return false;
        }

        setAsyncValidity(field: ValidatedField, callback: KnockoutValidationAsyncCallback): void {
            setTimeout((): void => {
                if (field.isValid) callback(true);
                else callback({
                    isValid: false,
                    message: field.errors[0].message,
                });
            }, 0);
        }

        asyncXhr: JQueryXHR;

        doAsync(settings: JQueryAjaxSettings, element: Element, fieldName: string, value: any, callback: KnockoutValidationAsyncCallback, cache: boolean = true): void {
            if (this.asyncXhr) this.asyncXhr.abort('stale');
            var field = this._field();
            if (!field) return;
            if (field.isModified()) this.spinner.start();
            if (element && settings.data) {
                var token = $(element).find('input[name=__RequestVerificationToken]').val();
                settings.data['__RequestVerificationToken'] = token;
            }
            this.asyncXhr = $.ajax(settings);
            this.asyncXhr.done((response: any): void => {
                var validated: ValidatedField = response[fieldName];
                if (cache) this.asyncResults(value, validated);
                this.setAsyncValidity(validated, callback);
                this.spinner.stop();
            });
            this.asyncXhr.fail((xhr: JQueryXHR, textStatus: string): void => {
                if (textStatus == 'stale') return;
                this.spinner.stop();
            });
        }
    }
}