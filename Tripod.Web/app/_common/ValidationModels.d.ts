interface ValidatedFieldError {
    message: string;
    customState: any;
}

interface ValidatedField {
    attemptedValue: any;
    attemptedString: string;
    errors: ValidatedFieldError[];
    isValid: boolean;
}
