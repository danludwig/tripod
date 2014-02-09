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
                    this.confirmPasswordValidation = new App.Widgets.ValidationState(this.settings.isPostBack);
                    this.initValidation();
                    if (this.settings.element)
                        this.applyBindings(this.settings.element);
                }
                ViewModel.create = function (settings) {
                    return new ViewModel(settings);
                };

                ViewModel.prototype.applyBindings = function (element) {
                    this.settings.element = element;
                    ko.applyBindings(this, element);
                };

                ViewModel.prototype.initValidation = function () {
                    this.passwordValidation.observe(this.password);
                    this.confirmPasswordValidation.observe(this.confirmPassword);

                    ko.validation.rules['passwordServer'] = this._validatePassword();
                    ko.validation.rules['confirmPasswordServer'] = this._validateConfirmPassword();
                    ko.validation.registerExtenders();

                    this.password.extend({
                        passwordServer: this
                    });
                    this.confirmPassword.extend({
                        confirmPasswordServer: this
                    });

                    ko.validation.configure({
                        insertMessages: false,
                        messagesOnModified: !this.settings.isPostBack
                    });
                    ko.validation.group(this);

                    if (this.settings.isPostBack)
                        this.errors.showAllMessages();
                };

                ViewModel.prototype._validatePassword = function () {
                    var _this = this;
                    var ruleDefinition = {
                        async: true,
                        validator: function (value, params, callback) {
                            if (_this.passwordValidation.hasAsyncResult(value, callback))
                                return;

                            var asyncSettings = {
                                url: _this.settings.passwordValidateUrl,
                                type: 'POST',
                                data: {
                                    password: value
                                }
                            };
                            _this.passwordValidation.doAsync(asyncSettings, _this.settings.element, 'password', value, callback);
                        },
                        message: 'Invalid.'
                    };
                    return ruleDefinition;
                };

                ViewModel.prototype._validateConfirmPassword = function () {
                    var _this = this;
                    var ruleDefinition = {
                        async: true,
                        validator: function (value, params, callback) {
                            if (_this.confirmPasswordValidation.hasAsyncResult(value, callback))
                                return;

                            var asyncSettings = {
                                url: _this.settings.confirmPasswordValidateUrl,
                                type: 'POST',
                                data: {
                                    password: params.password(),
                                    confirmPassword: value
                                }
                            };
                            _this.confirmPasswordValidation.doAsync(asyncSettings, _this.settings.element, 'confirmPassword', value, callback);
                        },
                        message: 'Invalid.'
                    };
                    return ruleDefinition;
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
