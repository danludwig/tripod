'use strict';

module App.Security.SignIn.Form {

    export interface Model {
        userName?: string;
        password?: string;
        isPersistent?: boolean;
    }

    export class Controller implements Model {

        userName: string = '';
        password: string = '';
        isPersistent: boolean = false;

        static $inject = ['$scope'];
        constructor($scope: ViewModelScope<Model>) {
            $scope.vm = this;
        }

        someMethod(): void {
            alert('doing some method');
        }
    }

    export var moduleName = 'sign-in-form';

    export var ngModule = angular.module(moduleName, [Modules.Tripod.moduleName]);
}
