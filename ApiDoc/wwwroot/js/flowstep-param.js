
function loadStepParamData(fksn) {

    $.post(urlFlowStepParamList, { FKSN: fksn }, function (html) {
        $("#myStepParam_" + fksn).html(html);
    });

}

//弹出步骤参数维护窗口
function showStepParam(sn) {

    var txtStepParamSN = $('#txtStepParamSN');
    var txtStepParamName = $('#txtStepParamName');
    var txtStepParamRemark = $('#txtStepParamRemark');
    var cbxStepParamDataType = $('#cbxStepParamDataType');
    var txtStepParamDefault = $('#txtStepParamDefault');
    var chkIsPreStep = document.getElementById("chkIsPreStep");

    if (sn > 0) {

        var fksn = selectFlowStep.SN;
        var selList = document.getElementsByName("myStepParam_" + fksn);
        var json = null;
        for (var i = 0; i < selList.length; i++) {
            var selected = selList[i];
            if (selected.checked) {
                json = selected.value;
                break;
            }
        }

        if (json == null) {
            popToastWarning("请选择要修改的数据");
            return;
        }
        json = eval("(" + json + ")");

        txtStepParamSN.val(json.SN);
        txtStepParamName.val(json.ParamName);
        txtStepParamRemark.val(json.Remark);
        cbxStepParamDataType.val(json.DataType);
        txtStepParamDefault.val(json.DefaultValue);
        chkIsPreStep.checked = json.IsPreStep == "True" ? true: false;

    }
    else {

        txtStepParamSN.val(0);
        txtStepParamName.val("");
        txtStepParamRemark.val("");
        cbxStepParamDataType.val("");
        txtStepParamDefault.val("");
        chkIsPreStep.checked = true;
            
    }

    $("#myStepParamModel").modal('show');
     
}

//保存步骤参数
function btnSaveStepParam_Click() {

    var fksn = selectFlowStep.SN;
    var vSN = $('#txtStepParamSN').val(); 
    var vParamName = $('#txtStepParamName').val();
    var vRemark = $('#txtStepParamRemark').val();
    var vDataType = $('#cbxStepParamDataType').val();
    var vDefault = $('#txtStepParamDefault').val();
    var vIsPreStep = document.getElementById("chkIsPreStep").checked;

    var vdata = {
        SN: vSN,
        FKSN: fksn,
        ParamName: vParamName,
        DataType: vDataType,
        DefaultValue: vDefault,
        Remark: vRemark,
        IsPreStep: vIsPreStep
    };

    $.post(urlFlowStepParamSave, vdata, function (result) {

        $("#myStepParamModel").modal('hide');
        $("#myStepParam_" + fksn).html(result);
         
    });

}

//弹出删除窗口
function showStepParamDelete() {

    showModalDelete(btnDeleteStepParam);

}

//删除步骤参数
function btnDeleteStepParam() {

    var fksn = selectFlowStep.SN;
    var selList = document.getElementsByName("myStepParam_" + fksn );
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
 
    var data = { ids: idsList, FKSN: fksn };
    $.post(urlFlowStepParamDelete, data, function (innerHtml) {
        $("#myStepParam_" + fksn).html(innerHtml);
    });
}