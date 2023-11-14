$(document).ready(function() {
    
    $('#txtTags').on('input',
        function(e) {

            var fullText = $(this).val();
            var tags = fullText.split(/,\s*/);
            var length = tags.length;

            var lastTag = tags[length - 1];


            getAndSetData(tags[length - 1], "txtTags");

        });


    function getAndSetData(term, textboxId) {

        var xhttp = new XMLHttpRequest();
        xhttp.onreadystatechange = function() {
            if (xhttp.readyState == 4 && xhttp.status == 200) {
                document.getElementById("state").innerHTML = xhttp.responseText;
            }
        };
        xhttp.open("GET", "/Cpanel/ArticleCatagory/AutoComplete?term=" + term, true);
        xhttp.send();


        var goOn = true;
        var myInterval = setInterval(function() {

                var check = document.getElementById("state").innerHTML;


                if ((check !== "") && goOn) {
                    var str = check;
                    var count = str.length - 1;
                    str = str.substring(1, count);
                    str = str.replace(/"/g, "");

                    var tags = new Array();
                    tags = str.split(",");
                    TagsResponse(tags, textboxId);


                    clearInterval(myInterval);
                    goOn = false;
                }
            },
            300);

    }
});

    function TagsResponse(availableTags, textboxId) {

        
        function split(val) {
            return val.split(/,\s*/);
        }

        function extractLast(term) {
            return split(term).pop();
        }

        $("#" + textboxId)
            // don't navigate away from the field on tab when selecting an item
            .bind("keydown", function (event) {
                if (event.keyCode === $.ui.keyCode.TAB &&
                    $(this).autocomplete("instance").menu.active) {
                    event.preventDefault();
                }
            })
            .autocomplete({
                minLength: 0,
                source: function (request, response) {
                    // delegate back to autocomplete, but extract the last term
                    response($.ui.autocomplete.filter(
                        availableTags, extractLast(request.term)));
                },
                focus: function () {
                    // prevent value inserted on focus
                    return false;
                },
                select: function (event, ui) {
                    var terms = split(this.value);
                    // remove the current input
                    terms.pop();
                    // add the selected item
                    terms.push(ui.item.value);
                    // add placeholder to get the comma-and-space at the end
                    terms.push("");
                    this.value = terms.join(",");
                    return false;
                }
            });
    };
