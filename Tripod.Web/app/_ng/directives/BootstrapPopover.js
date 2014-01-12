'use strict';
define(["require", "exports"], function(require, exports) {
    exports.directiveName = 't3Popover';

    var directiveFactory = function () {
        return [
            '$parse', '$timeout', function ($parse, $timeout) {
                var directive = {
                    name: exports.directiveName,
                    restrict: 'A',
                    link: function (scope, element, attrs) {
                        var options = {
                            content: $parse(attrs[exports.directiveName])(scope),
                            trigger: 'manual',
                            animation: typeof attrs['t3PopoverAnimation'] === 'string' ? attrs['t3PopoverAnimation'].toLowerCase() !== 'false' : true
                        };

                        var initPopup = function () {
                            var data = element.data(exports.directiveName);
                            if (data) {
                                element.popover('destroy');
                            }
                            element.popover(options);
                            element.data(exports.directiveName, true);
                        };

                        initPopup();

                        var isVisible = false;
                        scope.$watch(attrs[exports.directiveName], function (value) {
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

    exports.directive = directiveFactory();
});
