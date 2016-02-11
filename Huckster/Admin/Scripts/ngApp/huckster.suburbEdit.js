(function () {
    //var servicabilityApp = angular.module('directMoney.Servicability', ['restangular', 'ngSanitize', 'angular-loading-bar', 'ngAnimate']);

    var module = angular.module('module.suburbEdit', []);

    module.config(['RestangularProvider',
    function (RestangularProvider) {
        RestangularProvider.setBaseUrl('/api/restaurant');
    }]);

    module.controller('suburbEditController', suburbEdit);
    suburbEdit.$inject = ['$scope', 'Restangular', '$window'];


    function suburbEdit($scope, Restangular, $window) {
        $scope.deliverySuburbs = angular.fromJson($("#suburbsJSON").val());
        $scope.restaurantId = $("#restaurantId").val();

        $scope.suburbSearch = function() {
            var searchText = $scope.searchText;
            Restangular.all("suburbs").getList({ searchText: searchText }).then(function (result) {
                $scope.searchResults = result;
            }, function (error) {
                alert(error);
            });
        }

        $scope.addSuburb = function(deliverySuburb) {
            $scope.deliverySuburbs.push(deliverySuburb);
        };

        $scope.removeSuburb = function (index) {
            $scope.deliverySuburbs.splice(index, 1);
        };

        $scope.updateSuburb = function () {
            Restangular.all($scope.restaurantId + '/update-suburbs').post({ suburbs: $scope.deliverySuburbs }).then(function (result) {
                alert("suburbs saved");
            }, function (error) {
                alert(error);
            });
        };
    };

}).call(this);