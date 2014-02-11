interface KnockoutBindingHandlers {
    jQuery: KnockoutBindingHandler;
}

ko.bindingHandlers.jQuery = {
    preprocess: (value: string): string => {
        return "'" + value + "'";
    },
    update: (element: Element, valueAccessor: () => any,
        allBindingsAccessor: () => any, viewModel: any): void => {
        var name = ko.utils.unwrapObservable(valueAccessor());
        viewModel[name] = $(element);
    }
};
