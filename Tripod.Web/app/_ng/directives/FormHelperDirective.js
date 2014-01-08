'use strict';
define(["require", "exports"], function(require, exports) {
    exports.directiveName = 'formHelper';

    var FormHelperController = (function () {
        function FormHelperController() {
            // tells us whether or not a form submit has been attempted
            this.submitAttempted = false;
        }
        return FormHelperController;
    })();
    exports.FormHelperController = FormHelperController;

    //#region Directive
    var directiveFactory = function () {
        // inject parse service
        return [
            '$parse', function ($parse) {
                var directive = {
                    name: exports.directiveName,
                    restrict: 'A',
                    require: [exports.directiveName, 'form'],
                    controller: [FormHelperController],
                    compile: function () {
                        return {
                            pre: function (scope, element, attr, ctrls) {
                                // get the required controllers based on directive order
                                var helpCtrl = ctrls[0];
                                var formCtrl = ctrls[1];

                                // give the helper controller access to the form controller
                                helpCtrl.formController = formCtrl;

                                // initialize the form submission
                                if (attr['submit-attempted'])
                                    helpCtrl.submitAttempted = true;

                                // put the helper controller on the scope
                                var alias = $.trim(attr[exports.directiveName]);
                                if (alias)
                                    scope[alias] = helpCtrl;
                            },
                            post: function (scope, element, attr, ctrls) {
                                // get the required controllers based on directive order
                                var helpCtrl = ctrls[0];
                                var formCtrl = ctrls[1];

                                var fn = $parse(attr[exports.directiveName]);

                                element.bind('submit', function () {
                                    // record the fact that a form submission was attempted
                                    helpCtrl.submitAttempted = true;
                                    if (!scope.$$phase)
                                        scope.$apply();

                                    // prevent the form from being submitted if not valid
                                    if (!formCtrl.$valid)
                                        return false;

                                    scope.$apply(function () {
                                        fn(scope, { $event: event });
                                    });
                                    return true;
                                });
                            }
                        };
                    }
                };
                return directive;
            }];
    };

    //#endregion
    exports.directive = directiveFactory();
});
