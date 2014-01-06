require.config({
    paths: {
        'angular': '../../../scripts/angular',
        'ui-bootstrap': '../../../scripts/ui-bootstrap-tpls-0.9.0'
    },
    shim: {
        'angular': { 'exports': 'angular' },
        'ui-bootstrap': { deps: ['angular'], 'exports': 'ui-bootstrap' }
    },
    priority: ['angular', 'ui-bootstrap']
});

require(['angular', './FormModule', 'ui-bootstrap'], function (angular, module) {
    'use strict';
    $(document).ready(function () {
        var node = $('#ng-app_signin');
        angular.bootstrap(node, [module['name']]);
    });
});
