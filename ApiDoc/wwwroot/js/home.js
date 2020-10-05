var treeview;
var selectedNode; 
var vDo;

function genID(length){
    return Number(Math.random().toString().substr(3, length) + Date.now()).toString(36);
}

function LoadTreeView(result) {

    treeview = $('#treeview7').treeview({
        color: "#428bca",
        showBorder: false,
        enableLinks: false,
        backColor: "#f8f9fa",
        nodeIcon: 'zi zi_floderLine',
        selectedIcon: 'fa fa-check',
        selectedBackColor: '#f8f9fa',
        selectedColor: '#8cbe00', 
        data: result
    });

    treeview.treeview("collapseAll");
    treeview.on('nodeSelected', function (event, data) {

        selectedNode = data;
 
            //var tabItem = { id: data.nodeId, title: data.data.folderName, url: "Interface/Index?fksn=" + data.data.sn };

            // closableTab.addTab(tabItem);
            var iframe = $("#myIframeList"); 
            iframe.attr('src', urlRoot + '/Interface/Index?fksn=' + data.data.sn);
        
    }) 
}

//添加+修改=接口
function showInterface(SN,fksn) {
    var model = $("#myModalInterface");
    var height = window.screen.availHeight;
    model.find(".modal-body").height(height - 170);

    var iframe = $("#myIframe"); 

    if (SN > 0) {
        iframe.attr('src', urlRoot + '/Interface/Add?SN=' + SN);
    }
    else {
        iframe.attr('src', urlRoot + '/Interface/Add?fksn=' + fksn);
    }
    
    model.modal('show');
}

 
$(function () {

   
    var url = urlFolderAll + "?root=true"; 
   
    $.get(url, LoadTreeView);
 
})
 