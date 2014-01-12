'use strict';
var App;
(function (App) {
    (function (Directives) {
        (function (RemoveCssClass) {
            RemoveCssClass.directiveName = 'removeClass';

            var directiveFactory = function () {
                return function () {
                    var directive = {
                        restrict: 'A',
                        link: function (scope, element, attrs) {
                            element.removeClass(attrs[RemoveCssClass.directiveName]);
                        }
                    };
                    return directive;
                };
            };

            RemoveCssClass.directive = directiveFactory();
        })(Directives.RemoveCssClass || (Directives.RemoveCssClass = {}));
        var RemoveCssClass = Directives.RemoveCssClass;
    })(App.Directives || (App.Directives = {}));
    var Directives = App.Directives;
})(App || (App = {}));
