(function () {
    //var servicabilityApp = angular.module('directMoney.Servicability', ['restangular', 'ngSanitize', 'angular-loading-bar', 'ngAnimate']);

    var module = angular.module('module.menuEdit', []);

    module.config(['RestangularProvider',
    function (RestangularProvider) {
        RestangularProvider.setBaseUrl('/api/Restaurant');
    }]);

    module.controller('menuEditController', menuEdit);
    menuEdit.$inject = ['$scope', 'Restangular', '$window'];


    function menuEdit($scope, Restangular, $window) {
        $scope.menus = angular.fromJson($("#menuJSON").val());
    };

}).call(this);