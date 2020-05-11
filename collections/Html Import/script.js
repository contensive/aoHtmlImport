// javascript file create as exported for addon [Html Import], collection [Html Import] in site [app200509]
$(function(){
    console.log("html import ready");
    $("#hiImportTypeid").on("change",function(){
        var importTypeId = $('#hiImportTypeid').find(":selected").val();
        switch(importTypeId) {
            case "2":
                // -- layout
                $("#hiSelectPageTemplateId,#hiSelectEmailTemplateId,#hiSelectEmailId").hide();
                $("#hiSelectLayoutId").show();
                break;
            case "3":
                // -- page template
                $("#hiSelectLayoutId,#hiSelectEmailTemplateId,#hiSelectEmailId").hide();
                $("#hiSelectPageTemplateId").show();
                break;
            case "4":
                // -- email template
                $("#hiSelectLayoutId,#hiSelectPageTemplateId,#hiSelectEmailId").hide();
                $("#hiSelectEmailTemplateId").show();
                break;
            case "5":
                // -- email
                $("#hiSelectLayoutId,#hiSelectPageTemplateId,#hiSelectEmailTemplateId").hide();
                $("#hiSelectEmailId").show();
                break;
            default:
                //
                // -- upload has import type(s)
                $("#hiSelectLayoutId,#hiSelectPageTemplateId,#hiSelectEmailTemplateId").hide();
                $("#hiSelectEmail").show();
                break;
            }
    })
})