'use strict';
var App;
(function (App) {
    (function (Security) {
        (function (CreatePasswordForm) {
            var ViewModel = (function () {
                function ViewModel(settings) {
                    this.settings = settings;
                    this.password = ko.observable();
                    this.confirmPassword = ko.observable();
                    this.passwordValidation = new App.Widgets.ValidationState(this.settings.isPostBack);
                    this.initValidation();
                    if (this.settings.element)
                        this.applyBindings(this.settings.element);
                }
                ViewModel.create = function (settings) {
                    return new ViewModel(settings);
                };

                ViewModel.prototype.applyBindings = function (element) {
                    ko.applyBindings(this, element);
                };

                ViewModel.prototype.initValidation = function () {
                    this.password.extend({
                        required: true,
                        minLength: this.settings.passwordMinLength,
                        maxLength: this.settings.passwordMaxLength
                    });

                    ko.validation.configure({
                        insertMessages: false
                    });
                    ko.validation.group(this);

                    this.passwordValidation.observe(this.password);
                };

                ViewModel.prototype.onSubmit = function () {
                    if (!this.isValid()) {
                        this.errors.showAllMessages();
                    }

                    return this.isValid();
                };
                return ViewModel;
            })();
            CreatePasswordForm.ViewModel = ViewModel;
        })(Security.CreatePasswordForm || (Security.CreatePasswordForm = {}));
        var CreatePasswordForm = Security.CreatePasswordForm;
    })(App.Security || (App.Security = {}));
    var Security = App.Security;
})(App || (App = {}));
