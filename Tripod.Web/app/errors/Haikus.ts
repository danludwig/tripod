(($: JQueryStatic) => {
    var $haikus: JQuery = $('#haikus');
    var $button: JQuery = $('#haiku_btn');
    var haikuCount = $haikus.children().length;
    var haikuIndex = Math.floor(Math.random() * haikuCount);
    var isPaused = false;

    var showHaiku = (index: number, speed: any) => {
        $($haikus.children()[index]).css('display', 'none')
            .removeClass('hide').fadeIn(speed);
    };

    var nextHaiku = (speed: any) => {
        ++haikuIndex;
        if (haikuIndex >= haikuCount) haikuIndex = 0;
        $haikus.children().not('.hide').fadeOut(speed, function () {
            $(this).addClass('hide');
            showHaiku(haikuIndex, speed);
        });
    };

    setInterval(() => {
        if (isPaused) return;
        nextHaiku('slow');
    }, 6000);

    $button.on('click', () => {
        nextHaiku('fast');
        return false;
    });
    $('#haikus, #haiku_btn')
        .on('mouseenter', () => { isPaused = true; })
        .on('mouseleave', () => { isPaused = false; });

    showHaiku(haikuIndex, 0);
})(jQuery);
