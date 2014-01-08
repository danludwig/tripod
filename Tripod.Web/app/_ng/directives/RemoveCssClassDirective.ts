'use strict';

interface IRemoveClassAttributes extends ng.IAttributes {
    ngT3RemoveClass?: string;
}

export var directiveName = 'ngT3RemoveClass';

export function ngT3RemoveClass(): ng.IDirective {
    var directive: ng.IDirective = {
        restrict: 'A',
        link: (scope: ng.IScope, element: JQuery, attrs: IRemoveClassAttributes) => {
            element.removeClass(attrs.ngT3RemoveClass);
        }
    };
    return directive;
}

