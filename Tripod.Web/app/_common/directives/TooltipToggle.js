'use strict';
var App;
(function (App) {
    (function (Directives) {
        (function (TooltipToggle) {
            function directiveSettings(tooltipOrPopover) {
                if (typeof tooltipOrPopover === "undefined") { tooltipOrPopover = 'tooltip'; }
                var directiveName = tooltipOrPopover;

                var showEvent = 'show-' + directiveName;
                var hideEvent = 'hide-' + directiveName;

                var directiveConfig = [
                    '$tooltipProvider', function ($tooltipProvider) {
                        var trigger = {};
                        trigger[showEvent] = hideEvent;
                        $tooltipProvider.setTriggers(trigger);
                    }];

                var directiveFactory = function () {
                    return [
                        '$timeout', function ($timeout) {
                            var d = {
                                name: directiveName,
                                restrict: 'A',
                                link: function (scope, element, attr) {
                                    if (angular.isUndefined(attr[directiveName + 'Toggle']))
                                        return;

                                    attr[directiveName + 'Trigger'] = showEvent;

                                    var redrawPromise;
                                    $(window).on('resize', function () {
                                        if (redrawPromise)
                                            $timeout.cancel(redrawPromise);
                                        redrawPromise = $timeout(function () {
                                            if (!scope['tt_isOpen'])
                                                return;
                                            element.triggerHandler(hideEvent);
                                            element.triggerHandler(showEvent);
                                        }, 100);
                                    });

                                    scope.$watch(attr[directiveName + 'Toggle'], function (value) {
                                        if (value && !scope['tt_isOpen']) {
                                            $timeout(function () {
                                                element.triggerHandler(showEvent);
                                            });
                                        } else if (!value && scope['tt_isOpen']) {
                                            $timeout(function () {
                                                element.triggerHandler(hideEvent);
                                            });
                                        }
                                    });

                                    attr.$observe(directiveName, function (value) {
                                        if (value && scope['tt_isOpen']) {
                                            $timeout(function () {
                                                element.triggerHandler(hideEvent);
                                                element.triggerHandler(showEvent);
                                            });
                                        }
                                    });
                                }
                            };
                            return d;
                        }];
                };

                var directive = directiveFactory();

                var directiveSettings = {
                    directiveName: directiveName,
                    directive: directive,
                    directiveConfig: directiveConfig
                };

                return directiveSettings;
            }
            TooltipToggle.directiveSettings = directiveSettings;
        })(Directives.TooltipToggle || (Directives.TooltipToggle = {}));
        var TooltipToggle = Directives.TooltipToggle;
    })(App.Directives || (App.Directives = {}));
    var Directives = App.Directives;
})(App || (App = {}));
