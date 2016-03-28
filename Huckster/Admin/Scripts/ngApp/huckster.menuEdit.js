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

        $scope.removeMenuItem = function (index) {
            $scope.currentMenu.MenuItems.splice(index, 1);
        };

        $scope.addMenu = function() {
            var newMenu = { Title: "New Menu", Description: "", Order: "", Id: "", ParentAggregateId: $scope.restaurantId, MenuItems: [] }
            $scope.menus.push(newMenu);
            $scope.currentMenu = newMenu;
        }

        $scope.deleteMenu = function (menuToDelte) {
            menuToDelte.Deleted = true;
        }

        $scope.unDeleteMenu = function (menuToDelte) {
            menuToDelte.Deleted = false;
        }

        $scope.addmenuItem = function() {
            $scope.currentMenu.MenuItems.push({Id: "", MenuId:"", Name:"", MenuGroup:"", Description:"", Price:"", Order:""});
        }
    };

}).call(this);