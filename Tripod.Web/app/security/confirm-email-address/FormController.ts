'use strict';

import iScope = require('../../_common/IModelScope');
import iModel = require('./FormModel');

export interface IFormScope extends iScope.IModelScope<iModel.IFormModel> { }

export var controllerName = 'FormController';

export class FormController implements iModel.IFormModel {

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