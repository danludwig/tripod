'use strict';
var App;
(function (App) {
    (function (Directives) {
        (function (RemoveCssClass) {
            RemoveCssClass.directiveName = 'removeClass';

            var directiveFactory = function () {
                return function () {
                    var d = {
                        restrict: 'A',
                        link: function (scope, element, attr) {
                            element.removeClass(attr[RemoveCssClass.directiveName]);
                        }
                    };
                    return d;
                };
            };

            RemoveCssClass.directive = directiveFactory();
        })(Directives.RemoveCssClass || (Directives.RemoveCssClass = {}));
        var RemoveCssClass = Directives.RemoveCssClass;
    })(App.Directives || (App.Directives = {}));
    var Directives = App.Directives;
})(App || (App = {}));
