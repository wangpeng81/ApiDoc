
var selectFlowStep = null;

$(function () {
     
    var fksn = $('#txtSN').val(); 
    var data = {  
        FKSN: fksn 
    };

    //加载执行步骤
    $.post(urlFlowStepList, data, loadStepData); 
})

//加载步骤信息
function loadStepData(innerHtml) {

    $("#myStepList").html(innerHtml); 

    var tabSteps = $("#pills-tab-steps");
    tabSteps.on("shown.bs.tab", function (e) {

        var value = e.target.attributes["data-data"].nodeValue;
        selectFlowStep = eval("(" + value + ")");

        var isload = e.target.attributes["data-isload"].value;
        if (isload == "0") {

            //如果没有加载历史sql
            loadHisData(selectFlowStep.SN);

            //加载步骤参数
            loadStepParamData(selectFlowStep.SN);

            e.target.attributes["data-isload"].value = "1";
        }
    });

    //显示第一步 
    var tabFirst = $('#pills-tab-steps li:first-child a');
    tabFirst.tab('show');
}

//弹出主信息窗口
function showFlowStep(flag) { 
     
    if (flag == 1) {

        //修改
        if (selectFlowStep != null) {
             
            $('#txtStepSN').val(selectFlowStep.SN);
            $('#txtStepName').val(selectFlowStep.StepName);
            $('#txtStepFKSN').val(selectFlowStep.FKSN);
            $('#txtStepOrder').val(selectFlowStep.StepOrder); 
            
        }
        else { 
            popToastWarning("请选择要修改的步骤数据");

            return;
        }
    }
    else { //添加

        if (_SN == 0) {
            popToastSuccess("请先维护接口信息");
            return;
        }
        $('#txtStepSN').val(0);
        $('#txtStepName').val("");
        $('#txtStepFKSN').val(_SN);
        $('#txtStepOrder').val(1); 
    }

    $("#myStep").modal('show');
}

//保存主信息窗口
function btnSaveFlowStep() {

    var vSN = $('#txtStepSN').val();
    var vStepName = $('#txtStepName').val();
    var fksn = $('#txtStepFKSN').val();
    var vStepOrder = $('#txtStepOrder').val(); 

    var data = {
        SN: vSN,
        StepName: vStepName,
        FKSN: fksn,
        StepOrder: vStepOrder  
    };
    $.post(urlFlowStepSave, data ,
        function (innerHtml) {
            $("#myStep").modal('hide');
            loadStepData(innerHtml);
        });

}

//弹出删除步骤提示
function btnShowFlowStepDelete() {
    if (selectFlowStep != null) { 
        showModalDelete(btnSaveFlowStepDelete);
    }
    else { 
        popToastWarning("请选要删除的步骤"); 
    }
}

//删除步骤
function btnSaveFlowStepDelete() {
    if (selectFlowStep != null) {
        $.post(urlFlowStepDelete, selectFlowStep,
            function (innerHtml) {
                $("#myModalDelete").modal('hide');
                loadStepData(innerHtml);
            });
    }
}

//保存SQL
function btnSaveCmdText(SN) {

    var stepID = "#tab_step_" + SN;
     
    var cmdType = $(stepID + ' #cbxCommandType').val();
    var cmdText = $(stepID + ' #txtCommandText').val();
    var cmdDataBase = $(stepID + ' #cbxDataBase').val();
    var vServiceName = $(stepID + ' #cbxServiceName').val();
    var data = {
        SN: SN,
        CommandType: cmdType,
        CommandText: cmdText,
        DataBase: cmdDataBase,
        ServiceName: vServiceName
    };

    $.post(urlFlowStepSaveCmdText, data,
        function (data) {

            if (data > 0) {
                popToastSuccess("保存成功!");
            }

        });
}
