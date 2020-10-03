$(function () {

    var tab = $.fn.tab;
    var tab = $.fn['tab'].Constructor;

    tab.addTab = function (options) {

        var id = "tab_item_" + options.id;
        var container_id = "tab_container_" + options.id;

        //判断是否已存在指定ID的tab
        if ($("#" + id).length > 0) {
            throw "当前ID的Tab已存在．";
        }
 
        var li = $('<li class="nav - item">');
        var a = $('<a class="nav - link active" id="' + id + '" data-toggle="tab" href="#' + container + '"  role="tab" aria-controls="home" aria-selected="true">' + options.text);

        //合并li和a元素
        li.append(a);

       // var ul = $(this);
        var ul = tab;
        //合并ul和li元素
        ul.append(li);

        //添加完成显示当前li
        $(li).tab("show");

        //构建div内容元素
        //var div = $("<div />", {
        //    "id": options.id,
        //    "class": "tab-pane fade in active",
        //});

        var tabpanel = '<div class="tab-pane fade show active " id="' + container + '" role="tabpanel" aria-labelledby="nav-home-tab">' +
            '<iframe src="' + options.url + '" frameborder="1" scrolling="yes" style="width:100%;height:200px;background-color:white"></iframe>' +
            '</div>';

        var div = $(tabpanel);

        //兼容纯文本和html片段
        typeof options.content == "string" ? div.append(options.content) : div.html(options.content);

        var container = $(".tab-content");
        container.append(div);

        //添加完成后显示div
        $(div).siblings().removeClass("active");
    }

})
