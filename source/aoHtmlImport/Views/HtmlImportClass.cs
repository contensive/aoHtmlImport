
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
                    //
                    form.Footer += cp.Html5.H4("Instructions");
                    form.Footer += cp.Html5.P("Use this tool to upload an html file for use as a page template or layout. Page templates are used for web pages. Layouts are generic records used by add-ons to construct forms.");
                    form.Footer += cp.Html5.P("Upload an html file to create or update a template or layout from that file. Upload a zip file and the files will be unzipped and all non-html files will be copied to the websites root www directory. All html files wil be imported.");
                    //
                    form.Footer += cp.Html5.H5("Import Destination");
                    form.Footer += cp.Html5.P("The destination is the template or layout record where the upload will be stored. There are two ways to set the destination, html meta tags or select manually.");
                    form.Footer += cp.Html5.Ol(""
                        + cp.Html5.Li("Meta Tag. Include in your html file a meta tag with name set to either 'layout' or 'template' and content set to the name of the record."
                            + "<pre>" + cp.Utils.EncodeHTML("<meta name=\"template\" content=\"One Column Template\">") + "</pre>"
                            + "<pre>" + cp.Utils.EncodeHTML("<meta name=\"layout\" content=\"Join Form\">") + "</pre>"
                            + "")
                        + cp.Html5.Li("Manual Select. Select the template and/or layout on this form and the html will be saved to a record with the same name as the uploaded file.")
                        );
                    //
                    form.Footer += cp.Html5.H5("Mustache Templates");
                    form.Footer += cp.Html5.P("Mustache is a popular templating scheme. You may choose to include mustache tags in your html directly in which case the html may not render well in a browser. You can alternativly choose to set special styles outlined here and the import tool will add the Mustache tags you indicate. Reference any of the many <a href=\"https://gist.github.com/Dammmien/5f0bff8643cb931da7e9495f782aad0a\">Mustache cheat sheets online</a>.");
                    //
                    form.Footer += cp.Html5.H5("Basic Tag");
                    form.Footer += "<pre>"
                        + cp.Utils.EncodeHTML("<p>My name is <span class=\"mustache-basic firstName another-class\">Sample Name</span>.</p>"
                        + "\n<p>My name is <span class=\"another-class\">{{firstName}}</span>.</p>")
                        + "</pre>";
                    //
                    form.Footer += cp.Html5.H5("Ignore Tag");
                    form.Footer += "<pre>"
                        + cp.Utils.EncodeHTML(""
                            + "\n<p>This is in the layout.<span class=\"mustache-ignore\">This is not.</span></p>"
                            + "\n<p>This is in the layout.</p>"
                            + "")
                        + "</pre>";
                    //
                    form.Footer += cp.Html5.H5("Loop Tag");
                    form.Footer += "<pre>"
                        + cp.Utils.EncodeHTML(""
                            + "\n<ul class=\"mustache-loop staff\">"
                            + "\n\t<li class=\"mustache-basic name\">Sample Name</li>"
                            + "\n\t<li class=\"mustache-ignore\">Name To Skip</li>"
                            + "\n</ul>"
                            + "\n<ul class=\"mustache-loop staff\">"
                            + "\n\t{{#staff}}"
                            + "\n\t<li class=\"\">{{name}}</li>"
                            + "\n\t{{/staff}}"
                            + "\n</ul>"
                            + "")
                        + "</pre>";
                    //
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
