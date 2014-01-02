'use strict';

export interface IFormModel {
    emailAddress?: string;
    isExpectingEmail?: boolean;
}

export interface IModelScope<T> extends ng.IScope {
    m: T;
}

export interface IFormScope extends IModelScope<IFormModel> { }

export var controllerName = 'FormController';

export class FormController implements IFormModel {

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