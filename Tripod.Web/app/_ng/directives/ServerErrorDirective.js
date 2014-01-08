'use strict';
define(["require", "exports"], function(require, exports) {
    exports.directiveName = 'serverError';

    //#region Directive
    var directiveFactory = function () {
        return function () {
            var directive = {
                name: exports.directiveName,
                restrict: 'A',
                require: ['ngModel', 'modelHelper'],
                link: function (scope, element, attr, ctrls) {
                    // don't initialize this unless there is a value in the attribute
                    var serverError = attr['serverError'];
                    if (!serverError)
                        return;

                    // passwords may not be valid on server, but will come back with empty box
                    var inputType = attr['type'];
                    if (!inputType || inputType.toLowerCase() != 'password')
                        return;

                    var modelCtrl = ctrls[0];
                    var helpCtrl = ctrls[1];

                    // set the server error text on the model helper controller
                    helpCtrl.serverError = serverError;

                    // initial watch to remove required error and set server error
                    var removeInitWatch = scope.$watch(function () {
                        return modelCtrl.$error;
                    }, function (value) {
                        if (value.required) {
                            modelCtrl.$setValidity('required', true);
                        }
                        modelCtrl.$setValidity('server', false);
                        removeInitWatch(); // remove this watch now
                    });

                    // remove server error when view value becomes dirty
                    var removeChangeWatch = scope.$watch(function () {
                        return modelCtrl.$viewValue;
                    }, function () {
                        if (modelCtrl.$dirty) {
                            modelCtrl.$setValidity('server', true);
                            removeChangeWatch();
                        }
                    });
                }
            };
            return directive;
        };
    };

    //#endregion
    exports.directive = directiveFactory();
});
