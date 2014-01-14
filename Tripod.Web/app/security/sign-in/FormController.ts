'use strict';

module App.Security.SignIn.Form {

    export class Controller implements Model {

        userName: string = '';
        password: string = '';
        isPersistent: boolean = false;

        static $inject = ['$scope'];
        constructor($scope: ViewModelScope<Model>) {
            $scope.vm = this;
        }
    }
}
