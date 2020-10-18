
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
             
        }); 
})


//选择行
function onStepSelect_Click(json) {
    selectFlowStep = json;
}

//选择步骤项，加载历史sql数据
function OnCollapse_Click(id, sn) {

    var accordion = $('#' + id);

    accordion.on('show.bs.collapse', function (event, data) {

        var id = "myHis_" + sn;
        var data = { FKSN: sn};

        $.post(urlFlowStepHisList, data, function (result) {

            $('#myHis_' + sn).html(result);

        });
        
    });

    accordion.collapse('show');

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
            $('#myToastUpdate').toast("show"); 
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
            selectFlowStep = null;
        });

}

//弹出删除步骤提示
function btnShowFlowStepDelete() {
    if (selectFlowStep != null) {
        $("#myModalDelete").modal('show');
    }
    else {
        $("#myToastDelete").toast('show'); 
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
    var cmdDataBase = $('#cbxDataBase').val();
    var data = {
        SN: SN,
        CommandType: cmdType,
        CommandText: cmdText,
        DataBase: cmdDataBase
    };

    $.post(urlFlowStepSaveCmdText, data,
        function (data) {

            if (data > 0) {
                $('#mySuccess').toast('show');
            }

        });
}
 