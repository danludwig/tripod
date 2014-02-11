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
                    this.isSubmitWaiting = ko.observable(false);
                    this.isPostBack = ko.observable(this.settings.isPostBack);
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
                    this._initTooltips();
                };

                ViewModel.prototype.initValidation = function () {
                    var _this = this;
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

                    this.password.subscribe(function () {
                        _this.confirmPassword.isValid.notifySubscribers(false);
                    });

                    this.passwordValidation.observe(this.password);
                    this.confirmPasswordValidation.observe(this.confirmPassword);

                    this.isValidating = ko.computed(function () {
                        return _this.password.isValidating() || _this.confirmPassword.isValidating();
                    });

                    this.isSubmitError = ko.computed(function () {
                        return !_this.isSubmitWaiting() && !_this.isValid() && _this.isPostBack();
                    });

                    this.isSubmitReady = ko.computed(function () {
                        return !_this.isSubmitWaiting() && !_this.isSubmitError();
                    });

                    this.isSubmitDisabled = ko.computed(function () {
                        return _this.isSubmitWaiting() || _this.isSubmitError();
                    });

                    this.submitCss = ko.computed(function () {
                        return _this.isSubmitError() ? 'btn-danger' : '';
                    });
                };

                ViewModel.prototype._initTooltips = function () {
                    var _this = this;
                    var tooltipOptions = {
                        animation: false,
                        trigger: 'manual',
                        placement: 'right'
                    };
                    this.passwordTooltip = new App.Widgets.BootstrapTooltip(this.$passwordTooltip, tooltipOptions);
                    ko.computed(function () {
                        _this.password();
                        if (_this.passwordValidation.hasSuccess() && !_this.passwordValidation.hasError()) {
                            _this.passwordTooltip.hide();
                        } else if (!_this.passwordValidation.hasSuccess() && _this.passwordValidation.hasError()) {
                            _this.passwordTooltip.title(_this.password.error);
                        }
                    });
                    this.confirmPasswordTooltip = new App.Widgets.BootstrapTooltip(this.$confirmPasswordTooltip, tooltipOptions);
                    ko.computed(function () {
                        _this.confirmPassword();
                        if (_this.confirmPasswordValidation.hasSuccess() && !_this.confirmPasswordValidation.hasError()) {
                            _this.confirmPasswordTooltip.hide();
                        } else if (!_this.confirmPasswordValidation.hasSuccess() && _this.confirmPasswordValidation.hasError()) {
                            _this.confirmPasswordTooltip.title(_this.confirmPassword.error);
                        }
                    });
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

                ViewModel.prototype.onSubmit = function (formElement) {
                    if (this.isValidating()) {
                        this.isSubmitWaiting(true);
                        setTimeout(function () {
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

                    return this.isValid();
                };

                ViewModel.prototype._isPostBack = function () {
                    this.passwordValidation.isPostBack(true);
                    this.confirmPasswordValidation.isPostBack(true);
                    this.isPostBack(true);
                };
                return ViewModel;
            })();
            CreatePasswordForm.ViewModel = ViewModel;
        })(Security.CreatePasswordForm || (Security.CreatePasswordForm = {}));
        var CreatePasswordForm = Security.CreatePasswordForm;
    })(App.Security || (App.Security = {}));
    var Security = App.Security;
})(App || (App = {}));
