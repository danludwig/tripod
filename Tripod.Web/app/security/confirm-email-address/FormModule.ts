/// <amd-dependency path="angular"/>

'use strict';

import rcSubmit = require('../../ng/directives/PreventSubmit');
import c = require('./FormController');

export var ngModule = angular.module('test', [])
    .controller('FormController', c.FormController)
    .directive('rcSubmit', rcSubmit.rcSubmit)
;
return ngModule;