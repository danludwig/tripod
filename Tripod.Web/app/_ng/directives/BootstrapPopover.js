'use strict';
define(["require", "exports"], function(require, exports) {
    exports.directiveName = 't3Popover';

    function t3Popover() {
        var directive = {
            restrict: 'A',
            link: function (scope, element, attrs) {
                scope.$watch(attrs.t3PopoverSwitch, function (value) {
                    // has the popover been initialized?
                    var data = element.data('t3-popover');
                    if (!data) {
                        var animation = typeof attrs.t3PopoverAnimation == 'string' ? attrs.t3PopoverAnimation.toLowerCase() !== 'false' : true;
                        element.popover({
                            content: attrs.t3Popover,
                            trigger: 'manual',
                            animation: animation
                        });
                        element.data('t3-popover', true);
                        $(window).on('resize', function (e) {
                            // will be incorrectly positioned if triggered when hidden
                            if (element.data('t3-popover-redraw')) {
                                element.popover('hide');
                                element.popover('show');
                            }
                        });
                    }

                    if (value) {
                        element.popover('show');
                        element.data('t3-popover-redraw', !element.is(':visible'));
                    } else {
                        element.popover('hide');
                        element.data('t3-popover-redraw', false);
                    }
                });
            }
        };
        return directive;
    }
    exports.t3Popover = t3Popover;
});
