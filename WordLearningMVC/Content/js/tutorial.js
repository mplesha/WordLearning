var count = 0;
var removeItemIndex = 0;
var color = "#d3dce0";

function checkAnswer(element, answers) {
    //changeTextStyle();
    var flag = false;
    if ($(element).attr('name').toLowerCase() == "true") {
        flag = true;
        $(element).css("background-color", "chartreuse");
    } else {
        $(element).css("background-color", "red");
        $("input[name=true]").css("background-color", "chartreuse");
    }

    if (flag) {
        var removeItem = answers[0][removeItemIndex];
        answers[0] = jQuery.grep(answers[0], function (value) {
            return value != removeItem;
        });
    }

    if (count >= answers[0].length - 1 || answers[0].length == 1) {
        count = 0;
    } else {
        count = Math.floor((Math.random() * (answers[0].length - 1)));
    }

    if (answers[0].length != 0) {
        sleep(700, color, answers[0][count]);
    } else {
        window.location.href = "/Tutorial/FinishTutorial";
    }
    removeItemIndex = count;
    window.clearInterval(interval);
    StartProgress();
   
}

function sleep(millisecondsToWait, color, variants) {
    setTimeout(function () {
        changeWord(color, variants);
    }, millisecondsToWait);
}

function changeWord(color, variants) {
    $('#titleWord').val(variants.TitleWord);
    $('#option1').val(variants.PossibleAnswers[0]);
    $('#option2').val(variants.PossibleAnswers[1]);
    $('#option3').val(variants.PossibleAnswers[2]);
    $('#option4').val(variants.PossibleAnswers[3]);
    $('#option1').attr('name', variants.Colors[0].toString());
    $('#option2').attr('name', variants.Colors[1].toString());
    $('#option3').attr('name', variants.Colors[2].toString());
    $('#option4').attr('name', variants.Colors[3].toString());
    $('#titleWord').css("background-color", color);
    $('#option1').css("background-color", color);
    $('#option2').css("background-color", color);
    $('#option3').css("background-color", color);
    $('#option4').css("background-color", color);
}

function changeFontSize(button) {
    var lenght = $(button).val().toString().length - 20;
    if (lenght > 0) {
        var size = parseInt($(button).css("font-size"), 10);
        var pixel = (size - lenght) + 'px';
        $(button).css("font-size", pixel);
    } else {
        $(button).css("font-size", 28);
    }
}

function defaultFontSize(button) {
    $(button).css("font-size", '28px');
}

function changeTextStyle() {
    defaultFontSize($('#option1'));
    changeFontSize($('#option1'));
    defaultFontSize($('#option2'));
    changeFontSize($('#option2'));
    defaultFontSize($('#option3'));
    changeFontSize($('#option3'));
    defaultFontSize($('#option4'));
    changeFontSize($('#option4'));
}

function StartProgress() {
    $("#progressTimer").progressTimer({
        timeLimit: $("#restTime").val(),
        onFinish: function () {
            nextWord(window.answers);
        }
    });
}

$(document).ready(function () {
    StartProgress();
});

function nextWord(answers) {
    if (count >= answers[0].length - 1 || answers[0].length == 1) {
        count = 0;
    } else {
        count++;
    }
    $("input[name=true]").css("background-color", "chartreuse");
    var color = $("#titleWord").css("background-color");
    sleep(700, color, answers[0][count]);
    window.clearInterval(interval);
    StartProgress();
}
