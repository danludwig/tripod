var App;
(function (App) {
    (function (Widgets) {
        var minLengthCustom = {
            validator: function (val, data) {
                var isValid = ko.validation.utils.isEmptyVal(val) || val.length >= data.params;
                if (!isValid) {
                    var totalLength = val ? val.length : 0;
                    var characters = 'character';
                    if (totalLength != 1)
                        characters += 's';
                    this.message = data.messageTemplate.replace('{TotalLength}', totalLength.toString()).replace('{Characters}', characters);
                }
                return isValid;
            },
            message: ''
        };
        ko.validation.rules['minLengthCustom'] = minLengthCustom;
        ko.validation.addExtender('minLengthCustom');

        var maxLengthCustom = {
            validator: function (val, data) {
                var isValid = ko.validation.utils.isEmptyVal(val) || val.length <= data.params;
                if (!isValid) {
                    var totalLength = val ? val.length : 0;
                    var characters = 'character';
                    if (totalLength != 1)
                        characters += 's';
                    this.message = data.messageTemplate.replace('{TotalLength}', totalLength.toString()).replace('{Characters}', characters);
                }
                return isValid;
            },
            message: ''
        };
        ko.validation.rules['maxLengthCustom'] = maxLengthCustom;
        ko.validation.addExtender('maxLengthCustom');

        var equalTo = {
            getValue: function (other) {
                return (typeof other === 'function' ? other() : other);
            },
            validator: function (val, otherField) {
                return val === this.getValue(otherField);
            },
            message: ''
        };
        ko.validation.rules['equalTo'] = equalTo;
        ko.validation.addExtender('equalTo');
    })(App.Widgets || (App.Widgets = {}));
    var Widgets = App.Widgets;
})(App || (App = {}));
