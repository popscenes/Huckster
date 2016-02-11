(function () {
    'use strict';

    angular.module('hucksterApp', ['restangular', 'ngRoute', 'module.orderForm','module.checkout'])
        .run(['$rootScope', function ($rootScope) {}]);
})();