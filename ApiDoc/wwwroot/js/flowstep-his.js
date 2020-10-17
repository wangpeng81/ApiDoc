 
//弹出上传sql窗口
function showStepHis(fksn) {

    $("#txtStepHisSN").val(0);
    $("#txtStepHisFKSN").val(fksn);
    $("#txtFile").val("");

    $("#myStepHisModel").modal('show');

}

//上传
function btnAddHis_Click() {

    if (!(window.File || window.FileReader || window.FileList || window.Blob)) {
        alert('换Chrome浏览器啦');
        return;
    }
     
    var vFKSN = $("#txtStepHisFKSN").val();
    var vFile = $("#txtFile").val();

    var files = $('input[name="fileTrans"]').prop('files');
    if (files.length == 0) {
        alert('请选择文件');
        return;
    } else {
       
        if (!/.(sql)$/.test(vFile)) {

            alert("必须是sql文件");
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
function showStepHisDelete(FKSN) {

    var arrayList = [];
    var list = document.getElementsByName('chkHis_' + FKSN);
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
    document.getElementById("txtStepHisFKSN").value = FKSN;
    //$("#txtStepHisFKSN").val(FKSN); 
    $("#myHisModalDelete").modal("show");

}

//删除接口
function btnDeleteStepHis_Click() {

    var vFKSN = $("#txtStepHisFKSN").val();
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
            })
    }
}

function btnSmoExecute(fksn) {
 
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
        return;
    }

    var data = { FKSN: fksn, ids: arrayList }; 
    if (arrayList.length > 0) {
        $.post(urlFlowStepHisSmoExecute, data, function (result) {

            var html = $("#myHis_" + fksn);
            html.html(result); 

        });
    }
    
}