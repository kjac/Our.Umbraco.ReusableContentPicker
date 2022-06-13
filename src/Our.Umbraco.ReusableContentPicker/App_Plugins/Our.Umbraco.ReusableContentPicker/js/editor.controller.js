angular.module("umbraco").controller("Our.Umbraco.ReusableContentPicker.Editor.Controller", function ($scope, editorService, entityResource, angularHelper) {
    var vm = this;

    vm.add = add;
    vm.remove = remove;
    vm.open = open;
    vm.items = [];

    vm.config = {
        multiSelect: $scope.model.config.multiSelect === true
    };

    if ($scope.model.value) {
        var ids = $scope.model.value.split(',');
        entityResource.getByIds(ids, "Document").then(function (data) {
            vm.items = data;
            vm.items.forEach(i => i.published = isPublished(i));
        });
    }

    function open(item) {
        const cssTweakClass = "-content-builder-infinite-editor";
        document.body.classList.add(cssTweakClass);

        var editor = {
            id: item.id,
            size: "medium",
            submit: function (model) {
                entityResource.getById(model.contentNode.id, "Document").then(function (data) {
                    item.name = data.name;
                    item.published = isPublished(data);
                });

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

    function remove(item) {
        vm.items.splice(vm.items.indexOf(item), 1);
        updateSortableOptions();
        syncModelValue();
        setDirty();
    }

    function add() {
        var editor = {
            submit: function (model) {
                if (model.target) {
                    vm.items.push(model.target);
                    updateSortableOptions();
                    syncModelValue();
                    setDirty();
                }
                editorService.close();
            },
            close: function () {
                editorService.close();
            },
            view: "/App_Plugins/Our.Umbraco.ReusableContentPicker/views/picker.html",
            size: "medium"
        };
        editorService.open(editor);
    }

    function setDirty() {
        angularHelper.getCurrentForm($scope).$setDirty();
    }

    function updateSortableOptions() {
        vm.sortableOptions.disabled = vm.items.length <= 1;
    }

    function isPublished(item) {
        return item.metaData && item.metaData.IsPublished;
    }

    function syncModelValue() {
        $scope.model.value = vm.items.map(i => i.udi).join(',');
    }

    vm.sortableOptions = {
        axis: "y",
        containment: "parent",
        distance: 10,
        opacity: 0.7,
        tolerance: "pointer",
        scroll: true,
        zIndex: 6000,
        disabled: false,
        update: function (e, ui) {
            setDirty();
        }
    };
});