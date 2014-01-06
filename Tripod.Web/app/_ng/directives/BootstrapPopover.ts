'use strict';

interface IPopoverAttributes extends ng.IAttributes {
    t3Popover?: string;
    t3PopoverSwitch?: string;
    t3PopoverAnimation?: string;
}

export var directiveName = 't3Popover';

export function t3Popover(): ng.IDirective {
    var directive: ng.IDirective = {
        restrict: 'A',
        link: (scope: ng.IScope, element: JQuery, attrs: IPopoverAttributes) => {
            scope.$watch(attrs.t3PopoverSwitch, (value: boolean): void => {

                // has the popover been initialized?
                var data = element.data('t3-popover');
                if (!data) {
                    var animation = typeof attrs.t3PopoverAnimation == 'string' ? attrs.t3PopoverAnimation.toLowerCase() !== 'false' : true;
                    element.popover({
                        content: attrs.t3Popover,
                        trigger: 'manual',
                        animation: animation,
                    });
                    element.data('t3-popover', true);
                    $(window).on('resize', (e: JQueryEventObject): void => {
                        // will be incorrectly positioned if triggered when hidden
                        if (element.data('t3-popover-redraw')) {
                            element.popover('hide');
                            element.popover('show');
                        }
                    })
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
