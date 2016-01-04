(function () {
    //var servicabilityApp = angular.module('directMoney.Servicability', ['restangular', 'ngSanitize', 'angular-loading-bar', 'ngAnimate']);

    var module = angular.module('module.deliveryHoursEdit', []);

    module.config(['RestangularProvider',
    function (RestangularProvider) {
        RestangularProvider.setBaseUrl('/api/restaurant');
    }]);

    module.controller('deliveryHoursEditController', deliveryHoursEdit);
    deliveryHoursEdit.$inject = ['$scope', 'Restangular', '$window'];


    function deliveryHoursEdit($scope, Restangular, $window) {
        $scope.deliveryHours = angular.fromJson($("#suburbsJSON").val());
        $scope.restaurantId = $("#restaurantId").val();

    };

}).call(this);