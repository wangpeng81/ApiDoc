
const NAME = 'xnTable'
const VERSION = '4.5.2'
const DATA_KEY = 'bs.xnTable'
const EVENT_KEY = `.${DATA_KEY}`
const DATA_API_KEY = '.data-api'
const JQUERY_NO_CONFLICT = $.fn[NAME]
const SELECT_BG_COLOR = 'btn-info';

class xnTable {
    constructor(element) {
        this._element = element; 
        this._rowCount = 0;
        this._preTr;
        this._currentTr;
        this.init();
    }
     
    init() {

        var id = this._element.id;
        var nodes = $("#" + id + " tbody tr");
        var what = this;
          
        if (nodes.length != this._rowCount) {

            for (var i = 0; i < nodes.length; i++) {
                var tr = $(nodes[i]); 
                tr.click(function () { 

                    if ($(this).hasClass(SELECT_BG_COLOR)) {
                        $(this).removeClass(SELECT_BG_COLOR);
                        what._currentTr = null;
                    }
                    else {
                        $(this).addClass(SELECT_BG_COLOR);
                        what._currentTr = this;
                    }

                    if (what._preTr != undefined && what._preTr.rowIndex != this.rowIndex) {
                        $(what._preTr).removeClass(SELECT_BG_COLOR);
                    }

                    if (what._preTr == undefined || what._preTr.rowIndex != this.rowIndex) {
                        what._preTr = this;
                    }
                    
                });
            } 
        } 
        this._rowCount = nodes.length;
    }

    getSelectionItem() {

        if (this._currentTr != undefined && this._currentTr != null) { 
            return $(this._currentTr);
        }

        return null;
    }

    getSelection() {
        if (this._currentTr != undefined && this._currentTr != null) {
            var strJson = $(this._currentTr).attr("data-data");
            var Json = eval("(" + strJson + ")");
            return Json;
        }
        return null;
    }

    getSelections(columnName) {

        if (columnName == undefined) {
            columnName = "";
        }
        var id = this._element.id;
        var selector = "#" + id + " tbody :checked";
        var ids = [];
     
        var nodes = $(selector);
        for (var i = 0; i < nodes.length; i++) {
            var chk = nodes[i].parentElement.parentElement.parentElement;
            var strJson = $(chk).attr("data-data");
            var json = eval("(" + strJson + ")");
            if (columnName != "") {
                ids.push(json[columnName]);
            }
            else {
                ids.push(json);
            }
        }

        return ids;
    }

    static _jQueryInterface(config,opt) {

        var value;

        this.each(function () {

            const $this = $(this); //$this 只是个变量名，加$是为说明其是个jquery对象。
            let data = $this.data(DATA_KEY);

             if (!data) {
                 data = new xnTable(this);
                 $this.data(DATA_KEY, data); 
             }
 
             if (typeof config === 'string') {

                 if (typeof data[config] === 'undefined') {
                     throw new TypeError(`No method named "${config}"`)
                 }

                 value = data[config](opt);
             }
         });

        return typeof value === 'undefined' ? this : value;
    }
     
}
 
$.fn[NAME] = xnTable._jQueryInterface;
$.fn[NAME].Constructor = xnTable;
$.fn[NAME].noConflict = () => {
    $.fn[NAME] = JQUERY_NO_CONFLICT
    return Tab._jQueryInterface
}
  