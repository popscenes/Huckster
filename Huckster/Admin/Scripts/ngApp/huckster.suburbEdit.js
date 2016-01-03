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
                alert(result);
            }, function (error) {
                alert(error);
            });
        }

    };

}).call(this);