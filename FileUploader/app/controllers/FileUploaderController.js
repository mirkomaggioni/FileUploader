/// <reference path="../models/FileBlob.ts" />
var FileUploaderApp;
(function (FileUploaderApp) {
    var Controllers;
    (function (Controllers) {
        "use strict";
        var FileUploaderController = (function () {
            function FileUploaderController($http) {
                this.$http = $http;
                this.url = "/api/fileBlobs";
                this.Load();
            }
            FileUploaderController.prototype.AddAttachment = function (event) {
                var _this = this;
                var attachments = event.target.files;
                if (attachments.length > 0) {
                    var file_1 = attachments[0];
                    this.$http.get(this.url + "/GetCorrelationId").then(function (correlationId) {
                        var chunks = _this.SplitFile(file_1);
                        for (var i = 0; i < chunks.length; i++) {
                            var formData = new FormData();
                            formData.append("file", chunks[i], file_1.name);
                            formData.append("correlationId", correlationId.data);
                            formData.append("chunkNumber", i + 1);
                            formData.append("totalChunks", chunks.length);
                            _this.$http.post(_this.url, formData, { headers: { "Content-Type": undefined } }).then(function (result) {
                                if (result.data) {
                                    _this.Load();
                                }
                            });
                        }
                    });
                }
            };
            FileUploaderController.prototype.DownloadFile = function (fileBlob) {
                window.open("api/fileBlobs/" + fileBlob.Id, '_blank');
            };
            FileUploaderController.prototype.Load = function () {
                var _this = this;
                this.$http.get(this.url).then(function (result) {
                    _this.fileBlobs = result.data;
                });
            };
            ;
            FileUploaderController.prototype.SplitFile = function (file) {
                var chunks = Array();
                var size = file.size;
                var chunkSize = 1024 * 1024 * 10;
                var start = 0;
                var end = chunkSize;
                while (start < size) {
                    var chunk = file.slice(start, end);
                    chunks.push(chunk);
                    start = end;
                    end += chunkSize;
                }
                return chunks;
            };
            return FileUploaderController;
        }());
        Controllers.FileUploaderController = FileUploaderController;
        FileUploaderApp.FileUploader.module.controller("FileUploaderController", FileUploaderController);
    })(Controllers = FileUploaderApp.Controllers || (FileUploaderApp.Controllers = {}));
})(FileUploaderApp || (FileUploaderApp = {}));

//# sourceMappingURL=FileUploaderController.js.map
