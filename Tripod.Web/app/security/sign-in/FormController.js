'use strict';
define(["require", "exports"], function(require, exports) {
    exports.controllerName = 'FormController';

    var FormController = (function () {
        function FormController($scope) {
            this.userName = '';
            this.password = '';
            this.isPersistent = false;
            $scope.m = this;
        }
        FormController.$inject = ['$scope'];
        return FormController;
    })();
    exports.FormController = FormController;
});
