var dgvInterface = "dgvInterface";

function checkAll(sender, checkName) {

    var objList = document.getElementsByName(checkName)
    for (var i = 0; i < objList.length; i++) {
        objList[i].checked = sender.checked;
    }
}

function btnSearch_Click() {
    var fksn = $("#txtFKSN").val();
    var vTitle = $('#txtTitle').val();
    var vUrl = $('#txtUrl').val();
    var url = urlRoot + "/Interface/Index?";
    window.location = url + "title=" + vTitle + "&Url=" + vUrl + "&FKSN=" + fksn;

}


//弹出添加接口窗口
function btnShowAdd_Click() {

    var fksn = $("#txtFKSN").val();

    window.parent.showInterface(0, fksn);

}

//弹出添加接口窗口
function btnShowUpdate_Click() {

    var json = $("#" + dgvInterface).xnTable("getSelection");

    if (json != null) {
        　
        window.parent.showInterface(json.SN, 0);
        //}
    }
    else {
        $("#mySelect").toast("show");
    }
}


//弹出删除提示
function btnShowDeleteInter_Click() {
     
    var idsList = $("#" + dgvInterface).xnTable("getSelections", "SN");
    if (idsList.length == 0) {
        popToastWarning("请选择要删除的参数");
        return;
    }

    showModalDelete(btnDeleteIntterface_Click);

}

//删除接口
function btnDeleteIntterface_Click() {

    //var arrayList = [];
    //var list1 = document.getElementsByName('chk');
    //for (var i = 0; i < list1.length; i++) {
    //    var checked = list1[i].checked;
    //    var value = list1[i].value;
    //    if (checked) {
    //        arrayList.push(value);
    //    }
    //}

    var arrayList = $("#" + dgvInterface).xnTable("getSelections", "SN");  
    if (arrayList.length > 0) {

        var data = { "ids": arrayList };
        $.post(urlInterfaceDelete, data
            , function (data) {
                btnSearch_Click();
            })
    }
}

$(function () {

    var dgv = $("#" + dgvInterface);
    dgv.xnTable();

});
