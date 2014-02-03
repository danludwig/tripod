 module App.Common.Unobtrusive.BootBoxConfirm {
     $(document).ready(function() {
         $(this).on('click', '[data-bootbox=confirm-form-submit]', (e: JQueryEventObject) => {
             e.preventDefault();
             var clicked = $(e.target);
             var message = clicked.data('bootbox-message') || 'Are you sure you want to do this?';
             var options: any = {
                 message: marked(message),
                 closeButton: clicked.data('bootbox-close') || false,
                 buttons: {
                     confirm: {
                         label: clicked.data('bootbox-confirm') || 'Confirm',
                         className: clicked.data('bootbox-confirm-class') || '',
                         callback: (isConfirmed: boolean): boolean => {
                             if (!isConfirmed) return true;
                             $('.bootbox.modal .modal-footer button').attr('disabled', 'disabled');
                             $('<span><i class="fa fa-spinner fa-spin spin-fast"></i> Processing...</span>')
                                 .appendTo('.bootbox.modal .modal-footer');
                             clicked.parents('form').submit();
                             return false;
                         },
                     },
                     deny: {
                         label: clicked.data('bootbox-deny') || 'Cancel',
                         className: clicked.data('bootbox-deny-class') || '',
                     },
                 },
             };
             var title = clicked.data('bootbox-title');
             if (title) options.title = title;
             var className = clicked.data('bootbox-class');
             if (className) options.className = className;
             bootbox.dialog(options);
             return false;
         });
     });
 }