//messages
if (parseInt($('#AddedTranslationsCount').val()) == 0) {
    $(document).ready(function () {
        alert($('#msgTranslationsExist').val());
    });
} else if (parseInt($('#AddedTranslationsCount').val()) > 0) {
    $(document).ready(function () {
        $('#ModalAddedItems').modal('show');
    });
}

//on opening page
if ($('#ModelCanShow').val() == "True") {
    $(document).ready(function () {
        $('#myModal').modal('show');
    });
}
if ($('#ModelCanShow').val() == "False" && $('#MessageForAdd').val() != '') {
    $(document).ready(function () {
        alert($('#MessageForAdd').val());
        $('#MessageForAdd').val('');
    });
}

//if modal addItemsToWordsuite hiding ->reload page
$('#modalAddedWordsuite').on('hide.bs.modal', function () {

});

//for message if item was deleted
function onDeleteItem() {
    var message = ($("#MessageForDelete123").val());
    if (message !== "") {
        alert(message);
    }
    $("#MessageForDelete123").val("");
}

//set tokens to wordsuites token-input
$(document).ready(function () {
    $.getJSON(
        "/Dictionary/GetTeacherWordsuitesNames/", {},
        function (myData) {
            var liststr = new Array();
            for (var i = 0; i < myData.length; i++) {
                liststr.push(myData[i]);
            }
            $('#tokenfieldWordsuitesFile').tokenfield({
                typeahead: {
                    name: 'tagsWordsuitesFile',
                    local: liststr,
                }
            });
            $('#tokenfieldWordsuites').tokenfield({
                typeahead: {
                    name: 'tagsAddToWordsuite',
                    local: liststr,
                }
            });
        }
    );

});

//set tokens to token-input for tags in AddItem
$.getJSON(
    "/Dictionary/GetTags/", {},
    function (myData) {
        var liststr = new Array();
        for (var i = 0; i < myData.length; i++) {
            liststr.push(myData[i]);
        }
        $('#tokenfieldTags').tokenfield({
            typeahead: {
                name: 'Addtags',
                local: liststr,
            }
        });
    }
);

//check if one o more checkbox for translations were checked
function ifChecked() {
    var cheched = false;
    for (var i = 0; i < parseInt($("#TranslationsCount").val()) ; i++) {
        var idName = 'check' + i;
        if (document.getElementById(idName).checked === true) {
            cheched = true;
            break;
        }
    }
    if (!cheched) {
        alert($('#msgChooseItems').val());
    } else {
        $('#modalAddedWordsuite').modal('show');
    }
}

//function to read all wordsuites names
function readChecked() {
    var lst = $("#tokenfieldWordsuites").tokenfield('getTokens');
    var liststr = new Array();
    for (var ii = 0; ii < lst.length; ii++) {
        liststr.push(lst[ii].value);
    }
    if (liststr.length > 0) {
        var arrayIndex = 0;
        var ids = new Array();
        var transCount = parseInt($("#TranslationsCount").val());
        for (var i = 0; i < transCount; i++) {
            var idName = 'check' + i;
            if (document.getElementById(idName).checked === true) {
                ids[arrayIndex] = document.getElementById(idName).value;
                arrayIndex++;
            }
        }
        $.ajax({
            type: 'POST',
            url: '/Dictionary/CreateWordsuite/',
            traditional: true,
            dataType: "json",
            data: { methodParam: ids, origLanguage: $("#origWordsuiteLanguage").val(), wordsuiteName: liststr },
            success: onAddedWordsuite()
        });
    } else {
        alert($('#msgWordsuiteIsRequired').val());
    }
}

//reload page and show message that items were added to wordsuite
function onAddedWordsuite() {
    $('#tokenfieldWordsuites').tokenfield('setTokens', []);
    $('#modalAddedWordsuite').modal('hide');
    //document.location.reload(true);
    alert($('#msgItemsAdded').val());
}

//on modal for wordsuites hide
$('#modalAddedWordsuite').on('hide.bs.modal', function () {
    //clear token-input
    $('#tokenfieldWordsuites').tokenfield('setTokens', []);
});

//get all wordsuites names from addFromFile modal
$('#tokenfieldWordsuitesFile').on('afterCreateToken', function () {
    var lst = $("#tokenfieldWordsuitesFile").tokenfield('getTokens');
    var liststr = "";
    for (var ii = 0; ii < lst.length; ii++) {
        liststr += lst[ii].value + ",";
    }
    $('#AddWordsuitesId').val(liststr);
});

//read data from token-input for tags in AddItem modal
$('#tokenfieldTags').on('afterCreateToken', function () {
    var lst = $("#tokenfieldTags").tokenfield('getTokens');
    var liststr = "";
    for (var ii = 0; ii < lst.length; ii++) {
        liststr += lst[ii].value + " ";
    }
    $('#AddTranslation_TagsList').val(liststr);
});

//modal for adding translations hide/close
$('#myModal').on('hide.bs.modal', function () {
    //clear originalWord and translationWord
    $('#AddTranslation_OriginalWord').val("");
    $('#AddTranslation_TranslWord').val("");
    $('#AddTranslation_Transcription').val("");
    $('#tokenfieldTags').tokenfield('setTokens', []);
    $('#AddedMessage').hide();

});

//check all checkbox
function checkAllFunc(ch) {
    if ($(ch).is(":checked")) {
        for (var i = 0; i < parseInt($("#TranslationsCount").val()) ; i++) {
            var idName = 'check' + i;
            $("#" + idName).prop("checked", true);
        }
    } else {
        for (var i = 0; i < parseInt($("#TranslationsCount").val()) ; i++) {
            var idName = 'check' + i;
            $("#" + idName).prop("checked", false);
        }
    }
};


