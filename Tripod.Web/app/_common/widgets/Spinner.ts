module App.Widgets {

    export interface SpinnerSettings {
        delay?: number;
        runImmediately?: boolean;
    }

    export class Spinner {

        static defaultSettings: SpinnerSettings = {
            delay: 0,
            runImmediately: false,
        };

        // this offers a way to short circuit the spinner when its activity time is
        // greater than zero but less than the delay
        isRunning: KnockoutObservable<boolean> = ko.observable(true);

        isVisible: KnockoutObservable<boolean> = ko.observable(false);

        constructor(public settings: SpinnerSettings = {}) {
            this.settings = $.extend({}, Spinner.defaultSettings, this.settings);

            this.isRunning(this.settings.runImmediately);
            this.isVisible(this.settings.runImmediately);
        }

        start(immediately: boolean = false): void {
            this.isRunning(true);
            if (this.settings.delay < 1 || immediately)
                this.isVisible(true);
            else
                setTimeout((): void => {
                    // only show spinner when load is still being processed
                    if (this.isRunning())
                        this.isVisible(true);
                }, this.settings.delay);
        }
        stop(): void {
            this.isVisible(false);
            this.isRunning(false);
        }
    }
}