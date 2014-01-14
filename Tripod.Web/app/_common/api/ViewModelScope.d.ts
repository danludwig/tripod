declare module App {

    export interface ViewModelScope<T> extends ng.IScope {
        vm: T;
    }
}