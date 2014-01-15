'use strict';

module App.Directives.TooltipToggle {

    export var directiveName = 'tooltip';

    export var directiveConfig = ['$tooltipProvider', ($tooltipProvider: ng.ui.bootstrap.ITooltipProvider): void => {
        $tooltipProvider.setTriggers({
            'show-tooltip': 'hide-tooltip'
        });
    }];

    var directiveFactory = (): any[] => {
        return ['$timeout', ($timeout: ng.ITimeoutService): ng.IDirective => {
            var directive: ng.IDirective = {
                name: directiveName,
                restrict: 'A',
                link: (scope: ng.IScope, element: JQuery, attr: ng.IAttributes) => {

                    attr[directiveName + 'Trigger'] = 'show-tooltip';

                    var redrawPromise: ng.IPromise<void>;
                    $(window).on('resize', (): void => {
                        if (redrawPromise) $timeout.cancel(redrawPromise);
                        redrawPromise = $timeout((): void => {
                            if (!scope['tt_isOpen']) return;
                            element.triggerHandler('hide-tooltip');
                            element.triggerHandler('show-tooltip');

                        }, 100);
                    });

                    scope.$watch(attr[directiveName + 'Toggle'], (value: boolean): void => {
                        if (value && !scope['tt_isOpen']) {
                            // tooltip provider will call scope.$apply, so need to get out of this digest cycle first
                            $timeout((): void => {
                                element.triggerHandler('show-tooltip');
                            });
                        }
                        else if (!value && scope['tt_isOpen']) {
                            $timeout((): void => {
                                element.triggerHandler('hide-tooltip');
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

