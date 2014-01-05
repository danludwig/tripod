require.config({
    paths: {
        'angular': '../../../scripts/angular'
    },
    shim: {
        'angular': { 'exports': 'angular' }
    },
    priority: ['angular']
});

require(['angular', './FormModule'], function (angular, module) {
    'use strict';
    $(document).ready(function () {
        var node = $('#ng-app_signin');
        angular.bootstrap(node, [module['name']]);
    });
});
