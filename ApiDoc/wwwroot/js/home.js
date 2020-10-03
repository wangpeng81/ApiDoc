var treeview;
var selectedNode; 
var vDo;

var urlAll = "/Folder/All?root=false"; 

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

        if (data.data.sn > 0) {
            var tabItem = { id: data.nodeId, title: data.data.folderName, url: "Interface/Index?fksn=" + data.data.sn };

            closableTab.addTab(tabItem);
        }
    }) 
}
 
$(function () {
     
    $.get(urlAll, LoadTreeView);
 
})
 