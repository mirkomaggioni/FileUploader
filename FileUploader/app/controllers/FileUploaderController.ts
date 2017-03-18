/// <reference path="../models/FileBlob.ts" />

namespace FileUploaderApp.Controllers {
    "use strict";

    export class FileUploaderController {
        public fileBlob: File;
        url: string = "/api/fileBlobs";
        public fileBlobs;

        constructor(public $http: ng.IHttpService) {
            this.Load();
        }

        public AddAttachment(event) {
            let attachments = event.target.files;
            if (attachments.length > 0) {
                let file: File = attachments[0];

                this.$http.get(this.url + "/GetCorrelationId").then((correlationId) => {
                    let chunks = this.SplitFile(file);

                    for (let i = 0; i < chunks.length; i++) {
                        let formData = new FormData();
                        formData.append("file", chunks[i], file.name);
                        formData.append("correlationId", correlationId.data);
                        formData.append("chunkNumber", i + 1);
                        formData.append("totalChunks", chunks.length);

                        this.$http.post(this.url, formData, { headers: { "Content-Type": undefined } }).then((result) => {
                            if(result.data) {
                                this.Load();
                            }
                        });
                    }
                });
            }
        }

        public DownloadFile(fileBlob: Models.IFileBlob) {
            window.open("api/fileBlobs/" + fileBlob.Id, '_blank');
        }

        private Load() {
            this.$http.get(this.url).then((result) => {
                this.fileBlobs = result.data;
            })
        };

        private SplitFile(file: File): Array<Blob> {
            let chunks = Array<Blob>();
            let size = file.size;
            let chunkSize = 1024 * 1024 * 10;
            let start = 0;
            let end = chunkSize;

            while (start < size) {
                let chunk = file.slice(start, end);
                chunks.push(chunk);
                start = end;
                end += chunkSize;
            }

            return chunks;
        }
    }

    FileUploader.module.controller("FileUploaderController", FileUploaderController);
}
