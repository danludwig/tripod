ko.bindingHandlers.jQuery = {
    preprocess: function (value) {
        return "'" + value + "'";
    },
    update: function (element, valueAccessor, allBindingsAccessor, viewModel) {
        var name = ko.utils.unwrapObservable(valueAccessor());
        viewModel[name] = $(element);
    }
};
