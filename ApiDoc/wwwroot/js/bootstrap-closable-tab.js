var closableTab = {
	 
	addTab:function(tabItem){  
 
		var id = "tab_item_" + tabItem.id;
		var container ="tab_container_" + tabItem.id;
 
        var tab = $("#myTab");
       
        var id = "tab_item_" + tabItem.id;
        var container_id = "tab_container_" + tabItem.id;

        var li_tab = '<li class="nav-item">';
        li_tab += '<a class="nav-link" id="' + id + '" data-toggle="tab" href="#' + container_id + '"  role="tab" aria-selected="true">' + tabItem.title;
        li_tab = li_tab + '</a></li>';

        //合并ul和li元素
        tab.append(li_tab); 
      
        var tabpanel = '<div class="tab-pane fade h-100" id="' + container_id + '" role="tabpanel" aria-labelledby="nav-home-tab">'; 
        if (tabItem.url) {
            tabpanel += '<iframe src="' + tabItem.url + '" frameborder="0" scrolling="yes"></iframe>'
        } else if (tabItem.content) {
            tabpanel += tabItem.content;
        }
           
        tabpanel += '</div>';

        var myTabContent = $('#myTabContent');
        myTabContent.append(tabpanel);

        $('#myTab li:last-child a').tab('show');

	},

	//关闭tab
	closeTab:function(item){
		var val = $(item).attr('tabclose');
		var containerId = "tab_container_"+val.substring(9);
   	    
   	    if($('#'+containerId).hasClass('active')){
   	    	$('#'+val).prev().addClass('active');
   	    	$('#'+containerId).prev().addClass('active');
   	    }


		$("#"+val).remove();
		$("#"+containerId).remove();
	}
}