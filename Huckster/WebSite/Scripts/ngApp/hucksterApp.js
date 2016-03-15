(function () {
    'use strict';

    angular.module('hucksterApp', ['restangular', 'ngRoute', 'module.orderForm','module.checkout'])
        .directive('convertToNumber', function() {
                return {
                    require: 'ngModel',
                    link: function(scope, element, attrs, ngModel) {
                        ngModel.$parsers.push(function(val) {
                            return parseInt(val, 10);
                        });
                        ngModel.$formatters.push(function(val) {
                            return '' + val;
                        });
                    }
                }
            })
            .run(['$rootScope', function ($rootScope) {}]);
})();