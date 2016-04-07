(function () {
    

    var module = angular.module('module.ordersManager', []);

    module.config(['RestangularProvider',
    function (RestangularProvider) {
        RestangularProvider.setBaseUrl('/api/orders');
    }]);

    module.controller('ordersManagerController', ordersManager);
    ordersManager.$inject = ['$scope', 'Restangular', '$window'];


    function ordersManager($scope, Restangular, $window) {
        $scope.test = "test";
        $scope.orders = [];

        $scope.getOrders = function () {
            Restangular.all('').getList({ status: 'PaymentSucccessful' }).then(function (result) {
                $scope.orders = result;
            }, function (error) {
                alert(error);
            });
        }

        $scope.getOrders();
    };

}).call(this);