//**************************************************
//接口参数
//**************************************************

var dgvParam = "#dgvParam";

$(function () {
      
    loadParam();

});
 
//加载参数数据
function loadParam() {

    var fksn = $('#txtSN').val();
    var data = { FKSN: fksn };
    $.post(urlParamList, data, function (innerHtml) {
        drawParam(innerHtml);
    });
}

function drawParam(innerHtml) {

    $("#myParamList").html(innerHtml);
    var dgv = $(dgvParam);
    dgv.xnTable();
}

//查询参数
function btnSeachParam_Click()
{
    loadParam();
}

//弹出添加窗口
function showParamWin(sn) {
     
    if (sn > 0) {
        
        var json = $(dgvParam).xnTable("getSelection"); 
        if (json == null) {
            popToastWarning("请选择要修改的数据");
            return;
        }
         
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

    $.post(urlParamAdd, vdata, function (innerHtml) {
        $("#myParamModel").modal('hide');
        drawParam(innerHtml); 
    });
}

//弹出参数删除窗口
function showParamDelete() {
 
    var idsList = $(dgvParam).xnTable("getSelections", "SN"); 
    if (idsList.length == 0) {
        popToastWarning("请选择要删除的参数"); 
        return;
    }

    showModalDelete(btnDeleteParam_Click);
}

//删除参数
function btnDeleteParam_Click() {

    var idsList = $(dgvParam).xnTable("getSelections", "SN");  
    var vfksn = $('#txtSN').val();

    var data = { ids: idsList, FKSN: vfksn };
    $.post(urlParamDelete, data, function (innerHtml) {
        drawParam(innerHtml); 
    });
}
 