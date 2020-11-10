

function showCreateAuthorize() {
     
    showModalInfo("请确认是否要保存，保存后将更改码?", btnSaveAuthorize_Click);

}

//保存授权码
function btnSaveAuthorize_Click() {
     
    var txtAuthorize = $("#txtSecurityKey");
    var Authorize = txtAuthorize.val();
    var uri = urlMyConfigSaveAuthorize + "?SecurityKey=" + Authorize + "&random=" + Math.random();

    $.ajax({
        cache: false,
        url: uri,
        type: "GET", 
        success: function (result) {
            if (result > 0) {
                popToastSuccess("保存成功!");
            }
        }
    }); 

    return;
}

function btnCreateAuthorize_Click() {
    var txtAuthorize = $("#txtAuthorize");
    var vguid = guid();
    txtAuthorize.val(vguid);
}

//数据库保存
function btnSaveDataBase() {

    //SqlServer
    var txtSqlServerApiDocConnStr = $("#txtSqlServerApiDocConnStr");
    var txtSqlServerDataBases = $("#txtSqlServerDataBases"); 
    var dbSqlServer = txtSqlServerDataBases.val().split("\n"); 
     
    //MySql
    var txtMySqlApiDocConnStr = $("#txtMySqlApiDocConnStr");
    var txtMySqlDataBases = $("#txtMySqlDataBases");
    var dbMySqlServer = txtMySqlDataBases.val().split("\n"); 

    var txtOracleApiDocConnStr = $("#txtOracleApiDocConnStr");
    var txtOracleDataBases = $("#txtOracleDataBases");
    var dbOracle = txtOracleDataBases.val().split("\n"); 
    
    var data = {
        DataType: {
            SqlServer: {
                ApiDocConnStr: txtSqlServerApiDocConnStr.val(),
                DataBases: dbSqlServer
            },
            MySql: {
                ApiDocConnStr: txtMySqlApiDocConnStr.val(),
                DataBases: dbMySqlServer
            },
            Oracle: {
                ApiDocConnStr: txtOracleApiDocConnStr.val(),
                DataBases: dbOracle
            }
        }
    };

    $.post(urlMyConfigSaveDataType, data, function (result) {
        if (result > 0) {
            popToastSuccess("保存成功!");
        }
    });
}

$(function () {

    //初始化提示信息
    var option = { animation: true, delay: 2000 };
    $('.toast').toast(option);
 
});
    