'use strict';
var App;
(function (App) {
    (function (Security) {
        (function (ChangePasswordForm) {
            var ViewModel = (function () {
                function ViewModel(settings) {
                    this.settings = settings;
                    this.oldPassword = ko.observable();
                    this.newPassword = ko.observable();
                    this.confirmPassword = ko.observable();
                    this.oldPasswordValidation = new App.Widgets.ValidationState(this.settings.isPostBack);
                    this.newPasswordValidation = new App.Widgets.ValidationState(this.settings.isPostBack);
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
                    ko.validation.rules['oldPasswordServer'] = this._validateOldPassword();
                    ko.validation.rules['newPasswordServer'] = this._validateNewPassword();
                    ko.validation.rules['confirmPasswordServer'] = this._validateConfirmPassword();
                    ko.validation.registerExtenders();

                    this.oldPassword.extend({
                        required: {
                            params: true,
                            message: this.settings.oldPasswordRequiredMessage
                        },
                        oldPasswordServer: this
                    });
                    this.newPassword.extend({
                        required: {
                            params: true,
                            message: this.settings.newPasswordRequiredMessage
                        },
                        minLengthCustom: {
                            params: this.settings.newPasswordMinLength,
                            messageTemplate: this.settings.newPasswordMinLengthMessage
                        },
                        maxLengthCustom: {
                            params: this.settings.newPasswordMaxLength,
                            messageTemplate: this.settings.newPasswordMaxLengthMessage
                        },
                        newPasswordServer: this
                    });
                    this.confirmPassword.extend({
                        required: {
                            params: true,
                            message: this.settings.confirmPasswordRequiredMessage
                        },
                        equalTo: {
                            params: this.newPassword,
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

                    this.newPassword.subscribe(function () {
                        _this.confirmPassword.isValid.notifySubscribers(false);
                    });

                    this.oldPasswordValidation.observe(this.oldPassword);
                    this.newPasswordValidation.observe(this.newPassword);
                    this.confirmPasswordValidation.observe(this.confirmPassword);

                    this.isValidating = ko.computed(function () {
                        return _this.newPassword.isValidating() || _this.confirmPassword.isValidating();
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
                    this.oldPasswordTooltip = new App.Widgets.BootstrapTooltip(this.$oldPasswordTooltip, tooltipOptions);
                    ko.computed(function () {
                        _this.oldPassword();
                        if (_this.oldPasswordValidation.hasSuccess() && !_this.oldPasswordValidation.hasError()) {
                            _this.oldPasswordTooltip.hide();
                        } else if (!_this.oldPasswordValidation.hasSuccess() && _this.oldPasswordValidation.hasError()) {
                            _this.oldPasswordTooltip.title(_this.oldPassword.error);
                        }
                    });
                    this.newPasswordTooltip = new App.Widgets.BootstrapTooltip(this.$newPasswordTooltip, tooltipOptions);
                    ko.computed(function () {
                        _this.newPassword();
                        if (_this.newPasswordValidation.hasSuccess() && !_this.newPasswordValidation.hasError()) {
                            _this.newPasswordTooltip.hide();
                        } else if (!_this.newPasswordValidation.hasSuccess() && _this.newPasswordValidation.hasError()) {
                            _this.newPasswordTooltip.title(_this.newPassword.error);
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

                ViewModel.prototype._validateOldPassword = function () {
                    var _this = this;
                    var ruleDefinition = {
                        async: true,
                        validator: function (value, params, callback) {
                            if (_this.oldPasswordValidation.hasAsyncResult(value, callback))
                                return;

                            var asyncSettings = {
                                url: _this.settings.oldPasswordValidateUrl,
                                type: 'POST',
                                data: {
                                    oldPassword: value
                                }
                            };
                            _this.oldPasswordValidation.doAsync(asyncSettings, _this.settings.element, 'oldPassword', value, callback);
                        },
                        message: 'Invalid.'
                    };
                    return ruleDefinition;
                };

                ViewModel.prototype._validateNewPassword = function () {
                    var _this = this;
                    var ruleDefinition = {
                        async: true,
                        validator: function (value, params, callback) {
                            if (_this.newPasswordValidation.hasAsyncResult(value, callback))
                                return;

                            var asyncSettings = {
                                url: _this.settings.newPasswordValidateUrl,
                                type: 'POST',
                                data: {
                                    newPassword: value
                                }
                            };
                            _this.newPasswordValidation.doAsync(asyncSettings, _this.settings.element, 'newPassword', value, callback);
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
                                    newPassword: params.newPassword(),
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
                    this.newPasswordValidation.isPostBack(true);
                    this.confirmPasswordValidation.isPostBack(true);
                    this.isPostBack(true);
                };
                return ViewModel;
            })();
            ChangePasswordForm.ViewModel = ViewModel;
        })(Security.ChangePasswordForm || (Security.ChangePasswordForm = {}));
        var ChangePasswordForm = Security.ChangePasswordForm;
    })(App.Security || (App.Security = {}));
    var Security = App.Security;
})(App || (App = {}));
