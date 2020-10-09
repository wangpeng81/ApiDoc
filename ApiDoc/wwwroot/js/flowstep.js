
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

//弹出主信息窗口
function btnShowFlowStep(FKSN) { 

    if (FKSN == 0) {

        //修改
        if (selectFlowStep != null) {
            $('#txtStepSN').val(selectFlowStep.SN);
            $('#txtStepName').val(selectFlowStep.StepName);
            $('#txtStepFKSN').val(selectFlowStep.FKSN);
            $('#txtStepOrder').val(selectFlowStep.StepOrder); 
            
        }
        else {
            alert("请选择步骤数据");
            return;
        }
    }
    else { //添加

        $('#txtStepSN').val(0);
        $('#txtStepName').val(""); 
        $('#txtStepFKSN').val(FKSN);
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
        function (data) {
            $("#myStep").modal('hide');
            $("#myStepList").html(data);
        });

}

//弹出删除步骤提示
function btnShowFlowStepDelete() {
    if (selectFlowStep != null) {
        $("#myModalDelete").modal('show');
    }
}

//删除步骤
function btnSaveFlowStepDelete() {
    if (selectFlowStep != null) {
        $.post(urlFlowStepDelete, selectFlowStep,
            function (data) {
                $("#myModalDelete").modal('hide');
                $("#myStepList").html(data);
            });
    }
}

//保存SQL
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
                $('#mySuccess').toast('show');
            }
           
        });
}
 