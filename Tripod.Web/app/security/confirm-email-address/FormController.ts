'use strict';

module App.Security.SignUp.Form {

    export interface IFormScope extends IModelScope<IFormModel> { }

    export class Controller implements IFormModel {

        emailAddress: string = '';
        isExpectingEmail: boolean = false;

        static $inject = ['$scope'];
        constructor($scope: IFormScope) {
            $scope.m = this;
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
}
