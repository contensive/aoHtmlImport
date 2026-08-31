// javascript file create as exported for addon [Html Import], collection [Html Import] in site [app200509]
$(function(){
    $("#hiImportTypeid").on("change",htmlImportSelect);
    htmlImportSelect();
})
function htmlImportSelect(){
    var importTypeId = $('#hiImportTypeid').find(":selected").val();
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
            // -- import destination set in html file, hide all selectors
            $("#hiSelectLayoutId,#hiSelectPageTemplateId,#hiSelectEmailTemplateId,#hiSelectLayoutFrameworkId,#hiSelectEmailId").hide();
            break;
    }
}