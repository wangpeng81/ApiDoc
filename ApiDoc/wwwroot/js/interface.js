
//保存接口
function btnSaveIntterface_Click() {

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

            $('#myToastSuccess').toast('show');
            $("#txtSN").val(data.sn);
    })
}

//上传接口到路由
function btnUpLoad_Click() {

    var SN = $("#txtSN").val();
    var url = urlRouteUpLoad + "?SN=" + SN;
    $.get(url, function (myResponse) {

        if (myResponse.dataType == 0) {
            
            $("#myToastUpLoad").toast("show");

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

    var method = $("#cbxMethod").val();

    var paraList = document.getElementsByName("chkParam_");
    var csJson = ""; 
    for (var i = 0; i < paraList.length; i++) { 
        json = paraList[i].value;
        json = eval("(" + json + ")"); 
         
        if (method == "Get") {
            if (csJson != "") {
                csJson += "&";
            }
            if (json.DataType == "varchar") {
                csJson += json.ParamName + "= '" + json.DefaultValue + "'";
            }
            else if (json.DataType == "int") {
                csJson += json.ParamName + "= " + json.DefaultValue;
            }
            else {
                csJson += json.ParamName + "= '" + json.DefaultValue + "'";
            }
        }
        else if (method == "Post") {
            if (csJson != "") {
                csJson += ",";
            }
            csJson += json.ParamName + ": '" + json.DefaultValue + "'"
        }
    }

    if (method == "Post")
    {
        csJson = "{" + csJson + "}";
    }

    $("#txtInput").val(csJson); 
    $("#txtResult").val("");
    var model = $("#myModalCS");
    model.modal('show');
        
}

//--------------------------------------------------测试
function btnSendCS() {
 
    var url = window.location.protocol + "//" + window.location.host + urlRoot + $("#txtUrl").val();
    var txtInput = $("#txtInput").val();
    var txtResult = $("#txtResult");
    var method = $("#cbxMethod").val();
  
    if (method == "Post") { 
        var vdata = txtInput; 
        $.ajax({
            url: url,
            type: "POST",
            datType: "JSON",
            contentType: "application/json",
            data: vdata,
            async: false,
            success: function (result) {
                txtResult.val(result);
            }
        })
    }
    else if (method == "Get") {

        if (txtInput != "") {
            url = url + "?" + txtInput; 
        } 
        $.get(url, function (result) {
            txtResult.val(result);
        });

    }
}

$(function () {
    var option = { animation: true, delay: 1500 };
    $('.toast').toast(option); 
    $('[data-toggle="tooltip"]').tooltip()
});


