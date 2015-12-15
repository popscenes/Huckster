(function () {
    'use strict';

    angular.module('hucksterAdminApp', ['restangular', 'ngRoute', 'dndLists', 'module.menuEdit'])
        .run(['$rootScope', function ($rootScope) {}]);
})();