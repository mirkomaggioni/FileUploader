
namespace FileUploaderApp.Directives {
    "use strict";

    interface ICustomAttributes extends ng.IAttributes {
        Handler: () => void;
    }

    export class FileUpload implements ng.IDirective {
        public restrict: string = "A";
        
        public static factory(): ng.IDirectiveFactory {
            return () => new FileUpload();
        }

        public link(scope: ng.IScope, element: JQuery, attributes: ICustomAttributes, modelController: ng.INgModelController): void {
            element.bind("change", scope.$eval(attributes.Handler));
        }
    }

    FileUploader.module.directive("fileUpload", FileUpload.factory());
}
