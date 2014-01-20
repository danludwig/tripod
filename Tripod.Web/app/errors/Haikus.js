(function ($) {
    var $haikus = $('#haikus');
    var $button = $('#haiku_btn');
    var haikuCount = $haikus.children().length;
    var haikuIndex = Math.floor(Math.random() * haikuCount);
    var isPaused = false;

    var showHaiku = function (index, speed) {
        $($haikus.children()[index]).css('display', 'none').removeClass('hide').fadeIn(speed);
    };

    var nextHaiku = function (speed) {
        ++haikuIndex;
        if (haikuIndex >= haikuCount)
            haikuIndex = 0;
        $haikus.children().not('.hide').fadeOut(speed, function () {
            $(this).addClass('hide');
            showHaiku(haikuIndex, speed);
        });
    };

    setInterval(function () {
        if (isPaused)
            return;
        nextHaiku('slow');
    }, 6000);

    $button.on('click', function () {
        nextHaiku('fast');
        return false;
    });
    $('#haikus, #haiku_btn').on('mouseenter', function () {
        isPaused = true;
    }).on('mouseleave', function () {
        isPaused = false;
    });

    showHaiku(haikuIndex, 0);
})(jQuery);
