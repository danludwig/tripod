'use strict';
var App;
(function (App) {
    (function (Directives) {
        (function (TooltipToggle) {
            TooltipToggle.directiveName = 'tooltip';

            TooltipToggle.directiveConfig = [
                '$tooltipProvider', function ($tooltipProvider) {
                    $tooltipProvider.setTriggers({
                        'show-tooltip': 'hide-tooltip'
                    });
                }];

            var directiveFactory = function () {
                return [
                    '$timeout', function ($timeout) {
                        var directive = {
                            name: TooltipToggle.directiveName,
                            restrict: 'A',
                            link: function (scope, element, attr) {
                                attr[TooltipToggle.directiveName + 'Trigger'] = 'show-tooltip';

                                var redrawPromise;
                                $(window).on('resize', function () {
                                    if (redrawPromise)
                                        $timeout.cancel(redrawPromise);
                                    redrawPromise = $timeout(function () {
                                        if (!scope['tt_isOpen'])
                                            return;
                                        element.triggerHandler('hide-tooltip');
                                        element.triggerHandler('show-tooltip');
                                    }, 100);
                                });

                                scope.$watch(attr[TooltipToggle.directiveName + 'Toggle'], function (value) {
                                    if (value && !scope['tt_isOpen']) {
                                        $timeout(function () {
                                            element.triggerHandler('show-tooltip');
                                        });
                                    } else if (!value && scope['tt_isOpen']) {
                                        $timeout(function () {
                                            element.triggerHandler('hide-tooltip');
                                        });
                                    }
                                });
                            }
                        };
                        return directive;
                    }];
            };

            TooltipToggle.directive = directiveFactory();
        })(Directives.TooltipToggle || (Directives.TooltipToggle = {}));
        var TooltipToggle = Directives.TooltipToggle;
    })(App.Directives || (App.Directives = {}));
    var Directives = App.Directives;
})(App || (App = {}));
