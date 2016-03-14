function newAlert(type, message) {
    $("#alert-area").append($("<div class='alert-message successAction " + type + " fade in' data-alert><p> " + message + " </p></div>"));
    $(".alert-message").delay(2000).fadeOut("slow", function () { $(this).remove(); });
}
