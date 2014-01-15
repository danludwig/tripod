'use strict';

module App.Directives.RemoveCssClass {

    export var directiveName = 'removeClass';

    var directiveFactory = (): () => ng.IDirective => {
        return (): ng.IDirective => {
            var directive: ng.IDirective = {
                restrict: 'A',
                link: (scope: ng.IScope, element: JQuery, attr: ng.IAttributes) => {
                    element.removeClass(attr[directiveName]);
                }
            };
            return directive;
        };
    };

    export var directive = directiveFactory();
}
