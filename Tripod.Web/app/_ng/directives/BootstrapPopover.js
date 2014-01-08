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
    }

    // ReSharper restore DuplicatingLocalDeclaration
    function t3Popover($parse) {
        var directive = {
            restrict: 'A',
            link: function (scope, element, attrs) {
                var options = {
                    content: $parse(attrs.t3Popover)(scope),
                    trigger: 'manual',
                    animation: typeof attrs.t3PopoverAnimation == 'string' ? attrs.t3PopoverAnimation.toLowerCase() !== 'false' : true
                };
                initPopup(element, options);

                $(window).on('resize', function () {
                    // will be incorrectly positioned if triggered when hidden
                    if (needsRedraw(element)) {
                        element.popover('hide');
                        element.popover('show');
                    }
                });

                //var content = $parse(attrs.t3Popover);
                //var content2 = content(scope);
                scope.$watch(attrs[exports.directiveName], function (value) {
                    if (value != options.content) {
                        options.content = value;
                        initPopup(element, options);
                    }
                });

                scope.$watch(attrs.t3PopoverSwitch, function (value) {
                    //// has the popover been initialized?
                    //var data = element.data('t3-popover');
                    //if (!data) {
                    //    var animation = typeof attrs.t3PopoverAnimation == 'string' ? attrs.t3PopoverAnimation.toLowerCase() !== 'false' : true;
                    //    element.popover({
                    //        content: content2 || 'default content',
                    //        trigger: 'manual',
                    //        animation: animation,
                    //    });
                    //    element.data('t3-popover', true);
                    //    $(window).on('resize', (e: JQueryEventObject): void => {
                    //        // will be incorrectly positioned if triggered when hidden
                    //        if (element.data('t3-popover-redraw')) {
                    //            element.popover('hide');
                    //            element.popover('show');
                    //        }
                    //    });
                    //}
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
    }
    exports.t3Popover = t3Popover;

    exports.t3Popover.$inject = ['$parse'];
});
