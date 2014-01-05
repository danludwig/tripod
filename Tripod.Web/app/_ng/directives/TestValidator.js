'use strict';
define(["require", "exports"], function(require, exports) {
    function ngTripodValidateEmailAddress1($http) {
        return {
            restrict: 'A',
            require: 'ngModel',
            //scope: {
            //    ngTripodValidateEmailAddress: '&',
            //},
            link: function ($scope, element, attributes, fieldController) {
                element.on('blur', function (evt) {
                    $scope.$apply(function () {
                        var directive = attributes.ngTripodValidateEmailAddress;

                        //var directive: any = $scope['ngTripodValidateEmailAddress']();
                        var settings = typeof directive !== 'string' ? directive : {
                            method: 'POST',
                            url: directive
                        };
                        settings.method = settings.method || 'POST';
                        var defaultData = {};
                        if (element.attr('name'))
                            defaultData[element.attr('name')] = fieldController.$viewValue;
                        settings.data = settings.data || {};
                        settings.data = $.extend({}, defaultData, settings.data);

                        $http(settings).success(function (data, status, headers, config) {
                            //alert('success');
                            //if (data == true || status == 200) {
                            fieldController.$setValidity('server', true);

                            //}
                            //else if (data) {
                            //    var serverErrors = $scope['serverErrors'] || {};
                            //    serverErrors[element.attr('name')] = data;
                            //}
                            alert('validnow');
                        }).error(function (data, status, headers, config) {
                            //alert('error');
                            fieldController.$setValidity('server', false);
                            var serverErrors = $scope['serverErrors'] || {};
                            serverErrors[element.attr('name')] = data;
                            $scope['serverErrors'] = serverErrors;
                        });
                    });
                });
            }
        };
    }
    exports.ngTripodValidateEmailAddress1 = ngTripodValidateEmailAddress1;

    exports.ngTripodValidateEmailAddress1.$inject = ['$http'];

    function ngTripodValidateEmailAddress($http) {
        return {
            restrict: 'A',
            require: 'ngModel',
            //scope: {
            //    ngTripodValidateEmailAddress: '&',
            //},
            link: function ($scope, element, attributes, fieldController) {
                fieldController.$parsers.unshift(function (viewValue) {
                    //if (INTEGER_REGEXP.test(viewValue)) {
                    //    // it is valid
                    //    ctrl.$setValidity('server', true);
                    //    return viewValue;
                    //} else {
                    //    // it is invalid, return undefined (no model update)
                    fieldController.$setValidity('server', false);
                    return undefined;
                    //}
                });
            }
        };
    }
    exports.ngTripodValidateEmailAddress = ngTripodValidateEmailAddress;

    exports.ngTripodValidateEmailAddress.$inject = ['$http'];
});
