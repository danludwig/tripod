'use strict';

import iScope = require('../../_common/IModelScope');
import iModel = require('./FormModel');

export interface IFormScope extends iScope.IModelScope<iModel.IFormModel> { }

export var controllerName = 'FormController';

export class FormController implements iModel.IFormModel {

    userName: string = '';
    password: string = '';
    isPersistent: boolean = false;

    static $inject = ['$scope'];
    constructor($scope: IFormScope) {
        $scope.m = this;
    }
} 