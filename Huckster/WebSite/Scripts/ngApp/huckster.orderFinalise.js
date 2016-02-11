(function () {
    //var servicabilityApp = angular.module('directMoney.Servicability', ['restangular', 'ngSanitize', 'angular-loading-bar', 'ngAnimate']);




    var module = angular.module('module.orderForm');

    module.config(['RestangularProvider',
    function (RestangularProvider) {
        RestangularProvider.setBaseUrl('/api/Order');
    }]);

    module.controller('OrderFinaliseController', OrderFinalise);
    OrderFinalise.$inject = ['$scope', 'Restangular'];


    function OrderFinalise($scope, Restangular) {
        $scope.getToken = function() {
            Stripe.card.createToken({
                number: '4242424242424242',
                cvc: '111',
                exp_month: 01,
                exp_year: 16
            }, $scope.stripeResponseHandler);
        }

        $scope.paypalPayment = function() {
            Restangular.all('Payment/paypal-redirect').post({ OrderId: $scope.orderId }).then(function (result) {
                window.location = result;
            });
        };

        $scope.stripeResponseHandler = function(status, response) {
            if (response.error) {
                alert(response.error.message);
            } else {

                var token = response.id;
                Restangular.all('Payment/Stripe').post({ PaymentToken: token, OrderId: $scope.orderId }).then(function (result) {
                    window.location = "/order/Complete/" + result;
                });
            }
        }

        Stripe.setPublishableKey('pk_test_e9OAHJzcasWjjnNbMNNQlMlL');
    };
}).call(this);