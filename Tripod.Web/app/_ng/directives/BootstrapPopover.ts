'use strict';

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
    return undefined;
}
// ReSharper restore DuplicatingLocalDeclaration

var directiveFactory = (): any[]=> {
    return ['$parse', ($parse: ng.IParseService): ng.IDirective => {
        var directive: ng.IDirective = {
            name: directiveName,
            restrict: 'A',
            link: (scope: ng.IScope, element: JQuery, attrs: ng.IAttributes) => {

                var options: PopoverOptions = {
                    content: $parse(attrs[directiveName])(scope),
                    trigger: 'manual',
                    animation: typeof attrs['t3PopoverAnimation'] === 'string'
                    ? attrs['t3PopoverAnimation'].toLowerCase() !== 'false'
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

                var isVisible = false;
                scope.$watch(attrs[directiveName], (value: string): void => {
                    if (value != options.content) {
                        options.content = value;
                        initPopup(element, options);
                        if (isVisible) element.popover('show');
                    }
                });

                scope.$watch(attrs['t3PopoverSwitch'], (value: boolean): void => {

                    if (value) {
                        isVisible = true;
                        element.popover('show');
                        element.data('t3-popover-redraw', !element.is(':visible'));
                        needsRedraw(element, !element.is(':visible'));
                    } else {
                        isVisible = false;
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

export var directive = directiveFactory();