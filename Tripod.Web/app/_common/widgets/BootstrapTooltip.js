var App;
(function (App) {
    (function (Widgets) {
        var BootstrapTooltip = (function () {
            function BootstrapTooltip(jQuery, options) {
                var _this = this;
                this.jQuery = jQuery;
                this.options = options;
                this.isVisible = ko.observable(false);
                jQuery.tooltip(options);
                $(window).on('resize', function () {
                    _this._onResize();
                });
            }
            BootstrapTooltip.prototype.show = function () {
                this.isVisible(true);
                if ($(window).width() >= 992)
                    this.jQuery.tooltip('show');
            };

            BootstrapTooltip.prototype.hide = function () {
                this.isVisible(false);
                this.jQuery.tooltip('hide');
            };

            BootstrapTooltip.prototype.title = function (text, show) {
                if (typeof show === "undefined") { show = true; }
                this.jQuery.attr('title', text).tooltip('fixTitle');
                if (show)
                    this.show();
                else
                    this.hide();
            };

            BootstrapTooltip.prototype._onResize = function () {
                var viewportWidth = $(window).width();
                if (viewportWidth < 992) {
                    this.jQuery.tooltip('hide');
                } else if (this.isVisible()) {
                    this.jQuery.tooltip('show');
                }
            };
            return BootstrapTooltip;
        })();
        Widgets.BootstrapTooltip = BootstrapTooltip;
    })(App.Widgets || (App.Widgets = {}));
    var Widgets = App.Widgets;
})(App || (App = {}));
