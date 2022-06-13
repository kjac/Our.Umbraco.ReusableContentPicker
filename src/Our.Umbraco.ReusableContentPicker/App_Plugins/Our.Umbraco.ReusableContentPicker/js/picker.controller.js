angular.module("umbraco").controller("Our.Umbraco.ReusableContentPicker.Picker.Controller", function ($scope, $http, $q, editorService, editorState) {
    var vm = this;

    vm.submit = submit;
    vm.create = create;
    vm.close = close;
    vm.searchInfo = {
        showSearch: false
    };
    vm.selectedItem = null;
    vm.rootLoading = true;
    vm.itemsLoading = true;
    vm.root = null;
    vm.current = null;
    vm.items = null;
    vm.folders = null;
    vm.pagination = {
        pageNumber: 1,
        totalPages: 1
    };
    vm.query = "";
    vm.searching = false;

    var path = [];
    var folderContentTypeAlias = "";

    function load(folderId, pageNumber) {
        var deferred = $q.defer();
        vm.itemsLoading = true;
        $http
            .get("/umbraco/backoffice/reusablecontent/editorapi/loadfolder", { params: { contentId: editorState.getCurrent().id, folderId: folderId, pageNumber: pageNumber, query: vm.query } })
            .then(response => {
                folderContentTypeAlias = response.data.folderContentTypeAlias;

                vm.current = response.data.current;

                vm.items = response.data.items;
                vm.folders = response.data.folders;

                vm.pagination.pageNumber = response.data.pageNumber;
                vm.pagination.totalPages = response.data.totalPages;
                vm.rootLoading = false;
                vm.itemsLoading = false;
                vm.searching = vm.query.length > 0;

                deferred.resolve();
            });

        return deferred.promise;
    }

    function loadAndPushPath(folderId, pageNumber) {
        load(folderId, pageNumber).then(_ => path.push(vm.current.id));
    }

    loadAndPushPath(null, 1);
    
    vm.select = function (item) {
        if (item.contentTypeAlias === folderContentTypeAlias) {
            loadAndPushPath(item.id, 1);
            return;
        }
        if ($scope.model && $scope.model.submit) {
            $scope.model.target = item;
            $scope.model.submit($scope.model);
        }
    }

    vm.canGoBack = function() {
        return path.length > 1;
    }

    vm.goBack = function () {
        if (!vm.canGoBack()) {
            return;
        }
        path.pop();

        load(path[path.length - 1], 1);
    }

    vm.goToPage = function(pageNumber) {
        load(vm.current.id, pageNumber);
    }

    var debounceSearch = _.debounce(function () {
        load(vm.current.id, 1);
    }, 500);

    vm.search = function () {
        vm.searching = true;
        debounceSearch();
    }

    function submit() {
        if (!vm.selectedItem) {
            return;
        }

        if ($scope.model && $scope.model.submit) {
            $scope.model.target = vm.selectedItem;
            $scope.model.submit($scope.model);
        }
    }

    function close() {
        if ($scope.model && $scope.model.close) {
            $scope.model.close();
        }
    }

    function create() {
        if (!vm.current) {
            return;
        }

        var editor = {
            submit: function (model) {
                if (model.target.contentTypeAlias === folderContentTypeAlias) {
                    vm.select(model.target);
                    return;
                }
                editorService.close();
                $http
                    .get("/umbraco/backoffice/reusablecontent/editorapi/getitem", { params: { id: model.target.id } })
                    .then(response => {
                        vm.select(response.data.item);
                    });
            },
            close: function () {
                editorService.close();
            },
            view: "/App_Plugins/Our.Umbraco.ReusableContentPicker/views/create.html",
            size: "small",
            parent: vm.current,
            folderContentTypeAlias: folderContentTypeAlias
        };
        editorService.open(editor);
    }
});