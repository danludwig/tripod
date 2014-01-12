'use strict';
var App;
(function (App) {
    (function (Directives) {
        (function (Popover) {
            Popover.directiveName = 't3Popover';

            var directiveFactory = function () {
                return [
                    '$parse', '$timeout', function ($parse, $timeout) {
                        var directive = {
                            name: Popover.directiveName,
                            restrict: 'A',
                            link: function (scope, element, attrs) {
                                var options = {
                                    content: $parse(attrs[Popover.directiveName])(scope),
                                    trigger: 'manual',
                                    animation: typeof attrs['t3PopoverAnimation'] === 'string' ? attrs['t3PopoverAnimation'].toLowerCase() !== 'false' : true
                                };

                                var initPopup = function () {
                                    var data = element.data(Popover.directiveName);
                                    if (data) {
                                        element.popover('destroy');
                                    }
                                    element.popover(options);
                                    element.data(Popover.directiveName, true);
                                };

                                initPopup();

                                var isVisible = false;
                                scope.$watch(attrs[Popover.directiveName], function (value) {
                                    if (value != options.content) {
                                        options.content = value;
                                        initPopup();
                                        if (isVisible)
                                            element.popover('show');
                                    }
                                });

                                var redrawPromise;
                                $(window).on('resize', function () {
                                    if (redrawPromise)
                                        $timeout.cancel(redrawPromise);
                                    redrawPromise = $timeout(function () {
                                        if (!isVisible)
                                            return;
                                        element.popover('hide');
                                        element.popover('show');
                                    }, 100);
                                });

                                scope.$watch(attrs['t3PopoverSwitch'], function (value) {
                                    if (value) {
                                        isVisible = true;
                                        element.popover('show');
                                    } else {
                                        isVisible = false;
                                        element.popover('hide');
                                    }
                                });
                            }
                        };
                        return directive;
                    }];
            };

            Popover.directive = directiveFactory();
        })(Directives.Popover || (Directives.Popover = {}));
        var Popover = Directives.Popover;
    })(App.Directives || (App.Directives = {}));
    var Directives = App.Directives;
})(App || (App = {}));
