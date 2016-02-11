(function () {
    'use strict';

    angular.module('hucksterAdminApp', ['restangular', 'ngRoute','ui.bootstrap', 'dndLists', 'module.menuEdit', 'module.suburbEdit', 'module.deliveryHoursEdit'])
        .directive('convertToNumber', function () {
        return {
            require: 'ngModel',
            link: function (scope, element, attrs, ngModel) {
                ngModel.$parsers.push(function (val) {
                    return parseInt(val, 10);
                });
                ngModel.$formatters.push(function (val) {
                    return '' + val;
                });
            }
        };
    }).run(['$rootScope', function ($rootScope) {}]);
})();