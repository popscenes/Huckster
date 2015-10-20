(function () {
    //var servicabilityApp = angular.module('directMoney.Servicability', ['restangular', 'ngSanitize', 'angular-loading-bar', 'ngAnimate']);




    var module = angular.module('module.orderForm', []);

    module.config(['RestangularProvider',
    function (RestangularProvider) {
        RestangularProvider.setBaseUrl('/api/Order');
    }]);

    module.controller('OrderFormController', OrderForm);
    OrderForm.$inject = ['$scope', 'Restangular', '$window'];


    function OrderForm($scope, Restangular, $window) {
        $scope.test = "testy";
        $scope.OrderItems = [];
        $scope.order = {
            CustomerMobile: '',
            CustomerEmail: '',
            RestaurantId: '8DEE7B67-E126-4AFC-82F3-918E038AB3A3'
        };

        $scope.addmenuItem = function(name, price, quantity) {
            var found = false;
            for (var index = 0; index < $scope.OrderItems.length; index++) {
                if ($scope.OrderItems[index].Name === name) {
                    $scope.OrderItems[index].Quantity++;
                    found = true;
                    break;
                }
            }
            if(!found)
                $scope.OrderItems.push({ Name: name, Price: price, Quantity: quantity });
        }

        $scope.placeOrder = function() {
            Restangular.all('PlaceOrder').post({order : $scope.order, orderItems: $scope.OrderItems }).then(function (result) {
                window.location = "/order/customerdetails/" + result;
            });
        }
    };

}).call(this);