'use strict';
define(["require", "exports"], function(require, exports) {
    exports.directiveName = 'ngT3RemoveClass';

    function ngT3RemoveClass() {
        var directive = {
            restrict: 'A',
            link: function (scope, element, attrs) {
                element.removeClass(attrs.ngT3RemoveClass);
            }
        };
        return directive;
    }
    exports.ngT3RemoveClass = ngT3RemoveClass;
});
