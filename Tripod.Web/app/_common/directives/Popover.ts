'use strict';

module App.Directives.Popover {

    export var directiveName = 't3Popover';

    var directiveFactory = (): any[]=> {
        return ['$parse', '$timeout', ($parse: ng.IParseService, $timeout: ng.ITimeoutService): ng.IDirective => {
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

                    var initPopup = (): void => {
                        var data = element.data(directiveName);
                        if (data) {
                            element.popover('destroy');
                        }
                        element.popover(options);
                        element.data(directiveName, true);
                    };

                    initPopup();

                    var isVisible = false;
                    scope.$watch(attrs[directiveName], (value: string): void => {
                        if (value != options.content) {
                            options.content = value;
                            initPopup();
                            if (isVisible) element.popover('show');
                        }
                    });

                    var redrawPromise: ng.IPromise<void>;
                    $(window).on('resize', (): void => {
                        if (redrawPromise) $timeout.cancel(redrawPromise);
                        redrawPromise = $timeout((): void => {
                            if (!isVisible) return;
                            element.popover('hide');
                            element.popover('show');

                        }, 100);
                    });

                    scope.$watch(attrs['t3PopoverSwitch'], (value: boolean): void => {

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

    export var directive = directiveFactory();
}

