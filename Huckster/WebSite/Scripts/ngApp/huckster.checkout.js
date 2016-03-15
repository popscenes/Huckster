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

        

        $scope.InitCheckout = function() {
            Stripe.setPublishableKey('pk_test_e9OAHJzcasWjjnNbMNNQlMlL');
            $scope.orderData = angular.fromJson($("#orderData").val());

            $scope.paymentPaypal = false;
            $scope.paymentCC = false;
            $scope.deliveryFee = $scope.orderData.Order.DeliveryFee;

            $scope.deliveryDetailsSubmitted = false;
            $scope.personalDetailsSubmitted = false;
            $scope.paymentDetailsSubmitted = false;
            $scope.orderPaypalLoader = false;

            $scope.orderDeliveryLoader = false;
            $scope.orderDetailsLoader = false;
            $scope.orderCCLoader = false;

            $scope.serverErrorDelivery = false;
            $scope.serverErrorPersonal = false;
            $scope.serverErrorPayment = false;

            $scope.deliveryDetails = {
                OrderId: $scope.orderData.Order.AggregateRootId,
                Street: $scope.orderData.DeliverAddress == null ? "" : $scope.orderData.DeliverAddress.Street,
                Number: $scope.orderData.DeliverAddress == null ? "" : $scope.orderData.DeliverAddress.Number,
                Suburb: $scope.orderData.DeliverySuburb.Suburb,
                State: $scope.orderData.DeliverySuburb.State,
                Postcode: $scope.orderData.DeliverySuburb.Postcode
            };

            if ($scope.deliveryDetails.Street !== "" && $scope.deliveryDetails.Number !== "") {
                $scope.showPersonalDetails();
            }
            var firstName = "";
            var lastName = "";

            if ($scope.orderData.Customer != null) {
                var name = $scope.orderData.Customer.Name;
                var nameArr = name.split(" ");
                firstName = nameArr[0];
                lastName = nameArr[1];
            }
            

            $scope.personalDetails = {
                OrderId: $scope.orderData.Order.AggregateRootId,
                FirstName: firstName,
                LastName: lastName,
                Email: $scope.orderData.Customer == null ? "": $scope.orderData.Customer.Email,
                Mobile: $scope.orderData.Customer == null ?"": $scope.orderData.Customer.Mobile,
            };

            if ($scope.personalDetails.FirstName !== "" && $scope.personalDetails.FirstName !== ""
                && $scope.personalDetails.Email !== "" && $scope.personalDetails.Mobile !== "") {
                $scope.showPaymentlDetails();
            }
        }

        $scope.showPayPal = function() {
            $scope.paymentPaypal = true;
            $scope.paymentCC = false;
        };

        $scope.showCC = function () {
            $scope.paymentPaypal = false;
            $scope.paymentCC = true;
        };



        $scope.setPersonalDetails = function () {
            $scope.personalDetailsSubmitted = true;
            
            if (!$scope.checkoutTwo.$valid) {
                return;
            }
            $scope.orderDetailsLoader = true;

            Restangular.all('PersonalDetails').post($scope.personalDetails).then(function (result) {
                    $scope.showPaymentlDetails();
                },
            function (error) {
                $scope.orderDetailsLoader = false;
                $scope.serverErrorPersonal = true;
            });
        };

        $scope.setDeliveryDetails = function () {
            $scope.deliveryDetailsSubmitted = true;
            
            if (!$scope.deliverDetailsForm.$valid) {
                return;
            }
            $scope.orderDeliveryLoader = true;

            Restangular.all('DeliveryDetails').post($scope.deliveryDetails).then(function (result) {
                    $scope.showPersonalDetails();
                },

            function(error) {
                $scope.orderDeliveryLoader = false;
                $scope.serverErrorDelivery = true;
            });
        };

        $scope.showPaymentlDetails = function () {
            $('#step-three').removeClass('unstep');
            $(window).scrollTo($('#step-three'), { duration: 800 });
            $scope.orderDetailsLoader = false;
            $scope.serverErrorPersonal = false;
        }

        $scope.showPersonalDetails = function () {
            $('#step-two').removeClass('unstep');
            $(window).scrollTo($('#step-two'), { duration: 800 });
            $scope.orderDeliveryLoader = false;
            $scope.serverErrorDelivery = false;
        }

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
            }, function(error) {
                $scope.serverErrorPayment = true;
            });
        };

        $scope.stripeResponseHandler = function (status, response) {
            if (response.error) {
                $scope.serverErrorPayment = true;
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

        $scope.InitCheckout();
    };

}).call(this);