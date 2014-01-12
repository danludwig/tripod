'use strict';
var App;
(function (App) {
    (function (Security) {
        (function (SignUp) {
            (function (Form) {
                var Controller = (function () {
                    function Controller($scope) {
                        this.emailAddress = '';
                        this.isExpectingEmail = false;
                        $scope.m = this;
                    }
                    Controller.prototype.submit = function (arg1) {
                        alert('submit');
                        return false;
                    };

                    Controller.prototype.click = function (m) {
                    };

                    Controller.prototype.hasError = function (field) {
                        return field && field.$invalid && field.$dirty;
                    };
                    Controller.prototype.hasSuccess = function (field) {
                        return field && field.$valid && field.$dirty;
                    };
                    Controller.$inject = ['$scope'];
                    return Controller;
                })();
                Form.Controller = Controller;
            })(SignUp.Form || (SignUp.Form = {}));
            var Form = SignUp.Form;
        })(Security.SignUp || (Security.SignUp = {}));
        var SignUp = Security.SignUp;
    })(App.Security || (App.Security = {}));
    var Security = App.Security;
})(App || (App = {}));
