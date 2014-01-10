'use strict';
define(["require", "exports"], function(require, exports) {
    exports.directiveName = 't3Popover';

    function initPopup(element, options) {
        var data = element.data(exports.directiveName);
        if (data) {
            element.popover('destroy');
        }
        element.popover(options);
        element.data(exports.directiveName, true);
    }

    

    function needsRedraw(element, value) {
        var dataKey = exports.directiveName + 'Redraw';
        if (arguments.length === 1) {
            return element.data(dataKey) || false;
        }
        element.data(dataKey, value);
        return undefined;
    }

    // ReSharper restore DuplicatingLocalDeclaration
    var directiveFactory = function () {
        return [
            '$parse', function ($parse) {
                var directive = {
                    name: exports.directiveName,
                    scope: true,
                    restrict: 'A',
                    link: function (scope, element, attrs) {
                        var options = {
                            content: $parse(attrs[exports.directiveName])(scope),
                            trigger: 'manual',
                            animation: typeof attrs['t3PopoverAnimation'] === 'string' ? attrs['t3PopoverAnimation'].toLowerCase() !== 'false' : true
                        };
                        initPopup(element, options);

                        $(window).on('resize', function () {
                            // will be incorrectly positioned if triggered when hidden
                            if (needsRedraw(element)) {
                                element.popover('hide');
                                element.popover('show');
                            }
                        });

                        scope.$watch(attrs[exports.directiveName], function (value) {
                            if (value != options.content) {
                                options.content = value;
                                initPopup(element, options);
                            }
                        });

                        scope.$watch(attrs['t3PopoverSwitch'], function (value) {
                            if (value) {
                                element.popover('show');
                                element.data('t3-popover-redraw', !element.is(':visible'));
                                needsRedraw(element, !element.is(':visible'));
                            } else {
                                element.popover('hide');
                                element.data('t3-popover-redraw', false);
                                needsRedraw(element, false);
                            }
                        });
                    }
                };
                return directive;
            }];
    };

    exports.directive = directiveFactory();
});
