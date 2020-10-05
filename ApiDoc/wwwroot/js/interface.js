var urlSave = "~/Interface/Save";
var url = "~/Interface/Index?"
var urlDelete = "~/Interface/Delete";


function openwindow(url, name, iWidth, iHeight) {
    var url;                                 //转向网页的地址;
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
    window.location = url + "title=" + vTitle + "&Url=" + vUrl + "&FKSN=" + fksn;

}
function btnAdd_Click(fksn) {

    //var height = 600;
    //var url1 = "Add?fksn=" + fksn;
    //var width = window.screen.availWidth - 40;
    //var height = window.screen.availHeight - 80;

    //if (window.showModalDialog == undefined) {
       
    //    openwindow(url1, '_blank', width, height);

    //    //window.open(url1, '_blank', 'channelmode=yes,directories=no,fullscreen=true,location=0,menubar=no,status=no,top=0,left=0');
    //}
    //else {
    //    var rv = window.showModalDialog(url1, width, height, "", "", "", "", false);
    //}

    var nodeID = window.parent.genID(3);
    var closableTab = window.parent.closableTab;
    var tabItem = { id: nodeID, title: "添加接口", url: "Interface/Add?fksn=" + fksn };

    closableTab.addTab(tabItem);

}

function btnUpdate_Click() {
     
    var list = $('input[name="chk"]:checked').val();

    if (list != undefined) {
        var array = list.split(',');
        if (array.length > 0) {
            var id = array[0];

            var nodeID = window.parent.genID(3);
            var closableTab = window.parent.closableTab;
            var tabItem = { id: nodeID, title: "修改接口", url: "Interface/Add?SN=" + id };

            closableTab.addTab(tabItem);
        }
    }
   
}

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
        $.post(urlDelete, data
            , function (data) { 
                btnSearch_Click(fksn);
            })
    }
}


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
    var vReturnType = $("#cbxReturnType").val();
    var vMethod = $("#cbxMethod").val();
    var vFKSN = $("#txtFKSN").val();
    $.post(urlSave, {
        SN: vSN,
        Title: vTitle,
        Url: vUrl,
        Method: vMethod,
        FKSN: vFKSN,
        ReturnType: vReturnType
    }, function (data) {

            $('#mySuccess').toast('show');
            $("#txtSN").val(data.sn);
    })
}

$(function () {
    var option = { animation: true, delay: 1500 };
    $('.toast').toast(option);
});


