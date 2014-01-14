declare module App {

    export interface ValidatedFieldError {
        message?: string;
        customState?: any;
    }

    export interface ValidatedField {
        attemptedValue?: any;
        attemptedString?: string;
        errors?: ValidatedFieldError[];
        isValid?: boolean;
    }
}
