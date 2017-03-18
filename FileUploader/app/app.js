var FileUploaderApp;
(function (FileUploaderApp) {
    "use strict";
    var FileUploader = (function () {
        function FileUploader() {
        }
        FileUploader.module = angular.module("fileUploader", ["ui.router"]);
        return FileUploader;
    }());
    FileUploaderApp.FileUploader = FileUploader;
    FileUploader.module.config(function ($stateProvider, $urlRouterProvider) {
        $urlRouterProvider.otherwise("/fileUploader");
        $stateProvider
            .state("FileUploader", {
            url: "/fileUploader",
            templateUrl: "/app/views/fileUploader.html",
            controller: "FileUploaderController as vm"
        });
    });
})(FileUploaderApp || (FileUploaderApp = {}));

//# sourceMappingURL=app.js.map
