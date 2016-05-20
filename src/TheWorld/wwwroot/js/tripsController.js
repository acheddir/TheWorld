// tripsController.js
(function () {

    "use strict";

    // Getting the existing module
    angular.module("app-trips")
        .controller("tripsController", tripsController);

    function tripsController($http) {
        var vm = this;

        vm.trips = [];

        vm.newTrip = {};

        vm.errorMessage = "";
        vm.isBusy = true;
        
        $http.get("/api/trips")
            .then(function (response) {
                // success
                angular.copy(response.data.trips, vm.trips);
            }, function (err) {
                // failure
                vm.errorMessage = "Failed to load data: " + err;
            })
            .finally(function () {
                vm.isBusy = false;
            });

        vm.addTrip = function () {
            vm.isBusy = true;

            $http.post("/api/trips", vm.newTrip)
                .then(function (response) {
                    // success
                    vm.trips.push(response.data.createdTrip);
                    vm.newTrip = {};
                }, function (err) {
                    // failure
                    vm.errorMessage = "Failed to save a new trip: " + err;
                })
                .finally(function() {
                    vm.isBusy = false;
                });
        };
    }

})();