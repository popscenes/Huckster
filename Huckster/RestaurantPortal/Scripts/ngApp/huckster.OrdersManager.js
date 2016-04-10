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
        $scope.currentOrder = {};
        $scope.pickUpTime = null;

        $scope.getOrders = function (status) {
            Restangular.all('').getList({ orderStatus: status }).then(function (result) {
                $scope.orders = result;
            }, function (error) {
                alert(error);
            });
        }

        $scope.orderDetails = function(order)
        {
            $scope.currentOrder = order;
            $scope.pickUpTime = new Date();
            var timeSpan = order.Order.DeliveryTime.split("T")[1];
            var timeArray = timeSpan.split(":");
            $scope.pickUpTime.setHours(parseInt(timeArray[0]));
            $scope.pickUpTime.setMinutes(parseInt(timeArray[1]));
            $scope.pickUpTime.setSeconds(0);

            $scope.pickUpTime = new Date($scope.pickUpTime - 20 * 60000);

            
            $('#orderDetailsModal').modal('toggle')
        }

        $scope.getOrders('PaymentSucccessful');
    };

}).call(this);