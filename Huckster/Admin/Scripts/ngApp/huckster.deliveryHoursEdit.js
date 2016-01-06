(function () {
    //var servicabilityApp = angular.module('directMoney.Servicability', ['restangular', 'ngSanitize', 'angular-loading-bar', 'ngAnimate']);

    var module = angular.module('module.deliveryHoursEdit', []);

    module.config(['RestangularProvider',
    function (RestangularProvider) {
        RestangularProvider.setBaseUrl('/api/restaurant');
    }]);

    module.controller('deliveryHoursEditController', deliveryHoursEdit);
    deliveryHoursEdit.$inject = ['$scope', 'Restangular', '$window', '$filter'];


    function deliveryHoursEdit($scope, Restangular, $window, $filter) {
        $scope.deliveryHours = angular.fromJson($("#deliveryTimesJSON").val());
        $scope.restaurantId = $("#restaurantId").val();

        $scope.addDeliveryHours = function () {
            $scope.deliveryHours.push({ ServiceType: 'Dinner', DayOfWeek: 'Monday', OpenTime: '18:00:00', CloseTime: '20:00:00', TimeZoneId: 'AUS Eastern Standard Time' });
        };

        $scope.removeDeliveryHours = function(index) {
            $scope.deliveryHours.splice(index, 1);
        };

        $scope.updateOpenTime = function(deliveryTime, newOpenHours) {
            deliveryTime.OpenTime = $filter('date')(newOpenHours, 'H:mm:ss');
        }

        $scope.updateCloseTime = function (deliveryTime, newOpenHours) {
            deliveryTime.CloseTime = $filter('date')(newOpenHours, 'H:mm:ss');
        }

        $scope.InitUnformattedOpenTime = function (deliveryTime, timeSpan) {
            deliveryTime.UnformattedOpenTime = new Date();
            var timeArray = timeSpan.split(":");
            deliveryTime.UnformattedOpenTime.setHours(parseInt(timeArray[0]));
            deliveryTime.UnformattedOpenTime.setMinutes(parseInt(timeArray[1]));
            deliveryTime.UnformattedOpenTime.setSeconds(0);
        };

        $scope.InitUnformattedCloseTime = function (deliveryTime, timeSpan) {
            deliveryTime.UnformattedCloseTime = new Date();
            var timeArray = timeSpan.split(":");
            deliveryTime.UnformattedCloseTime.setHours(parseInt(timeArray[0]));
            deliveryTime.UnformattedCloseTime.setMinutes(parseInt(timeArray[1]));
            deliveryTime.UnformattedCloseTime.setSeconds(0);
        };

        $scope.updateDeliveryHours = function () {
            Restangular.all($scope.restaurantId + '/update-delivery-hours').post({ deliveryHours: $scope.deliveryHours }).then(function (result) {
                alert("delivery hours saved");
            }, function (error) {
                alert(error);
            });
        };

    };



}).call(this);