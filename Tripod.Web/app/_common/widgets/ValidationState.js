var App;
(function (App) {
    (function (Widgets) {
        var ValidationState = (function () {
            function ValidationState(isPostBack) {
                var _this = this;
                this.isPostBack = isPostBack;
                this._field = ko.observable();
                this.isSpinning = ko.observable();
                this.hasError = ko.computed(function () {
                    var field = _this._field();
                    if (!field || !field.isModified || !field.isValid)
                        return false;
                    return !_this.isSpinning() && (field.isModified() || _this.isPostBack) && !field.isValid();
                });
                this.hasSuccess = ko.computed(function () {
                    var field = _this._field();
                    if (!field || !field.isModified || !field.isValid)
                        return false;
                    return !_this.isSpinning() && (field.isModified() || _this.isPostBack) && field.isValid();
                });
                this.fieldCss = ko.computed(function () {
                    if (_this.hasError())
                        return 'has-error';
                    if (_this.hasSuccess())
                        return 'has-success';
                    return null;
                });
                this.hasNoAddOn = ko.computed(function () {
                    return !_this.hasError() && !_this.hasSuccess() && !_this.isSpinning();
                });
            }
            ValidationState.prototype.observe = function (field) {
                this._field(field);
            };

            ValidationState.prototype.inputGroupCss = function (css) {
                return this.hasError() || this.hasSuccess() || this.isSpinning() ? css : null;
            };
            return ValidationState;
        })();
        Widgets.ValidationState = ValidationState;
    })(App.Widgets || (App.Widgets = {}));
    var Widgets = App.Widgets;
})(App || (App = {}));
