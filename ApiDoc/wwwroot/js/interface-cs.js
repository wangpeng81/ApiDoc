 
//弹出测试窗口
function showCS() {

    //var url = window.location.protocol + "//" + window.location.host + $("#txtUrl").val();
    var url = urlRoot + $("#txtUrl").val();
    $("#txtCSUrl").val(url);

    var paraList = document.getElementsByName("chkParam_");

    var txtCSContentType = $("#txtCSContentType"); 
    txtCSContentType.val("application/json");
    for (var i = 0; i < paraList.length; i++) {
        var json = paraList[i].value;
        json = eval("(" + json + ")");
        if (json.DataType == "Image") { 
            txtCSContentType.val("multipart/form-data");
            break;
        }
    }

    $("#txtResult").val("");
    $("#txtAjax").val("");
    $("#txtJsonResult").val("");
    $("#dgvCS").html("");

    url = urlParamGetCSParam + "?SN=" + _SN;
    $.get(url, function (innerHtml) {

        var dgvCSParam = $("#dgvCSParam");
        dgvCSParam.html(innerHtml);

        CreateAjaxHtml();

        $('#pills-cs-tab li:first-child a').tab('show');

        var model = $("#myModalCS");
        model.modal('show');
    });
}

//--------------------------------------------------测试
function btnSendCS() {
 
    var strUserName = $("#myModalCS #txtUserName").val();
    var strPassword = $("#myModalCS #txtPassword").val();

    if (strUserName == "") {
        return;
    }
    if (strPassword == "") {
        return;
    }

    var data = {
        UserName: strUserName,
        Password: strPassword
    };

    $.ajax({
        url: urlAuthorLogin,
        dataType: 'json',
        type: 'Post',
        contentType: 'application/json',
        data: JSON.stringify(data),
        success: function (author) {
            CSCallBack(author);
        }
    })
}

function CSCallBack(author) {

    var version = ""; //是否返回Layui的数据
    
    var chkVersion = document.getElementById("chkVersion");
    if (chkVersion.checked) {
        version = chkVersion.value;
    }
    var method = $("#cbxMethod").val();
    var ContentType = $("#txtCSContentType").val(); 
    if (author.result == true) {

        var cbxExecuteType = $("#cbxExecuteType"); //结果集类型 
        var url = $("#txtCSUrl").val(); //地址  
        var vData = CreateData(method, ContentType); 
        if (ContentType == "multipart/form-data") {
            $.ajax({
                beforeSend: function (xhr) {
                    xhr.setRequestHeader("Authorization", "Bearer " + author.token);
                    xhr.setRequestHeader("Version", version);
                },
                type: "POST",
                url: url, 
                type: "post",
                cache: false,
                contentType: false,
                processData: false,
                data: vData,
                success: function (result) { 
                    DrawTable(result);
                },
                error: function () {

                }
            });
        }
        else {  
            var method = $("#cbxMethod").val();
            if (method == "Get") {
                if (vData != "") {
                    url = url + "?" + vData;
                }

                $.ajax({
                    url: url,
                    beforeSend: function (xhr) {
                        xhr.setRequestHeader("Authorization", "Bearer " + author.token);
                        xhr.setRequestHeader("Version", version);
                    },
                    type: method,
                    contentType: "application/json", 
                    success: function (result) {
                        DrawTable(result);
                    }
                })
               
            } 
            else {
               
                $.ajax({
                    url: url,
                    beforeSend: function (xhr) {
                        xhr.setRequestHeader("Authorization", "Bearer " + author.token);
                        xhr.setRequestHeader("Version", version);
                    },
                    type: method,
                    contentType: "application/json",
                    data: JSON.stringify( vData ),
                    success: function (result) {
                        DrawTable(result);
                    }
                })
            } 
        }
    }

}

function DrawTable(result) {

    var txtJsonResult = $("#txtJsonResult"); 
    txtJsonResult.val(result);

    var cbxExecuteType = $("#cbxExecuteType");
    var ExecuteType = cbxExecuteType.val();
    if (ExecuteType == "DataSet") {

        var dgvCS = $("#dgvCS");
        var html = "<table class='table table-sm'>";
        var serializeType = $("#cbxSerializeType").val();

        var chkVersion = document.getElementById("chkVersion");

        if (serializeType == "Json") {

            var model = eval("(" + result + ")"); 
            var length = 0;
            if (chkVersion.checked) {

                length = model.count;
                if (length > 0) {

                    //列
                    html += "<tr>"; 
                    var row = model.data[0];
                    $.each(row, function (key) {
                        html += "<th class='bg-light'>";
                        html += key;
                        html += "</th>";
                    });
                    html += "</tr>";

                    //行
                    for (var i = 0; i < length; i++) {

                        html += "<tr>"; 
                        var row = model.data[i];
                        $.each(row, function (key) {
                            html += "<td>";
                            html += row[key];
                            html += "</td>";
                        });

                        html += "</tr>";
                    }
                }
            }
            else {

                length = model.Result.Table.length;

                //列 
                if (length > 0) {
                    html += "<tr>"; 
                    var row = model.Result.Table[0];
                    $.each(row, function (key) {
                        html += "<th class='bg-light'>";
                        html += key;
                        html += "</th>";
                    });
                    html += "</tr>";
                }
               
                //行
                for (var i = 0; i < length; i++) {

                    html += "<tr>";

                    var row = model.Result.Table[0];
                    $.each(row, function (key) {
                        html += "<td>";
                        html += row[key];
                        html += "</td>";
                    });

                    html += "</tr>";
                }

            } 
           
        }
        else if (serializeType == "Xml") {

        }
        html += "</table>";
        dgvCS.html(html);
    }
}

//创建发送的数据
function CreateData(method, ContentType) {
     
    var paraList = document.getElementsByName("chkParam_");
    var csJson = "";
    var value = "";
  
    var data = new FormData(); 
    for (var i = 0; i < paraList.length; i++) {
        json = paraList[i].value;
        json = eval("(" + json + ")");
        var paramName = json.ParamName;
        var dataType = json.DataType; 
        var txtValue = $("#cs_" + paramName);
        value = txtValue.val().trim().replace("\'", "");

        if (ContentType == "multipart/form-data") {

            if (dataType == "Image") {

                var file = txtValue.get(0);
                var files = file.files;
                if (files.length > 0) {
                    data.append(paramName, files[0]);
                }
            }
            else { 
                data.append(paramName, value);
            }
        }
        else {

            if (method == "Get") {
                if (csJson != "") {
                    csJson += "&";
                }
                if (dataType == "Varchar" || dataType == "Int" || dataType == "Decimal") {
                    csJson += paramName + "=" + value;
                }  
                else {
                    csJson += paramName + "='" + value + "'";
                }
            }
            else if (method == "Post") {
                if (csJson != "") {
                    csJson += ",";
                }

                var value1 = '';
                if (dataType == "Varchar" || dataType == "Int" || dataType == "Decimal") {
                    value1 = value.replace("\'", "");
                } 
                else {
                    value1 = "'" + value + "'";
                }

                csJson += paramName + ":" + value1;
            }
        } 
    }
    
    if (ContentType == "multipart/form-data") {
        return data;
    }
    else {

        var jsonResult = csJson;
        if (method == "Post") {
           
            jsonResult = eval("({" + csJson + "})");
        } 

        return jsonResult;
    }  
}

//发送脚本例子
function CreateAjaxHtml() {

    var spac = "    ";
    var version = "";
    var chkVersion = document.getElementById("chkVersion");

    if (chkVersion.checked) {
        version = chkVersion.value;
    }
     
    var contentType = $("#txtCSContentType").val();
    var jsonHtml = "";
    var method = $("#cbxMethod").val();
    var url = $("#txtCSUrl").val();

    jsonHtml = "function fun1()\n";
    jsonHtml += "{\n"
    jsonHtml += spac +'$.ajax({\n'
    jsonHtml += spac + spac + 'url:' + urlAuthorLogin + ',\n';
    jsonHtml += spac + spac + 'dataType:"json",\n';
    jsonHtml += spac + spac + 'type:"Post",\n';
    jsonHtml += spac + spac + 'contentType:"application/json",\n';
    jsonHtml += spac + spac + 'data:JSON.stringify({UserName:"admin",Password:"12345"}),\n';
    jsonHtml += spac + spac + 'success: function (author) {\n';
    jsonHtml += spac + spac + spac + 'fun2(author);\n';
    jsonHtml += spac + spac + '}\n';
    jsonHtml += spac +'})\n';
    jsonHtml += "}\n\n"

    jsonHtml += "function fun2(author)\n";
    jsonHtml += "{\n";
     
    if (contentType == "multipart/form-data") {
        jsonHtml += "    var data = new FormData();\n"; 
        jsonHtml += CreateValueAjax(method, contentType);
    } 
    jsonHtml += spac + '$.ajax({\n'; 
    jsonHtml += spac + spac + 'beforeSend: function (xhr) {\n';
    jsonHtml += spac + spac + spac + 'xhr.setRequestHeader("Authorization", "Bearer " + author.token); \n';
    jsonHtml += spac + spac + spac + 'xhr.setRequestHeader("Version", "' + version + '"); \n';
    jsonHtml += spac + spac + '},\n'; 

    if (contentType == "multipart/form-data") {

        jsonHtml += spac + spac + 'url:"' + url + '",\n';
        jsonHtml += spac + spac + 'type: "post",\n'; 
        jsonHtml += spac + spac + 'cache: false,\n'; 
        jsonHtml += spac + spac + 'contentType: false,\n'; 
        jsonHtml += spac + spac + 'processData: false,\n'; 
        jsonHtml += spac + spac + 'data: data,\n'; 

    }
    else if (contentType == "application/json") {  

        var valueAjax = "";
        if (method == "Get") { 
            valueAjax = CreateValueAjax(method, contentType);
            jsonHtml += spac + spac + 'url:"' + url + '?' + valueAjax + '",\n'; 
            jsonHtml += spac + spac + 'type: "get",\n';
        }
        else {
            jsonHtml += spac + spac + 'url:"' + url + '",\n'; 
            jsonHtml += spac + spac + 'type: "post",\n'; 
            valueAjax = CreateValueAjax(method, contentType);
            jsonHtml += spac + spac + 'data: JSON.stringify( {' + valueAjax + '} ),\n';
        }  
        jsonHtml += spac + spac + 'contentType: "application/json",\n'; 
        
    }
    jsonHtml += spac + spac + 'success: function (result) {}\n'; 
    jsonHtml += spac + '})\n'; 
    jsonHtml += "}";
    $("#txtAjax").val(jsonHtml);
}

function CreateValueAjax(method, ContentType) {
    var strAjax = '';

    var paraList = document.getElementsByName("chkParam_"); 
    var value = ""; 
     
    for (var i = 0; i < paraList.length; i++) {
        json = paraList[i].value;
        json = eval("(" + json + ")");

        var paramName = json.ParamName;
        var dataType = json.DataType;

        value = json.DefaultValue.replace("\'", "").trim();;

        if (ContentType == "multipart/form-data") {

            if (dataType == "Image") {
                strAjax += '    var file = $("#' + paramName+'").get(0);\n';
                strAjax += '    var files = file.files;\n';
                strAjax += '    if (files.length > 0) {\n';
                strAjax += '        data.append(paramName, files[0]);\n';
                strAjax += '    }\n\n';
            }
            else {
                strAjax += '    data.append(' + paramName + ', ' + value+');\n'; 
            }
        }
        else {

            if (method == "Get") {
                if (strAjax != "") {
                    strAjax += "&";
                }
                if (dataType == "Varchar" || dataType == "Int" || dataType == "Decimal") {
                    strAjax += paramName + "=" + value;
                }  
                else {
                    strAjax += paramName + "='" + value + "'";
                }
            }
            else if (method == "Post") {
                if (strAjax != "") {
                    strAjax += ",";
                }

                var value1 = '';
                if (dataType == "Varchar" || dataType == "Int" || dataType == "Decimal") {
                    value1 = value;
                } 
                else {
                    value1 = "'" + value + "'";
                }

                strAjax += paramName + ":" + value1;
            }
        }
    }

    return strAjax;
}

function onVersion() {
    CreateAjaxHtml();
}