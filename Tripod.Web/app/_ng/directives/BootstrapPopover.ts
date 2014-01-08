'use strict';

interface IPopoverAttributes extends ng.IAttributes {
    t3Popover?: string;
    t3PopoverSwitch?: string;
    t3PopoverAnimation?: string;
}

export var directiveName = 't3Popover';

function initPopup(element: JQuery, options: PopoverOptions): void {
    var data = element.data(directiveName);
    if (data) {
        element.popover('destroy');
    }
    element.popover(options);
    element.data(directiveName, true);
}

// ReSharper disable DuplicatingLocalDeclaration
function needsRedraw(element: JQuery): boolean;
function needsRedraw(element: JQuery, value: boolean): void;
function needsRedraw(element: JQuery, value?: boolean): any {
    var dataKey = directiveName + 'Redraw';
    if (arguments.length === 1) {
        return element.data(dataKey) || false;
    }
    element.data(dataKey, value);
}
// ReSharper restore DuplicatingLocalDeclaration

export function t3Popover($parse: ng.IParseService): ng.IDirective {
    var directive: ng.IDirective = {
        restrict: 'A',
        link: (scope: ng.IScope, element: JQuery, attrs: IPopoverAttributes) => {

            var options: PopoverOptions = {
                content: $parse(attrs.t3Popover)(scope),
                trigger: 'manual',
                animation: typeof attrs.t3PopoverAnimation == 'string'
                    ? attrs.t3PopoverAnimation.toLowerCase() !== 'false'
                    : true,
            };
            initPopup(element, options);

            $(window).on('resize', (): void => {
                // will be incorrectly positioned if triggered when hidden
                if (needsRedraw(element)) {
                    element.popover('hide');
                    element.popover('show');
                }
            });

            //var content = $parse(attrs.t3Popover);
            //var content2 = content(scope);
            scope.$watch(attrs[directiveName], (value: string): void => {
                if (value != options.content) {
                    options.content = value;
                    initPopup(element, options);
                }
            });

            scope.$watch(attrs.t3PopoverSwitch, (value: boolean): void => {

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

t3Popover.$inject = ['$parse'];

