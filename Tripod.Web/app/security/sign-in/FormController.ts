'use strict';

module App.Security.SignIn.Form {

    export interface IFormScope extends IModelScope<IFormModel> { }

    export class Controller implements IFormModel {

        userName: string = '';
        password: string = '';
        isPersistent: boolean = false;

        static $inject = ['$scope'];
        constructor($scope: IFormScope) {
            $scope.m = this;
        }
    }
}
