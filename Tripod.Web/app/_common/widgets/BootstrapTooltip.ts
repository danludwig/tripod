module App.Widgets {

    export class BootstrapTooltip {

        isVisible = ko.observable(false);

        constructor(public jQuery: JQuery, public options: TooltipOptions) {
            jQuery.tooltip(options);
            $(window).on('resize', (): void => { this._onResize(); });
        }

        show(): void {
            this.isVisible(true);
            if ($(window).width() >= 992)
                this.jQuery.tooltip('show');
        }

        hide(): void {
            this.isVisible(false);
            this.jQuery.tooltip('hide');
        }

        title(text: string, show: boolean = true): void {
            this.jQuery.attr('title', text).tooltip('fixTitle');
            if (show) this.show();
            else this.hide();
        }

        private _resizeTimeout: number;
        private _onResize(): void {

            // hide tooltips when we fall below medium viewport
            var viewportWidth = $(window).width();
            if (viewportWidth < 992) {
                this.jQuery.tooltip('hide');
            }
            else if (this.isVisible()) {
                this.jQuery.tooltip('show');
            }

            //if (this._resizeTimeout) {
            //    clearTimeout(this._resizeTimeout);
            //}
            //this._resizeTimeout = setTimeout((): void => {
            //    //alert('will resize now');
            //}, 100);
        }
    }
}