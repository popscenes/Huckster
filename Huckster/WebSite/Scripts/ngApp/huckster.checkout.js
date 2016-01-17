(function () {
    //var servicabilityApp = angular.module('directMoney.Servicability', ['restangular', 'ngSanitize', 'angular-loading-bar', 'ngAnimate']);




    var module = angular.module('module.checkout', []);

    module.config(['RestangularProvider',
    function (RestangularProvider) {
        RestangularProvider.setBaseUrl('/api/Order');
    }]);

    module.controller('CheckoutController', Checkout);
    Checkout.$inject = ['$scope', 'Restangular', '$window'];


    function Checkout($scope, Restangular, $window) {
        $scope.orderData = angular.fromJson($("#orderData").val());

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
            Restangular.all('PersonalDetails').post($scope.personalDetails).then(function (result) {
                $(window).scrollTo($('#chkBtnThree'), { duration: 800 });
            },
            function (error) {
                alert("error occured");
            });
        };

        $scope.setDeliveryDetails = function () {
            Restangular.all('DeliveryDetails').post($scope.deliveryDetails).then(function (result) {
                    $(window).scrollTo($('#chkBtnTwo'), { duration: 800 });
                },
            function(error) {
                alert("error occured");
            });
        };
    };

}).call(this);