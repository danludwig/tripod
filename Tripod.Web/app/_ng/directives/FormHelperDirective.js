'use strict';
define(["require", "exports"], function(require, exports) {
    exports.directiveName = 'formHelper';

    var FormHelperController = (function () {
        function FormHelperController() {
            this.submitAttempted = false;
            this.isSubmitDisabled = false;
        }
        return FormHelperController;
    })();
    exports.FormHelperController = FormHelperController;

    var directiveFactory = function () {
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
                                var helpCtrl = ctrls[0];
                                var formCtrl = ctrls[1];

                                helpCtrl.formController = formCtrl;

                                if (attr['submitAttempted'])
                                    helpCtrl.submitAttempted = true;

                                var alias = $.trim(attr[exports.directiveName]);
                                if (alias)
                                    scope[alias] = helpCtrl;
                            },
                            post: function (scope, element, attr, ctrls) {
                                var helpCtrl = ctrls[0];
                                var formCtrl = ctrls[1];

                                element.bind('submit', function () {
                                    helpCtrl.submitAttempted = true;
                                    if (formCtrl.$valid)
                                        helpCtrl.isSubmitDisabled = true;
                                    if (!scope.$$phase)
                                        scope.$apply();

                                    if (!formCtrl.$valid)
                                        return false;

                                    return true;
                                });
                            }
                        };
                    }
                };
                return directive;
            }];
    };

    exports.directive = directiveFactory();
});
