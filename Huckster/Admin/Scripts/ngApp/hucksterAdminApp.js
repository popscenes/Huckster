(function () {
    'use strict';

    angular.module('hucksterAdminApp', ['restangular', 'ngRoute', 'dndLists', 'module.menuEdit', 'module.suburbEdit'])
        .run(['$rootScope', function ($rootScope) {}]);
})();