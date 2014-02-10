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
                    var _this = this;
                    ko.validation.rules['passwordServer'] = this._validatePassword();
                    ko.validation.rules['confirmPasswordServer'] = this._validateConfirmPassword();
                    ko.validation.registerExtenders();

                    this.password.extend({
                        required: {
                            params: true,
                            message: this.settings.passwordRequiredMessage
                        },
                        minLengthCustom: {
                            params: this.settings.passwordMinLength,
                            messageTemplate: this.settings.passwordMinLengthMessage
                        },
                        maxLengthCustom: {
                            params: this.settings.passwordMaxLength,
                            messageTemplate: this.settings.passwordMaxLengthMessage
                        },
                        passwordServer: this
                    });
                    this.confirmPassword.extend({
                        required: {
                            params: true,
                            message: this.settings.confirmPasswordRequiredMessage
                        },
                        equalTo: {
                            params: this.password,
                            message: this.settings.confirmPasswordEqualsMessage
                        },
                        confirmPasswordServer: this
                    });

                    ko.validation.configure({
                        insertMessages: false,
                        messagesOnModified: !this.settings.isPostBack
                    });
                    ko.validation.group(this);

                    if (this.settings.isPostBack)
                        this.errors.showAllMessages();

                    this.password.subscribe(function () {
                        _this.confirmPassword.isValid.notifySubscribers(false);
                    });

                    this.passwordValidation.observe(this.password);
                    this.confirmPasswordValidation.observe(this.confirmPassword);
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
                            _this.confirmPasswordValidation.doAsync(asyncSettings, _this.settings.element, 'confirmPassword', value, callback, false);
                        },
                        message: 'Invalid.'
                    };
                    return ruleDefinition;
                };

                ViewModel.prototype.onSubmit = function () {
                    if (!this.isValid()) {
                        this._isPostBack();
                        this.errors.showAllMessages();
                        return false;
                    }

                    return this.isValid();
                };

                ViewModel.prototype._isPostBack = function () {
                    this.passwordValidation.isPostBack(true);
                    this.confirmPasswordValidation.isPostBack(true);
                };
                return ViewModel;
            })();
            CreatePasswordForm.ViewModel = ViewModel;
        })(Security.CreatePasswordForm || (Security.CreatePasswordForm = {}));
        var CreatePasswordForm = Security.CreatePasswordForm;
    })(App.Security || (App.Security = {}));
    var Security = App.Security;
})(App || (App = {}));
