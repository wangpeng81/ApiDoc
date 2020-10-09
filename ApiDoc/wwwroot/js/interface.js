

//import { data } from "jquery";


function openwindow(url, name, iWidth, iHeight) {
    var url = "~/Interface/Index?"
    var name;                           //网页名称，可为空;
    var iWidth;                          //弹出窗口的宽度;
    var iHeight;                        //弹出窗口的高度;
    var width = window.screen.availWidth;
    var height = window.screen.availHeight;

    var iTop = (height - iHeight) / 2 - 30;       //获得窗口的垂直位置;
    var iLeft = (width - iWidth) / 2;           //获得窗口的水平位置;
    window.open(url, name, 'height=' + iHeight + ',i 3nnerHeight=' + iHeight + ',width=' + iWidth + ',innerWidth=' + iWidth + ',top=' + iTop + ',left=' + iLeft + ',toolbar=no,menubar=no,scrollbars=auto,resizeable=no,location=no,status=no,titlebar=no', true );
}


function btnSearch_Click(fksn)
{
    var vTitle = $('#txtTitle').val();
    var vUrl = $('#txtUrl').val(); 
    var url = urlRoot + "/Interface/Index?";
    window.location = url + "title=" + vTitle + "&Url=" + vUrl + "&FKSN=" + fksn;

}

//添加接口基础信息
function btnAdd_Click(fksn) {
 
    //var nodeID = window.parent.genID(3);
    //var closableTab = window.parent.closableTab;
    //var tabItem = { id: nodeID, title: "添加接口", url: "Interface/Add?fksn=" + fksn }; 
    //closableTab.addTab(tabItem);
    window.parent.showInterface(0, fksn);
    
}

function btnUpdate_Click() {
     
    var list = $('input[name="chk"]:checked').val();

    if (list != undefined) {
        var array = list.split(',');
        if (array.length > 0) {
            var id = array[0];

            //var nodeID = window.parent.genID(3);
            //var closableTab = window.parent.closableTab;
            //var tabItem = { id: nodeID, title: "修改接口", url: window.location + "/Interface/Add?SN=" + id };

            window.parent.showInterface(id,0);
             
        }
    }
   
}

//删除步骤接口
function btnDelete_Click() { 

    var model = $("#myModalDelete");
    model.modal('show');

}
function DeleteHandler(fksn) {

    var list1 = document.getElementsByName('chk');
    for (var i = 0; i < list1.length; i++) {
        var value = list1[i];
    }

    var list = "";
    var arrayList = [];

    $.each($('input[name="chk"]:checked'), function () {

        arrayList.push($(this).val()) 
    });

    if (arrayList.length > 0) {
        var array = list.split(',');

        var data = { "ids": arrayList };
        $.post(urlInterfaceDelete, data
            , function (data) { 
                btnSearch_Click(fksn);
            })
    }
}

//选择步骤项
function OnCollapse_Click(id)
{
    //$(id).rotate({ animateTo: 180 });
    var obj = $(id);
    obj.removeClass("fa");
    obj.removeClass("fa-angle-double-right");

}

//保存接口
function btnSave_Click() {

    var objSN = $("#txtSN");
    var vSN = objSN.val();
    var vTitle = $("#txtTitle").val();
    var vUrl = $("#txtUrl").val();
    var vSerializeType = $("#cbxSerializeType").val();
    var vMethod = $("#cbxMethod").val();
    var vFKSN = $("#txtFKSN").val();
    var vExecuteType = $("#cbxExecuteType").val();
    var vIsTransaction = document.getElementById("txtStepIsTransaction").checked;

    $.post(urlInterfaceSave, {
        SN: vSN,
        Title: vTitle,
        Url: vUrl,
        Method: vMethod,
        FKSN: vFKSN,
        SerializeType: vSerializeType,
        IsTransaction: vIsTransaction,
        ExecuteType: vExecuteType
    }, function (data) {

            $('#mySuccess').toast('show');
            $("#txtSN").val(data.sn);
    })
}

//上传接口到路由
function btnUpLoad_Click() {

    var SN = $("#txtSN").val();
    var url = urlRouteUpLoad + "?SN=" + SN;
    $.get(url, function (myResponse) {

        if (myResponse.dataType == 0) {
            //成功
            alert("上传成功");
        }
        else {
            alert( myResponse.exception );
        }
    });
}

//删除接口路由信息
function btnDownLoad_Click() {
 
    var vUrl = $("#txtUrl").val();
    var url = urlRouteDelete + "?Url=" + vUrl;
    $.get(url, function (myResponse) {

        if (myResponse.dataType == 0) {
            //成功
            alert("删除成功");
        }
        else {
            alert(myResponse.exception);
        }
    });

}

//弹出测试窗口
function btnShow_CS_Click() {

    $("#txtResult").val("");
    var model = $("#myModalCS");
    model.modal('show');
        
}

//测试
function btnSendCS() {

    var url = window.location.protocol + "//" + window.location.host + $("#txtUrl").val();
    var txtInput = $("#txtInput").val();
    var txtResult = $("#txtResult");
    var method = $("#cbxMethod").val();
  
    if (method == "Post") {
       
        var data = $.parseJSON(txtInput);
        $.post(url, data, function (result) {
            txtResult.val(result);
        });

    }
    else if (method == "Get") {

        url = url + "?" + txtInput;
        $.get(url, function (result) {
            txtResult.val(result);
        });

    }
}

$(function () {
    var option = { animation: true, delay: 1500 };
    $('.toast').toast(option);
});


