require.config({
    paths: {
        'angular': '../../../scripts/angular',
        'ui-bootstrap': '../../../scripts/ui-bootstrap-tpls-0.9.0',
    },
    shim: {
        'angular': { 'exports': 'angular' },
        'ui-bootstrap': { deps: ['angular'], 'exports': 'ui-bootstrap' },
    },
    priority: ['angular', 'ui-bootstrap'],

});

require(['angular', './FormModule', 'ui-bootstrap'], (angular: ng.IAngularStatic, module: ng.IModule) => {
    'use strict';
    $(document).ready((): void=> {
        var node = $('#ng-app_signin');
        angular.bootstrap(node, [module['name']]);
    });
});