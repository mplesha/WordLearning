$(document).ready(function () {
    $("input[type='text']").each(function () {
        $(this).attr("autocomplete", "off");
    });
});
