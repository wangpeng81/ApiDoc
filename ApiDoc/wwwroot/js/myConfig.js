

function showCreateAuthorize() {
     
    showModalInfo("请确认是否要保存，保存后将更改码?", btnSaveAuthorize_Click);

}

//保存授权码
function btnSaveAuthorize_Click() {
     
    var txtAuthorize = $("#txtSecurityKey");
    var Authorize = txtAuthorize.val();
    var uri = urlMyConfigSaveAuthorize + "?SecurityKey=" + Authorize + "&random=" + Math.random();

    $.ajax({
        cache: false,
        url: uri,
        type: "GET", 
        success: function (result) {
            if (result > 0) {
                popToastSuccess("保存成功!");
            }
        }
    }); 

    return;
}

function btnCreateAuthorize_Click() {
    var txtAuthorize = $("#txtAuthorize");
    var vguid = guid();
    txtAuthorize.val(vguid);
}

function guid() {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
        var r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
        return v.toString(16);
    });
}

$(function () {

    //初始化提示信息
    var option = { animation: true, delay: 2000 };
    $('.toast').toast(option);
 
});
    