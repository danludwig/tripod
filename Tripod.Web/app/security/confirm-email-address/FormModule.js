'use strict';
var App;
(function (App) {
    (function (Security) {
        (function (SignUp) {
            (function (Form) {
                Form.moduleName = 'sign-up-form';

                Form.ngModule = angular.module(Form.moduleName, [App.Modules.Tripod.moduleName]);
            })(SignUp.Form || (SignUp.Form = {}));
            var Form = SignUp.Form;
        })(Security.SignUp || (Security.SignUp = {}));
        var SignUp = Security.SignUp;
    })(App.Security || (App.Security = {}));
    var Security = App.Security;
})(App || (App = {}));
