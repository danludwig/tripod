 module App.Common.Unobtrusive.BootBoxConfirm {

     $(document).on('click', '[data-bootbox=confirm-form-submit]', (e: JQueryEventObject) => {
         e.preventDefault();
         var clicked = $(e.target);
         var options: any = {
             message: clicked.data('bootbox-message') || 'Are you sure you want to do this?',
             closeButton: clicked.data('bootbox-close') || false,
             buttons: {
                 confirm: {
                     label: clicked.data('bootbox-confirm') || 'Confirm',
                     className: clicked.data('bootbox-confirm-class') || '',
                     callback: (isConfirmed: boolean) => {
                         if (!isConfirmed) return;
                         clicked.parents('form').submit();
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

 }