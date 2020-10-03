var urlFlowStepSave = "/Interface/StepSave";
var urlFlowStepList = "/Interface/FlowStepList";
var urlFlowStepDelete = "/Interface/DeleteFlowStep";
var urlFlowStepSaveCmdText = "/Interface/SaveCmdText";

var selectFlowStep = null;

$(function () {
   
    var fksn = $('#txtSN').val();
    var data = {  
        FKSN: fksn 
    };

    //加载执行步骤
    $.post(urlFlowStepList, data,
        function (data) {
            $("#myStepList").html(data);

            //加载方法数据
            InitAction();

        });
})

//选择行
function onStepClick(json) {
    selectFlowStep = json;
}

function btnShowFlowStep(FKSN) { 

    if (FKSN == 0) {

        //修改
        if (selectFlowStep != null) {
            $('#txtStepSN').val(selectFlowStep.SN);
            $('#txtStepName').val(selectFlowStep.StepName);
            $('#txtStepFKSN').val(selectFlowStep.FKSN);
            $('#txtStepOrder').val(selectFlowStep.StepOrder); 
            $('#txtStepIsTransaction').attr('checked', selectFlowStep.IsTransaction);
        }
        else {
            return;
        }
    }
    else {
        $('#txtStepSN').val(0);
        $('#txtStepName').val(""); 
        $('#txtStepFKSN').val(FKSN);
        $('#txtStepOrder').val(1);
        $('#txtStepIsTransaction').attr('checked', true);
    }

    $("#myStep").modal('show');
}

function btnSaveFlowStep() {

    var vSN = $('#txtStepSN').val();
    var vStepName = $('#txtStepName').val();
    var fksn = $('#txtStepFKSN').val();
    var vStepOrder = $('#txtStepOrder').val();
    var vIsTransaction = document.getElementById("txtStepIsTransaction").checked;

    var data = {
        SN: vSN,
        StepName: vStepName,
        FKSN: fksn,
        StepOrder: vStepOrder,
        IsTransaction: vIsTransaction
    };
    $.post(urlFlowStepSave, data ,
        function (data) {
            $("#myStep").modal('hide');
            $("#myStepList").html(data);
        });

}

//------------------------------------删除
function btnShowFlowStepDelete() {
    if (selectFlowStep != null) {
        $("#myModalDelete").modal('show');
    }
}

function btnSaveFlowStepDelete() {
    if (selectFlowStep != null) {
        $.post(urlFlowStepDelete, selectFlowStep,
            function (data) {
                $("#myModalDelete").modal('hide');
                $("#myStepList").html(data);
            });
    }
}

//--------------------------------------------------保存SQL
function btnSaveCmdText(SN) {

    var cbx = $('#cbxCommandType');
    var cmdType = $('#cbxCommandType').val();
    var cmdText = $('#txtCommandText').val();
 
    var data = {
        SN: SN, 
        CommandType: cmdType,
        CommandText: cmdText
    };

    $.post(urlFlowStepSaveCmdText, data,
        function (data) {

            if (data > 0) {
                alert("保存成功!")
            }
           
        });
}
 