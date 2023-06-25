// javascript file create as exported for addon [Html Import], collection [Html Import] in site [app200509]
$(function(){
    console.log("html import ready");
    $("#hiImportTypeid").on("change",htmlImportSelect);
    htmlImportSelect();
})
function htmlImportSelect(){
    var importTypeId = $('#hiImportTypeid').find(":selected").val();
    console.log("html htmlImportSelect [" + importTypeId + "]");
    switch(importTypeId) {
        case "2":
            // -- layout
            $("#hiSelectPageTemplateId,#hiSelectEmailTemplateId,#hiSelectEmailId").hide();
            $("#hiSelectLayoutId,#hiSelectLayoutFrameworkId").show();
            break;
        case "3":
            // -- page template
            $("#hiSelectLayoutId,#hiSelectEmailTemplateId,#hiSelectEmailId,#hiSelectLayoutFrameworkId").hide();
            $("#hiSelectPageTemplateId").show();
            break;
        case "4":
            // -- email template
            $("#hiSelectLayoutId,#hiSelectPageTemplateId,#hiSelectEmailId,#hiSelectLayoutFrameworkId").hide();
            $("#hiSelectEmailTemplateId").show();
            break;
        case "5":
            // -- email
            $("#hiSelectLayoutId,#hiSelectPageTemplateId,#hiSelectEmailTemplateId,#hiSelectLayoutFrameworkId").hide();
            $("#hiSelectEmailId").show();
            break;
        default:
            //
            // -- upload has import type(s)
            $("#hiSelectLayoutId,#hiSelectPageTemplateId,#hiSelectEmailTemplateId,#hiSelectLayoutFrameworkId").hide();
            $("#hiSelectEmail").show();
            break;
    }
}