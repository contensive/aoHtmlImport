
using System;
using Contensive.Addons.HtmlImport.Controllers;
using Contensive.Addons.HtmlImport.Models;
using Contensive.BaseClasses;

namespace Contensive.Addons.HtmlImport {
    namespace Views {
        //
        public class HtmlImportClass : AddonBaseClass {
            //
            public override object Execute(CPBaseClass cp) {
                try {
                    string button = cp.Doc.GetText("button");
                    string statusMessage = "";
                    const string uploadFormInputName = "uploadFile";
                    if (!string.IsNullOrEmpty(button)) {
                        //
                        // -- process button
                        string uploadFile = "";
                        string uploadFolder = cp.TempFiles.CreateUniqueFolder();
                        if (!cp.TempFiles.SaveUpload(uploadFormInputName, uploadFolder, ref uploadFile)) {
                            statusMessage = "Upload failed";
                        } else {
                            ImportController.importHtmlFile(cp, cp.TempFiles.PhysicalFilePath + uploadFolder + uploadFile, cp.Doc.GetInteger("layoutId"), cp.Doc.GetInteger("templateId"), ref statusMessage);
                        }
                    }
                    //
                    // -- create output form
                    var form = cp.AdminUI.NewToolForm();
                    form.Title = "Html Importer";
                    form.Warning = (string.IsNullOrWhiteSpace(statusMessage) ? "" : statusMessage);
                    form.Description = cp.Html5.P("This tool uploads and imports html files and converts the html to Mustache-format compatible templates and layouts. See reference at the end of this document.");
                    form.Body += cp.AdminUI.GetEditRow("Html Upload", cp.AdminUI.GetFileEditor(uploadFormInputName, ""), "Select the file you need to import. The file may include directives the determine the save location and Mustache replacements.");
                    form.Body += cp.AdminUI.GetEditRow("Select Layout", cp.AdminUI.GetLookupContentEditor("layoutId", "Layouts"), "(Optional) Select the Layout you want to populate with this html document. Leave blank if the target is set in a meta tag of the html document.");
                    form.Body += cp.AdminUI.GetEditRow("Select Template", cp.AdminUI.GetLookupContentEditor("templateId", "Page Templates"), "(Optional) Select the Page Template you want to populate with this html document. Leave blank if the target is set in a meta tag of the html document.","",false,true);
                    form.AddFormButton("Upload");
                    form.AddFormButton("Cancel");
                    return form.GetHtml(cp);
                } catch (Exception ex) {
                    //
                    // -- the execute method should typically not throw an error into the consuming method. Log and return.
                    cp.Site.ErrorReport(ex);
                    return string.Empty;
                }
            }
        }
    }
}
