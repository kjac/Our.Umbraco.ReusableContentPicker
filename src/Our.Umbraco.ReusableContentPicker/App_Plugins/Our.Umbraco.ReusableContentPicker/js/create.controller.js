angular.module("umbraco").controller("Our.Umbraco.ReusableContentPicker.Create.Controller", function ($scope, $http, contentTypeResource, iconHelper, editorService) {
    var vm = this;
    vm.submit = submit;
    vm.close = close;
    vm.createFolder = createFolder;
    vm.loading = true;
    vm.creatingFolder = false;
    vm.allowedContentTypes = [];

    vm.labels = {
        selectTypeDescription: "Choose the type of item to create in '" + $scope.model.parent.name + "'",
        createFolderDescription: "The new folder will be created in '" + $scope.model.parent.name + "'"
    };

    contentTypeResource.getAllowedTypes($scope.model.parent.id).then(function (data) {
        vm.allowedContentTypes = iconHelper.formatContentTypeIcons(data);
        vm.loading = false;
    });

    function submit(contentType) {
        if (!contentType) {
            return;
        }

        if (contentType.alias === $scope.model.folderContentTypeAlias) {
            vm.creatingFolder = true;
            return;
        }

        // hack: make sure the infinite editor buttons are styled in a meaningful way
        const cssTweakClass = "-content-builder-infinite-editor";
        document.body.classList.add(cssTweakClass);

        var editor = {
            create: true,
            size: "medium",
            parentId: $scope.model.parent.id,
            documentTypeAlias: contentType.alias,
            submit: function (model) {
                $scope.model.target = model.contentNode;
                $scope.model.submit($scope.model);
                document.body.classList.remove(cssTweakClass);
                editorService.close();
            },
            close: function () {
                document.body.classList.remove(cssTweakClass);
                editorService.close();
            },
            allowPublishAndClose: true,
            allowSaveAndClose: true
        };
        editorService.contentEditor(editor);
    }

    function close() {
        if ($scope.model && $scope.model.close) {
            $scope.model.close();
        }
    }

    function createFolder(name) {
        $http
            .post("/umbraco/backoffice/reusablecontent/editorapi/createfolder", { name: name, parentId: $scope.model.parent.id })
            .then(response => {
                $scope.model.target = response.data.folder;
                $scope.model.submit($scope.model);
                editorService.close();
            });
    }
});