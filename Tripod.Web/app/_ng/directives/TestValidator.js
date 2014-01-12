'use strict';
define(["require", "exports"], function(require, exports) {
    function ngTripodValidateEmailAddress1($http) {
        return {
            restrict: 'A',
            require: 'ngModel',
            link: function ($scope, element, attributes, fieldController) {
                element.on('blur', function (evt) {
                    $scope.$apply(function () {
                        var directive = attributes.ngTripodValidateEmailAddress;

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
                            fieldController.$setValidity('server', true);

                            alert('validnow');
                        }).error(function (data, status, headers, config) {
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
            link: function ($scope, element, attributes, fieldController) {
                fieldController.$parsers.unshift(function (viewValue) {
                    fieldController.$setValidity('server', false);
                    return undefined;
                });
            }
        };
    }
    exports.ngTripodValidateEmailAddress = ngTripodValidateEmailAddress;

    exports.ngTripodValidateEmailAddress.$inject = ['$http'];
});
