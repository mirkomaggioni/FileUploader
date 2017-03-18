
namespace FileUploaderApp {
    "use strict";

    export class FileUploader {
        public static module: ng.IModule = angular.module("fileUploader", ["ui.router"]);
    }

    FileUploader.module.config(($stateProvider: ng.ui.IStateProvider, $urlRouterProvider: ng.ui.IUrlRouterProvider) => {
        $urlRouterProvider.otherwise("/fileUploader");

        $stateProvider
            .state("FileUploader", {
                url: "/fileUploader",
                templateUrl: "/app/views/fileUploader.html",
                controller: "FileUploaderController as vm"
            });
    });
}
