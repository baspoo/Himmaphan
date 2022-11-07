mergeInto(LibraryManager.library, {




  OnHtmlMessage: function (code,str) {
    console.log("call from plugin");
    HtmlMessage(code,Pointer_stringify(str));
  },
  OnHtmlPing: function (code) {
    console.log("call from plugin");
    HtmlPing(code);
  }



});