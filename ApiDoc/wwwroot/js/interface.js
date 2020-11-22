var _SN = 0;
 
//保存接口
function btnSaveIntterface_Click() {

    var objSN = $("#txtSN");
    var vSN = objSN.val();
    var vTitle = $("#txtTitle").val();
    var vUrl = $("#txtUrl").val();
    var vSerializeType = $("#cbxSerializeType").val();
    var vMethod = $("#cbxMethod").val();
    var vFKSN = $("#txtFKSN").val();
    var vExecuteType = $("#cbxExecuteType").val();
    var vIsTransaction = document.getElementById("txtStepIsTransaction").checked;
    var vIsJms = document.getElementById("chkIsJms").checked;

    var vIsStop = $("#txtIsStop").val();
    var vDataType = $("#cbxDataType").val();

    $.post(urlInterfaceSave, {
        SN: vSN,
        Title: vTitle,
        Url: vUrl,
        Method: vMethod,
        FKSN: vFKSN,
        SerializeType: vSerializeType,
        IsTransaction: vIsTransaction,
        ExecuteType: vExecuteType,
        IsStop: vIsStop,
        DataType: vDataType,
        IsJms: vIsJms
    }, function (data) {

            popToastSuccess("保存成功!");

            _SN = data.sn;
            $("#txtSN").val(data.sn);
    })
}

//停用接口
function btnStopIntterface_Click() {

    var isStop = $("#iPlay").hasClass("text-success");

    var url = urlInterfaceStop + "?SN=" + _SN + "&bStop=" + isStop;
    var data = { SN: _SN, bStop: isStop };
    $.get(url, function (result) {

        if (result > 0) {
            if (isStop) {
                $("#iPlay").removeClass("text-success");
                $("#txtIsStop").val(1);
            }
            else {
                $("#iPlay").addClass("text-success");
                $("#txtIsStop").val(0);
            }
        }
        else {

            popToastWarning("停用失败!"); 
        }
    });
}

//上传接口到路由
function btnUpLoad_Click() {

    var SN = $("#txtSN").val();
    var url = urlRouteUpLoad + "?SN=" + SN;
    $.get(url, function (myResponse) {

        if (myResponse.dataType == 0) {

            popToastSuccess("发布接口成功!");
             
        }
        else {
            alert( myResponse.exception );
        }
    });
}

//删除接口路由信息
function btnDownLoad_Click() {
 
    var vUrl = $("#txtUrl").val();
    var url = urlRouteDelete + "?Url=" + vUrl;
    $.get(url, function (myResponse) {

        if (myResponse.dataType == 0) {

            //成功
            alert("删除成功");
        }
        else {
            alert(myResponse.exception);
        }
    });

}

//显示接口信息的完整性
function showBugInfo() {

    var path = $("#txtUrl").val();
    var url = urlInterfaceBug + "?path=" + path;

    $.get(url, function (html) {

        $("#myModalBug .modal-body").html(html);
        $("#myModalBug").modal('show');
    });

}

function checkAll(sender, checkName) {

    var objList = document.getElementsByName(checkName)
    for (var i = 0; i < objList.length; i++) {
        objList[i].checked = sender.checked;
    }
}
 
$(function () {

    //初始化提示信息
    var option = { animation: true, delay: 2000 };
    $('.toast').toast(option); 
    $('[data-toggle="tooltip"]').tooltip();

    //$("#myParamList").height(50);

    _SN = $("#txtSN").val();
});
