

function loadHisData(SN) { 
    $.post(urlFlowStepHisList, { FKSN: SN }, function (html) {
        $("#myHis_" + SN).html(html);
    });
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

                $("#myHis_" + vFKSN).html(result);
                $("#myStepHisModel").modal('hide');

            });
        }
    } 
}

//弹出删除窗口
function showStepHisDelete() {

    var fksn = selectFlowStep.SN; 
    var arrayList = [];
    var list = document.getElementsByName('chkHis_' + fksn);
    for (var i = 0; i < list.length; i++) {
        var checked = list[i].checked;
        var value = list[i].value;
        if (checked) {
            arrayList.push(value);
        }
    };

    if (arrayList.length == 0) {
        $('#myDelete').toast('show');
        return;
    }  
    $("#myHisModalDelete").modal("show");

}

//删除接口
function btnDeleteStepHis_Click() {

    var vFKSN = selectFlowStep.SN; 
    var arrayList = [];

    var list = document.getElementsByName('chkHis_' + vFKSN);
    for (var i = 0; i < list.length; i++) {
        var checked = list[i].checked;
        var value = list[i].value;
        if (checked) {
            arrayList.push(value);
        }
    }
   
    if (arrayList.length > 0) {
       
        var data = { ids: arrayList, FKSN: vFKSN };
        $.post(urlFlowStepHisDelete, data
            , function (data) {
                $("#myHis_" + vFKSN).html(data);
            });
    }
}

//执行历史sql
function btnSmoExecute() {

    var fksn = selectFlowStep.SN;  //步骤SN
    var cmdText; //历史sql变量 
    var idList = []; //历史 SN
    var list = document.getElementsByName('chkHis_' + fksn);
    for (var i = 0; i < list.length; i++) {
        var chk = list[i];
        var checked = chk.checked;
        if (checked) {
            cmdText = chk.attributes["data-text"].value;
            idList.push(chk.value);
        }
    };

    if (idList.length != 1) {

        popToastWarning("请选择一条数据");
        return;
    }

    var data = { FKSN: fksn, ids: idList };

    $.post(urlFlowStepHisSmoExecute, data, function (result) {

        var html = $("#myHis_" + fksn);
        html.html(result);

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

    var vFKSN = selectFlowStep.SN; 
    var list = document.getElementsByName('chkHis_' + vFKSN); 
    var cmdText; //历史sql变量 
    for (var i = 0; i < list.length; i++) {
        var chk = list[i]; 
        var checked = chk.checked; 
        if (checked) {
            cmdText = chk.attributes["data-text"].value;
        }
    }

    return cmdText;
}