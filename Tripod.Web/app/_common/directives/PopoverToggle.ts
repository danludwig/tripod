'use strict';

module App.Directives.PopoverToggle {

    export var directiveName = 'popover';

    export var directiveConfig = ['$tooltipProvider', ($tooltipProvider: ng.ui.bootstrap.ITooltipProvider): void => {
        $tooltipProvider.setTriggers({
            'show-popover': 'hide-popover'
        });
    }];

    var directiveFactory = (): any[] => {
        return ['$timeout', ($timeout: ng.ITimeoutService): ng.IDirective => {
            var directive: ng.IDirective = {
                name: directiveName,
                restrict: 'A',
                link: (scope: ng.IScope, element: JQuery, attr: ng.IAttributes) => {

                    attr[directiveName + 'Trigger'] = 'show-popover';

                    var redrawPromise: ng.IPromise<void>;
                    $(window).on('resize', (): void => {
                        if (redrawPromise) $timeout.cancel(redrawPromise);
                        redrawPromise = $timeout((): void => {
                            if (!scope['tt_isOpen']) return;
                            element.triggerHandler('hide-popover');
                            element.triggerHandler('show-popover');

                        }, 100);
                    });

                    scope.$watch(attr[directiveName + 'Toggle'], (value: boolean): void => {
                        if (value && !scope['tt_isOpen']) {
                            // tooltip provider will call scope.$apply, so need to get out of this digest cycle first
                            $timeout((): void => {
                                element.triggerHandler('show-popover');
                            });
                        }
                        else if (!value && scope['tt_isOpen']) {
                            $timeout((): void => {
                                element.triggerHandler('hide-popover');
                            });
                        }
                    });
                }
            };
            return directive;
        }];
    };

    export var directive = directiveFactory();
}

