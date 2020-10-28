var lstHisName = "dgvStepHis_";

function loadHisData(SN) { 
    $.post(urlFlowStepHisList, { FKSN: SN }, function (html) {
    
        drawStepHis(html);
    });
}

function drawStepHis(innerHtml) {

    var fksn = selectFlowStep.SN;
    $("#myHis_" + fksn).html(innerHtml);
    var dgv = $("#" + lstHisName + fksn);
    dgv.xnTable();

}


//弹出上传sql窗口
function showStepHis() {

    var fksn = selectFlowStep.SN;

    $("#txtStepHisSN").val(0);
    $("#txtStepHisFKSN").val(fksn);
    $("#txtFile").val("");

    $("#myStepHisModel").modal('show');

}

//上传
function btnAddHis_Click() {

    if (!(window.File || window.FileReader || window.FileList || window.Blob)) {
        popToastWarning("请换Chrome浏览器"); 
        return;
    }
     
    var vFKSN = $("#txtStepHisFKSN").val();
    var vFile = $("#txtFile").val();

    var files = $('input[name="fileTrans"]').prop('files');
    if (files.length == 0) {
        popToastWarning("请选择文件");  
        return;
    } else {
       
        if (!/.(sql)$/.test(vFile)) {
            popToastWarning("必须是sql文件");  
            return false; 
        }

        var reader = new FileReader();//新建一个FileReader
        reader.readAsText(files[0],"gb2312");//读取文件 
        reader.onload = function (evt) { //读取完文件之后会回来这里

            var fileString = evt.target.result; 
            var data = {
                SN: 0,
                FKSN: vFKSN,
                FileName: files[0].name,
                text: fileString,
                IsEnable: false
            }

            $.post(urlFlowStepHisAdd, data, function (result) {

                drawStepHis(result);

                $("#myStepHisModel").modal('hide');

            });
        }
    } 
}

//弹出删除窗口
function showStepHisDelete() {

    var fksn = selectFlowStep.SN;  
    var idsList = $("#" + lstHisName + fksn).xnTable("getSelections", "SN"); 

    if (idsList.length == 0) {
        $('#myDelete').toast('show');
        return;
    }  
    showModalDelete(btnDeleteStepHis_Click); 
}

//删除接口
function btnDeleteStepHis_Click() {
     
    var fksn = selectFlowStep.SN;
    var idsList = $("#" + lstHisName + fksn).xnTable("getSelections", "SN"); 
     
    if (idsList.length > 0) {
       
        var data = { ids: idsList, FKSN: fksn };
        $.post(urlFlowStepHisDelete, data
            , function (result) {
                drawStepHis(result);
            });
    }
}

//执行历史sql
function btnSmoExecute() {

    var fksn = selectFlowStep.SN;  //步骤SN 
    var selectionItem = $("#" + lstHisName + fksn).xnTable("getSelectionItem");  
    var json = $("#" + lstHisName + fksn).xnTable("getSelection","SN"); 
    if (json == null) {

        popToastWarning("请选择一条数据");
        return;
    } 

    var cmdText = selectionItem[0].attributes["data-text"].value; //历史sql变量 
    var idList = [];
    idList.push(json.SN);

    var data = { FKSN: fksn, ids: idList };

    $.post(urlFlowStepHisSmoExecute, data, function (result) {

        drawStepHis(result);

        //更新commandText内容
        var commandType = $("#tab_step_" + fksn + " [name = 'commandType']");
        if (commandType.val() == "Text") {

            var commandText = $("#tab_step_" + fksn + " [name = 'commandText']");
            commandText.val(cmdText);

        }
    });

}

//查询执行脚本
function showText() {

    var text = selectedHisText();
    var textarea = $("#myHisModalInfo textarea");

    var height = $(document).height();
    var rows = height / 32;

    textarea[0].attributes["rows"].value = rows;
    textarea.html(text); 
    $("#myHisModalInfo").modal("show");

}

function selectedHisText() {
 
    var fksn = selectFlowStep.SN;  //步骤SN 
    var selectionItem = $("#" + lstHisName + fksn).xnTable("getSelectionItem");
    if (selectionItem != null) {
        var cmdText = selectionItem[0].attributes["data-text"].value; //历史sql变量 

        return cmdText;
    }
    return "";
}