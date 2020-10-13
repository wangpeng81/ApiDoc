$(function () {

    loadParam();

});

function loadParam() {

    var fksn = $('#txtSN').val();
    var data = { FKSN: fksn };
    $.post(urlParamList, data, function (result) {
        $("#myParamList").html(result);
    });
}

function showParamAdd() {

    $("#myParamModel").model("show");

}