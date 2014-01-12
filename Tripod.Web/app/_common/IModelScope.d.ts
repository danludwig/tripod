declare module App {
    export interface IModelScope<T> extends ng.IScope {
        m: T;
    }
}