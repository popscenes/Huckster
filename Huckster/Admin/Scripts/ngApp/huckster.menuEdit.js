(function () {
    //var servicabilityApp = angular.module('directMoney.Servicability', ['restangular', 'ngSanitize', 'angular-loading-bar', 'ngAnimate']);

    var module = angular.module('module.menuEdit', []);

    module.config(['RestangularProvider',
    function (RestangularProvider) {
        RestangularProvider.setBaseUrl('/api/restaurant');
    }]);

    module.controller('menuEditController', menuEdit);
    menuEdit.$inject = ['$scope', 'Restangular', '$window'];


    function menuEdit($scope, Restangular, $window) {
        $scope.menus = angular.fromJson($("#menuJSON").val());
        $scope.restaurantId = $("#restaurantId").val();

        $scope.currentMenu = $scope.menus[0];

        $scope.saveMenus = function () {
            Restangular.all($scope.restaurantId + '/update-menu').post({ menus: $scope.menus }).then(function (result) {
                alert("menu saved");
            }, function(error) {
                alert(error);
            });
        }
    };

}).call(this);