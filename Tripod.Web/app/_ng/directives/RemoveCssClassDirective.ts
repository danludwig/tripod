'use strict';

module App.Directives.RemoveCssClass {

    export var directiveName = 'removeClass';

    var directiveFactory = (): () => ng.IDirective => {
        return (): ng.IDirective => {
            var directive: ng.IDirective = {
                restrict: 'A',
                link: (scope: ng.IScope, element: JQuery, attrs: ng.IAttributes) => {
                    element.removeClass(attrs[directiveName]);
                }
            };
            return directive;
        };
    };

    export var directive = directiveFactory();
}
