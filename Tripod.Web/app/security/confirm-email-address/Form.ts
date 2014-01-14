'use strict';

module App.Security.SignUp.Form {

    export interface Model {
        emailAddress?: string;
        isExpectingEmail?: boolean;
    }

    export class Controller implements Model {

        emailAddress: string = '';
        isExpectingEmail: boolean = false;

        static $inject = ['$scope'];
        constructor($scope: ViewModelScope<Model>) {
            $scope.vm = this;
        }

        submit(arg1: any): boolean {
            alert('submit');
            return false;
        }

        click(m: any): void {
            //alert('click');
        }

        hasError(field: ng.INgModelController): boolean {
            return field && field.$invalid && field.$dirty;
        }
        hasSuccess(field: ng.INgModelController): boolean {
            return field && field.$valid && field.$dirty;
        }
    }

    export var moduleName = 'sign-up-form';

    export var ngModule = angular.module(moduleName, [Modules.Tripod.moduleName]);
}