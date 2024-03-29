﻿(function () {
    //var servicabilityApp = angular.module('directMoney.Servicability', ['restangular', 'ngSanitize', 'angular-loading-bar', 'ngAnimate']);




    var module = angular.module('module.orderForm', []);

    module.config(['RestangularProvider',
    function (RestangularProvider) {
        RestangularProvider.setBaseUrl('/api/Order');
    }]);

    module.controller('OrderFormController', OrderForm);
    OrderForm.$inject = ['$scope', 'Restangular', '$window'];


    function OrderForm($scope, Restangular, $window) {
        $scope.deliveryFee = $("#deliveryFee").val();
        $scope.minimumOrder = $("#minimumOrder").val();
        $scope.restaurantId = $("#restaurantId").val();

        $scope.orderLoading = false;
        $scope.serverError = false;

        

        $scope.OrderItems = [];
        $scope.order = {
            CustomerMobile: '',
            CustomerEmail: '',
            RestaurantId: $scope.restaurantId,
            DeliveryTime: '',
            DeliverySuburbId: ''
        };

        if ($("#orderData").val() !== "") {
            $scope.orderData = angular.fromJson($("#orderData").val());
            $scope.OrderItems = $scope.orderData.Order.OrderItems;
            $scope.order = $scope.orderData.Order;
        }
        

        $scope.removeMenuItemQuantity = function(index) {
            $scope.OrderItems[index].Quantity--;
            if ($scope.OrderItems[index].Quantity <= 0) {
                $scope.OrderItems.splice(index, 1);
            }
        }

        $scope.MinimumOrderMet = function() {
            return $scope.subTotal() >= $scope.minimumOrder;
        }
        $scope.addMenuItemQuantity = function(index) {
            $scope.OrderItems[index].Quantity++;
        }
        $scope.deleteMenuItem = function (index) {
            $scope.OrderItems.splice(index, 1);
        }

        $scope.subTotal = function() {
            var result = 0;
            for (var index = 0; index < $scope.OrderItems.length; index++) {
                result += $scope.OrderItems[index].Quantity * $scope.OrderItems[index].Price;
            }
            return result;
        };

        $scope.total = function () {
            return $scope.subTotal() + parseInt($scope.deliveryFee);
        };

        $scope.addmenuItem = function (id, name, price, quantity) {
            var found = false;
            for (var index = 0; index < $scope.OrderItems.length; index++) {
                if ($scope.OrderItems[index].Name === name) {
                    $scope.OrderItems[index].Quantity++;
                    found = true;
                    break;
                }
            }
            if(!found)
                $scope.OrderItems.push({ MenuItemKey: id, Name: name, Price: price, Quantity: quantity });
        }

        $scope.placeOrder = function() {
            $scope.orderLoading = true;
            if ($scope.order.Id == undefined) {
                Restangular.all('PlaceOrder').post({ order: $scope.order, orderItems: $scope.OrderItems }).then(function(result) {
                        window.location = "/order/checkout/" + result;
                    },
                    function(error) {
                        $scope.orderLoading = false;
                        $scope.serverError = true;
                    });
            } else {
                Restangular.all('UpdateOrder').post({ order: $scope.order, orderItems: $scope.OrderItems }).then(function (result) {
                    window.location = "/order/checkout/" + result;
                },
                    function (error) {
                        $scope.orderLoading = false;
                        $scope.serverError = true;
                    });
            }
        }
    };

}).call(this);