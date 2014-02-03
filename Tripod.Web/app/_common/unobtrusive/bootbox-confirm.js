var App;
(function (App) {
    (function (Common) {
        (function (Unobtrusive) {
            (function (BootBoxConfirm) {
                $(document).on('click', '[data-bootbox=confirm-form-submit]', function (e) {
                    e.preventDefault();
                    var clicked = $(e.target);
                    var options = {
                        message: clicked.data('bootbox-message') || 'Are you sure you want to do this?',
                        closeButton: clicked.data('bootbox-close') || false,
                        buttons: {
                            confirm: {
                                label: clicked.data('bootbox-confirm') || 'Confirm',
                                className: clicked.data('bootbox-confirm-class') || '',
                                callback: function (isConfirmed) {
                                    if (!isConfirmed)
                                        return;
                                    clicked.parents('form').submit();
                                }
                            },
                            deny: {
                                label: clicked.data('bootbox-deny') || 'Cancel',
                                className: clicked.data('bootbox-deny-class') || ''
                            }
                        }
                    };
                    var title = clicked.data('bootbox-title');
                    if (title)
                        options.title = title;
                    var className = clicked.data('bootbox-class');
                    if (className)
                        options.className = className;
                    bootbox.dialog(options);
                    return false;
                });
            })(Unobtrusive.BootBoxConfirm || (Unobtrusive.BootBoxConfirm = {}));
            var BootBoxConfirm = Unobtrusive.BootBoxConfirm;
        })(Common.Unobtrusive || (Common.Unobtrusive = {}));
        var Unobtrusive = Common.Unobtrusive;
    })(App.Common || (App.Common = {}));
    var Common = App.Common;
})(App || (App = {}));
