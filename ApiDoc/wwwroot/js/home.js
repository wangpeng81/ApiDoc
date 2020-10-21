var treeview;
var selectedNode; 
var vDo;
 
function LoadTreeView(result) {

    treeview = $('#treeview7').treeview({
        color: "var(--gray)",
        showBorder: false,
        enableLinks: false,
        backColor: "#f8f9fa",
        nodeIcon: '',
        selectedIcon: 'fa fa-check',
        selectedBackColor: '#f8f9fa',
        selectedColor: '#8cbe00', 
        data: result
    });

    treeview.treeview("collapseAll");
    treeview.on('nodeSelected', function (event, data) {

        selectedNode = data;
  
            var iframe = $("#myIframeList"); 
            iframe.attr('src', urlRoot + '/Interface/Index?fksn=' + data.data.sn);
        
    }) 
}

//添加+修改=接口
function showInterface(SN,fksn) {
    var model = $("#myModalInterface");
    var height = $(window).height();
    var width = $(window).width();
    model.find(".modal-content").height(height); 
    model.find(".modal-content").width(width); 
    var iframe = $("#myIframe"); 

    if (SN > 0) {
        iframe.attr('src', urlRoot + '/Interface/Add?SN=' + SN);
    }
    else {
        iframe.attr('src', urlRoot + '/Interface/Add?fksn=' + fksn);
    }
    
    model.modal('show');
}

//查询
function btnSearchFolder_Click() {
    var txtQFolderName = $("#txtQFolderName").val();
    var url = urlFolderAll + "?root=true&folderName=" + txtQFolderName; 
    $.get(url, LoadTreeView);
}

$(function () {
     
    var url = urlFolderAll + "?root=true"; 
   
    $.get(url, LoadTreeView);
 
})
 