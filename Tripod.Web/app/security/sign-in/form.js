'use strict';
var App;
(function (App) {
    (function (Security) {
        (function (SignIn) {
            (function (Form) {
                Form.moduleName = 'sign-in-form';

                Form.ngModule = angular.module(Form.moduleName, [App.Modules.Tripod.moduleName]);
            })(SignIn.Form || (SignIn.Form = {}));
            var Form = SignIn.Form;
        })(Security.SignIn || (Security.SignIn = {}));
        var SignIn = Security.SignIn;
    })(App.Security || (App.Security = {}));
    var Security = App.Security;
})(App || (App = {}));

'use strict';
var App;
(function (App) {
    (function (Security) {
        (function (SignIn) {
            (function (Form) {
                var Controller = (function () {
                    function Controller($scope) {
                        this.userName = '';
                        this.password = '';
                        this.isPersistent = false;
                        $scope.vm = this;
                    }
                    Controller.$inject = ['$scope'];
                    return Controller;
                })();
                Form.Controller = Controller;
            })(SignIn.Form || (SignIn.Form = {}));
            var Form = SignIn.Form;
        })(Security.SignIn || (Security.SignIn = {}));
        var SignIn = Security.SignIn;
    })(App.Security || (App.Security = {}));
    var Security = App.Security;
})(App || (App = {}));

