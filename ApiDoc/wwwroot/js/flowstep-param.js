
function loadStepParamData(fksn) {

    $.post(urlFlowStepParamList, { FKSN: fksn }, function (html) {

        drawStepParam(html);

    }); 
}

function drawStepParam(innerHtml ) {

    var fksn = selectFlowStep.SN; 
    $("#myStepParam_" + fksn).html(innerHtml);
    var dgv = $("#dgvSPList_" + fksn);
    dgv.xnTable();

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
        var json = $("#dgvSPList_" + fksn).xnTable("getSelection");
 
        if (json == null) {
            popToastWarning("请选择要修改的数据");
            return;
        }
       
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

    $.post(urlFlowStepParamSave, vdata, function (html) {

        $("#myStepParamModel").modal('hide'); 
        drawStepParam(html); 
    });

}

//弹出删除窗口
function showStepParamDelete() {

    var fksn = selectFlowStep.SN;
    var idsList = $("#dgvSPList_" + fksn).xnTable("getSelections", "SN");

    if (idsList.length == 0) {
        popToastWarning("请选择要删除的参数");
        return;
    }

    showModalDelete(btnDeleteStepParam);

}

//删除步骤参数
function btnDeleteStepParam() {
     
    var fksn = selectFlowStep.SN;
    var idsList = $("#dgvSPList_" + fksn).xnTable("getSelections", "SN"); 

    var data = { ids: idsList, FKSN: fksn };
    $.post(urlFlowStepParamDelete, data, function (innerHtml) { 
        drawStepParam(innerHtml); 
    });
}

//选择接口参数
function showSelectParam() {

    var fksn = selectFlowStep.FKSN; //接口SN
    var sn = selectFlowStep.SN;

    $.post(urlFlowStepInterParamList,
        {
            FKSN: fksn,
            StepSN: sn
        },
        function (innerHtml) {

            var dgvList = $("#myStepParamSelectModel #dgvList");
            var heigth = $(document).height();

            dgvList.height(heigth - 230);
            dgvList.html(innerHtml);
            
            $("#myStepParamSelectModel").modal("show");

    }); 
}

//保存步骤参数
function btnSaveStepParamInter() {
     
    var fksn = selectFlowStep.SN;
    var selList = document.getElementsByName("chkParam1_");
    var idsList = [];
    for (var i = 0; i < selList.length; i++) {
        if (selList[i].checked) {
            json = selList[i].value;
            json = eval("(" + json + ")");
            json.FKSN = fksn;
            json.SN = 0;
            idsList.push(json);
        }
    }

    if (idsList.length == 0) {
        popToastWarning("请选择要删除的参数");
        return;
    }

    $.post(urlFlowStepParamSaveList,
        { list: idsList },
        function (innerHtml) {
            $("#myStepParamSelectModel").modal("hide");
            drawStepParam(innerHtml); 
    });
}