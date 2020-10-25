$(function () {
      
    loadParam();

});
 
//加载参数数据
function loadParam() {

    var fksn = $('#txtSN').val();
    var data = { FKSN: fksn };
    $.post(urlParamList, data, function (result) {
        $("#myParamList").html(result);
    });
}

//查询参数
function btnSeachParam_Click()
{
    loadParam();
}

//弹出添加窗口
function showParamWin(sn) {
     
    if (sn > 0) {
        var selList = document.getElementsByName("chkParam_");
        var json = null;
        for (var i = 0; i < selList.length; i++) {
            if (selList[i].checked) {
               json = selList[i].value;
            }
        }

        if (json == null) {
            popToastWarning("请选择要修改的数据");
            return;
        }
        json = eval("(" + json + ")");

        $('#txtParamSN').val(json.SN);
        $('#txtParamName').val(json.ParamName);
        $('#txtParamRemark').val(json.Remark);
        $('#cbxParamDataType').val(json.DataType);
        $('#txtParamDefault').val(json.DefaultValue);
         
    }
    else {
         
        $('#txtParamSN').val(0); 
        $('#txtParamName').val(""); 
        $('#txtParamRemark').val("");
        $('#cbxParamDataType').val("");
        $('#txtParamDefault').val(""); 

    }

    $("#myParamModel").modal('show');

}

//保存参数
function btnSaveParam_Click() {

    var vSN = $('#txtParamSN').val();
    var vFksn = $('#txtSN').val();
    var vParamName = $('#txtParamName').val();
    var vRemark = $('#txtParamRemark').val();
    var vDataType = $('#cbxParamDataType').val();
    var vDefault = $('#txtParamDefault').val();

    var vdata = {
        SN: vSN,
        FKSN: vFksn,
        ParamName: vParamName,
        DataType: vDataType,
        DefaultValue: vDefault,
        Remark: vRemark
    };

    $.post(urlParamAdd, vdata , function (result) {
        $("#myParamModel").modal('hide');
        $("#myParamList").html(result);
    });
}

//弹出参数删除窗口
function showParamDelete() {

    var selList = document.getElementsByName("chkParam_");
    var ids = [];
    for (var i = 0; i < selList.length; i++) {
        if (selList[i].checked) {
            json = selList[i].value;
            json = eval("(" + json + ")");
            ids.push(json.SN);
        }
    }

    if (ids.length == 0) {
        popToastWarning("请选择要删除的参数"); 
        return;
    }

    showModalDelete(btnDeleteParam_Click);
}

//删除参数
function btnDeleteParam_Click() {

    var selList = document.getElementsByName("chkParam_");
    var idsList = [];
    for (var i = 0; i < selList.length; i++) {
        if (selList[i].checked) {
            json = selList[i].value;
            json = eval("(" + json + ")");
            idsList.push(json.SN);
        }
    }

    if (idsList.length == 0) {
        popToastWarning("请选择要删除的参数"); 
        return;
    }

    var vfksn = $('#txtSN').val();

    var data = { ids: idsList, FKSN: vfksn };
    $.post(urlParamDelete, data, function (result) {
        $("#myParamList").html(result);
    });
}
 