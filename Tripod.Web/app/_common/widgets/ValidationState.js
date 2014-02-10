var App;
(function (App) {
    (function (Widgets) {
        var ValidationState = (function () {
            function ValidationState(isPostBack) {
                var _this = this;
                this._field = ko.observable();
                this._lastState = '';
                this.spinner = new App.Widgets.Spinner({ delay: 200 });
                this.isPostBack = ko.observable();
                this.hasError = ko.computed(function () {
                    return _this._has('error');
                });
                this.hasSuccess = ko.computed(function () {
                    return _this._has('success');
                });
                this.fieldCss = ko.computed(function () {
                    if (_this.hasError())
                        return 'has-error';
                    if (_this.hasSuccess())
                        return 'has-success';
                    if (_this.spinner.isVisible())
                        return 'has-spinner';
                    return '';
                });
                this.hasNoAddOn = ko.computed(function () {
                    return !_this.hasError() && !_this.hasSuccess() && !_this.spinner.isVisible();
                });
                this._asyncResults = [];
                this.isPostBack(isPostBack);
            }
            ValidationState.prototype.observe = function (field) {
                this._field(field);
            };

            ValidationState.prototype._has = function (errorOrSuccess) {
                var field = this._field();
                var isSpinnerRunning = this.spinner.isRunning();
                var isSpinnerVisible = this.spinner.isVisible();
                var isPostBack = this.isPostBack();

                if (!field || !field.isModified || !field.isValid)
                    return false;

                var isFieldModified = field.isModified() || isPostBack;
                var isValidating = field.isValidating();
                var isValid = field.isValid();

                if (isSpinnerRunning && !isSpinnerVisible && this._lastState == errorOrSuccess)
                    return true;

                var has = !isSpinnerVisible && isFieldModified && !isValidating;
                if (errorOrSuccess == 'error')
                    has = has && !isValid;
                if (errorOrSuccess == 'success')
                    has = has && isValid;
                if (has)
                    this._lastState = errorOrSuccess;
                return has;
            };

            ValidationState.prototype.inputGroupCss = function (css) {
                return this.hasError() || this.hasSuccess() || this.spinner.isVisible() ? css : '';
            };

            ValidationState.prototype.asyncResults = function (value, validation) {
                this._asyncResults.push({
                    value: value,
                    validation: validation
                });
            };
            ValidationState.prototype.asyncResult = function (value) {
                for (var i = 0; i < this._asyncResults.length; i++) {
                    var result = this._asyncResults[i];
                    if (result.value === value)
                        return result.validation;
                }
                return null;
            };

            ValidationState.prototype.hasAsyncResult = function (value, callback) {
                var field = this.asyncResult(value);
                if (field) {
                    this.asyncXhr.abort('stale');
                    this.spinner.stop();
                    this.setAsyncValidity(field, callback);
                    return true;
                }
                return false;
            };

            ValidationState.prototype.setAsyncValidity = function (field, callback) {
                setTimeout(function () {
                    if (field.isValid)
                        callback(true);
                    else
                        callback({
                            isValid: false,
                            message: field.errors[0].message
                        });
                }, 0);
            };

            ValidationState.prototype.doAsync = function (settings, element, fieldName, value, callback, cache) {
                if (typeof cache === "undefined") { cache = true; }
                var _this = this;
                if (this.asyncXhr)
                    this.asyncXhr.abort('stale');
                if (this._field().isModified())
                    this.spinner.start();
                if (element && settings.data) {
                    var token = $(element).find('input[name=__RequestVerificationToken]').val();
                    settings.data['__RequestVerificationToken'] = token;
                }
                this.asyncXhr = $.ajax(settings);
                this.asyncXhr.done(function (response) {
                    var field = response[fieldName];
                    if (cache)
                        _this.asyncResults(value, field);
                    _this.setAsyncValidity(field, callback);
                    _this.spinner.stop();
                });
                this.asyncXhr.fail(function (xhr, textStatus) {
                    if (textStatus == 'stale')
                        return;
                    _this.spinner.stop();
                });
            };
            return ValidationState;
        })();
        Widgets.ValidationState = ValidationState;
    })(App.Widgets || (App.Widgets = {}));
    var Widgets = App.Widgets;
})(App || (App = {}));
