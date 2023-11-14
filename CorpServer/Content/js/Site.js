$(document).ready(function() {
    var $j_object = $(".myTable");
    $j_object.each(function () {
        var dataCount = $(this).children(".row:not(.myHeader):not(.myFooter)").length;
        if (dataCount === 0) {
            $(this).children().css("display","none");
        }
    });
})