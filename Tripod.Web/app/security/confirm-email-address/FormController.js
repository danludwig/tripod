'use strict';
define(["require", "exports"], function(require, exports) {
    var FormController = (function () {
        function FormController($scope) {
            this.emailAddress = '';
            this.prop = 'asdf';
            $scope.m = this;
        }
        FormController.prototype.submit = function (arg1) {
            alert('submit');
            return false;
        };

        FormController.prototype.click = function (m) {
            alert('click');
        };

        FormController.prototype.hasError = function (field) {
            return field && field.$invalid && field.$dirty;
        };
        FormController.prototype.hasSuccess = function (field) {
            return field && field.$valid && field.$dirty;
        };
        FormController.$inject = ['$scope'];
        return FormController;
    })();
    exports.FormController = FormController;
});
