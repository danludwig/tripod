'use strict';
define(["require", "exports"], function(require, exports) {
    function ngTripodValidateEmailAddress($http) {
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
    exports.ngTripodValidateEmailAddress = ngTripodValidateEmailAddress;

    exports.ngTripodValidateEmailAddress.$inject = ['$http'];
});
