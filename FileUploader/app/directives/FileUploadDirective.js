var FileUploaderApp;
(function (FileUploaderApp) {
    var Directives;
    (function (Directives) {
        "use strict";
        var FileUpload = (function () {
            function FileUpload() {
                this.restrict = "A";
            }
            FileUpload.factory = function () {
                return function () { return new FileUpload(); };
            };
            FileUpload.prototype.link = function (scope, element, attributes, modelController) {
                element.bind("change", scope.$eval(attributes.Handler));
            };
            return FileUpload;
        }());
        Directives.FileUpload = FileUpload;
        FileUploaderApp.FileUploader.module.directive("fileUpload", FileUpload.factory());
    })(Directives = FileUploaderApp.Directives || (FileUploaderApp.Directives = {}));
})(FileUploaderApp || (FileUploaderApp = {}));

//# sourceMappingURL=FileUploadDirective.js.map
