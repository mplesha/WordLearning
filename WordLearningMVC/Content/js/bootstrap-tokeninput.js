jQuery(function() {
    $("#download-master").click(function() {
        _trackEvent("Downloads", "master")
    })
}),
jQuery(document).ready(function (a) {
    a("body").scrollspy({ target: ".bs-sidebar", offset: 0 }), a(window).on("load", function() {
        a("body").scrollspy("refresh")
    }), setTimeout(function () {
        var b = a(".bs-sidebar");
        b.affix({
            offset: {
                top: function() {
                    var a = b.offset().top, c = parseInt(b.children(0).css("margin-top"), 10);
                    return this.top = a - c
                },
                bottom: function() { return this.bottom = a(".bs-footer").outerHeight(!0) }
            }
        })
    }, 100), a(".token-example-field").tokenfield(), a("#tokenfield-1").tokenfield(
        { autocomplete: { source: ["red", "blue", "green", "yellow", "violet", "brown", "purple", "black", "white"], delay: 100 }, showAutocompleteOnFocus: !0 }), a("#tokenfield-typeahead").tokenfield({
            typeahead: {
                name: "tags",
                local: ["red", "blue", "green", "yellow", "violet", "brown", "purple", "black", "white"]
            }
        }), a("#tokenfield-2").on("beforeCreateToken", function(a) {
            var b = a.token.value.split("|");
            a.token.value = b[1] || b[0], a.token.label = b[1] ? b[0] + " (" + b[1] + ")" : b[0]
        }).on("afterCreateToken", function(b) {
        var c = /\S+@\S+\.\S+/, d = c.test(b.token.value);
        d || a(b.relatedTarget).addClass("invalid")
    }).on("beforeEditToken", function(a) {
        if (a.token.label !== a.token.value) {
            var b = a.token.label.split(" (");
            a.token.value = b[0] + "|" + a.token.value
        }
    }).on("removeToken", function(b) {
        if (b.token.length > 1) {
            var c = a.map(b.token, function(a) { return a.value });
            alert(b.token.length + " tokens removed! Token values were: " + c.join(", "))
        } else alert("Token removed! Token value was: " + b.token.value)
    }).on("preventDuplicateToken", function(a) { alert("Duplicate detected! Token value is: " + a.token.value) }).tokenfield()
});