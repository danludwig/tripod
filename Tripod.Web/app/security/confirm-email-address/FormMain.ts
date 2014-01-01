require.config({
    paths: {
        'angular': '../../../scripts/angular'
    },
    shim: {
        'angular': { 'exports': 'angular' }
    },
    priority: ['angular'],

});

require(['angular', './FormModule'], (angular: ng.IAngularStatic, module: ng.IModule) => {
    'use strict';
    $(document).ready((): void=> {
        var node = $('#ng-app_test');
        angular.bootstrap(node, [module['name']]);
    });
});
