$(document).ready(function() {
    

});

$(document).on('submit', 'form', function () {
    var button = $(this).find("[type='submit']");
    setTimeout(function () {
        button.attr('disabled', 'disabled');
    }, 0);
});