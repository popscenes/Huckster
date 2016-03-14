(function () {
    //var servicabilityApp = angular.module('directMoney.Servicability', ['restangular', 'ngSanitize', 'angular-loading-bar', 'ngAnimate']);


    //jQuery.noConflict();

    var module = angular.module('module.checkout', []);

    module.config(['RestangularProvider',
    function (RestangularProvider) {
        RestangularProvider.setBaseUrl('/api/Order');
    }]);

    module.controller('CheckoutController', Checkout);
    Checkout.$inject = ['$scope', 'Restangular', '$window'];


    function Checkout($scope, Restangular, $window) {
        $scope.orderData = angular.fromJson($("#orderData").val());

        $scope.paymentPaypal = false;
        $scope.paymentCC = false;
        $scope.deliveryFee = 5;

        $scope.deliveryDetailsSubmitted = false;
        $scope.personalDetailsSubmitted = false;
        $scope.paymentDetailsSubmitted = false;
        $scope.orderPaypalLoader = false;

        $scope.orderDeliveryLoader = false;
        $scope.orderDetailsLoader = false;
        $scope.orderCCLoader = false;

        //$scope.showPersonalDetails = false;
        //$scope.showPaymentDetails = false;

        Stripe.setPublishableKey('pk_test_e9OAHJzcasWjjnNbMNNQlMlL');

        $scope.showPayPal = function() {
            $scope.paymentPaypal = true;
            $scope.paymentCC = false;
        };

        $scope.showCC = function () {
            $scope.paymentPaypal = false;
            $scope.paymentCC = true;
        };

        $scope.deliveryDetails = {
            OrderId: $scope.orderData.Order.AggregateRootId,
            Street: '',
            Number: '',
            Suburb: $scope.orderData.DeliverySuburb.Suburb,
            State: $scope.orderData.DeliverySuburb.State,
            Postcode: $scope.orderData.DeliverySuburb.Postcode
        };

        $scope.personalDetails = {
            OrderId: $scope.orderData.Order.AggregateRootId,
            FirstName: '',
            LastName: '',
            Email: '',
            Mobile: '',
        };

        $scope.setPersonalDetails = function () {
            $scope.personalDetailsSubmitted = true;
            
            if (!$scope.checkoutTwo.$valid) {
                return;
            }
            $scope.orderDetailsLoader = true;

            Restangular.all('PersonalDetails').post($scope.personalDetails).then(function (result) {
                //$scope.showPaymentDetails = true;
                $('#step-three').removeClass('unstep');
                $(window).scrollTo($('#step-three'), { duration: 800 });
                $scope.orderDetailsLoader = false;
            },
            function (error) {
                $scope.orderDetailsLoader = false;
                alert("error occured");
            });
        };

        $scope.setDeliveryDetails = function () {
            $scope.deliveryDetailsSubmitted = true;
            
            if (!$scope.deliverDetailsForm.$valid) {
                return;
            }
            $scope.orderDeliveryLoader = true;

            Restangular.all('DeliveryDetails').post($scope.deliveryDetails).then(function (result) {
                    //$scope.showPersonalDetails = true;
                $('#step-two').removeClass('unstep');
                $(window).scrollTo($('#step-two'), { duration: 800 });
                $scope.orderDeliveryLoader = false;
            },

            function(error) {
                $scope.orderDeliveryLoader = false;
                alert("error occured");
            });
        };

        $scope.getStripeToken = function () {
            $scope.paymentDetailsSubmitted = true;
            
            if (!$scope.paymentForm.$valid) {
                return;
            }
            $scope.orderCCLoader = true;

            Stripe.card.createToken({
                number: $scope.creditCard.number,
                cvc: $scope.creditCard.cvc,
                exp_month: $scope.creditCard.expiryMonth,
                exp_year: $scope.creditCard.expiryYear
            }, $scope.stripeResponseHandler);
        }

        $scope.paypalPayment = function () {
            $scope.orderPaypalLoader = true;
            Restangular.all('Payment/paypal-redirect').post({ OrderId: $scope.orderData.Order.AggregateRootId }).then(function (result) {
                window.location = result;
            });
        };

        $scope.stripeResponseHandler = function (status, response) {
            if (response.error) {
                alert(response.error.message);
                $scope.orderCCLoader = false;
            } else {

                var token = response.id;
                Restangular.all('Payment/Stripe').post({ PaymentToken: token, OrderId: $scope.orderData.Order.AggregateRootId }).then(function (result) {
                    window.location = "/order/Complete/" + result;
                });
            }
        }

        $scope.subTotal = function () {
            var result = 0;
            for (var index = 0; index < $scope.orderData.Order.OrderItems.length; index++) {
                result += $scope.orderData.Order.OrderItems[index].Quantity * $scope.orderData.Order.OrderItems[index].Price;
            }
            return result;
        };

        $scope.total = function () {
            return $scope.subTotal() + $scope.deliveryFee;
        };
    };

}).call(this);