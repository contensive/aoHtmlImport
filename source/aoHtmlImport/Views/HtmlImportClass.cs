
using System;
using System.Collections.Generic;
using System.IO;
using Contensive.Addons.HtmlImport.Controllers;
using Contensive.Addons.HtmlImport.Models;
using Contensive.BaseClasses;
using Contensive.Models.Db;

namespace Contensive.Addons.HtmlImport {
    namespace Views {
        //
        public class HtmlImportClass : AddonBaseClass {
            //
            private readonly string uploadFormInputName = "uploadFile";
            private readonly string buttonCancel = "Cancel";
            private readonly string buttonSubmit = "Upload";
            //
            public override object Execute(CPBaseClass cp) {
                try {
                    //
                    // -- create output form
                    var form = cp.AdminUI.NewToolForm();
                    if (!string.IsNullOrEmpty(cp.Doc.GetText("button"))) {
                        //
                        // -- process button click
                        if (cp.Doc.GetText("button").Equals(buttonCancel)) {
                            cp.Response.Redirect("?");
                            return string.Empty;
                        }
                        string uploadFile = "";
                        string uploadFolderPath = cp.TempFiles.CreateUniqueFolder();
                        if (!cp.TempFiles.SaveUpload(uploadFormInputName, uploadFolderPath, ref uploadFile)) {
                            //
                            // -- upload failed
                            form.FailMessage = "Upload failed";
                        } else {
                            string statusMessage = string.Empty;
                            if (!ImportController.processImportFile(cp, uploadFolderPath + uploadFile, cp.Doc.GetInteger("importTypeId"), cp.Doc.GetInteger("layoutId"), cp.Doc.GetInteger("pageTemplateId"), cp.Doc.GetInteger("emailTemplateId"), cp.Doc.GetInteger("emailId"), ref statusMessage)) {
                                form.FailMessage = statusMessage;
                            } else {
                                form.SuccessMessage = "Success";
                            }
                        }
                        cp.TempFiles.DeleteFolder(uploadFolderPath);
                    }
                    //
                    // -- populate output form
                    form.Title = "Html Importer";
                    form.Description = cp.Html5.P("This tool uploads and imports html files and converts the html to Mustache-format compatible layouts, templates and addons. See reference at the end of this document.");
                    {
                        var importTypeList = new List<string>() { "Import Destination set in Html File", "Layout For Addon", "Page Template", "Email Template", "Email" };
                        string editRow = cp.AdminUI.GetEditRow("Select Import Type", cp.AdminUI.GetLookupListEditor("importTypeId", importTypeList, 1, "hiImportTypeid"), 
                            "Select the type of html you are importing.", "");
                        form.Body += cp.Html5.Div(editRow, "", "hiImportType");
                    }
                    {
                        //form.Body += cp.AdminUI.GetEditRow("Select Layout", cp.AdminUI.GetLookupContentEditor("layoutId", "Layouts", cp.Doc.GetInteger("layoutId")), "(Optional) Select the Layout you want to populate with this html document. Leave blank if the target is set in a meta tag of the html document.", "hiSelectLayout");
                        string editRow = cp.AdminUI.GetEditRow("Select Layout", cp.AdminUI.GetLookupContentEditor("layoutId", "Layouts", cp.Doc.GetInteger("layoutId")), 
                            "Select the Layout you want to populate with this html document.", "hiSelectLayout");
                        form.Body += cp.Html5.Div(editRow, "", "hiSelectLayoutId");
                    }
                    {
                        string editRow = cp.AdminUI.GetEditRow("Select Page Template", cp.AdminUI.GetLookupContentEditor("pageTemplateId", "Page Templates", cp.Doc.GetInteger("pagetemplateId")), 
                            "Select the Page Template you want to populate with this html document. " +
                            "Page templates import just the html body. Head metadata and associated resources have to be manually configured.", "hiSelectPageTemplate");
                        form.Body += cp.Html5.Div(editRow, "", "hiSelectPageTemplateId");
                    }
                    {
                        string editRow = cp.AdminUI.GetEditRow("Select Email Template", cp.AdminUI.GetLookupContentEditor("emailTemplateId", "Email Templates", cp.Doc.GetInteger("emailtemplateId")), 
                            "Select the Email Template you want to populate with this html document. " +
                            "Email templates import the entire html document including the head tag elements. If they are missing the system will create them.", "hiSelectPageTemplate");
                        form.Body += cp.Html5.Div(editRow, "", "hiSelectEmailTemplateId");
                    }
                    {
                        string editRow = cp.AdminUI.GetEditRow("Select Email", cp.AdminUI.GetLookupContentEditor("emailId", "Email", cp.Doc.GetInteger("emailId")), 
                            "Select the Group, System or Conditional Email you want to populate with this html document. ", "hiSelectEmail");
                        form.Body += cp.Html5.Div(editRow, "", "hiSelectEmailId");
                    }

                    form.Body += cp.AdminUI.GetEditRow("Html Upload", cp.AdminUI.GetFileEditor(uploadFormInputName, ""), "Select the file you need to import. The file may include directives the determine the save location and Mustache replacements.");

                    //
                    form.Footer += cp.Html5.H4("Instructions");
                    form.Footer += cp.Html5.Div(
                        cp.Html5.P("Use this tool to upload an html file for use as a page template or layout. Page templates are used for web pages. Layouts are generic records used by add-ons to construct forms.")
                        + cp.Html5.P("Upload an html file to create or update a template or layout from that file. Upload a zip file and the files will be unzipped and all non-html files will be copied to the websites root www directory. All html files wil be imported.")
                        , "ml-4"
                    );
                    //
                    form.Footer += cp.Html5.H5("Import Type");
                    form.Footer += cp.Html5.Div(
                        cp.Html5.P("Select the type of data being imported. There are two ways to set the destination, html meta tags or select manually.")
                        + cp.Html5.Ol(""
                            + cp.Html5.Li("Meta Tag. Include in your html file a meta tag with name set to either 'layout' or 'template' and content set to the name of the record."
                            + "<pre>" + cp.Utils.EncodeHTML("<meta name=\"template\" content=\"One Column Template\">") + "</pre>"
                            + "<pre>" + cp.Utils.EncodeHTML("<meta name=\"layout\" content=\"Join Form\">") + "</pre>"
                            + "<pre>" + cp.Utils.EncodeHTML("<meta name=\"email\" content=\"Invitation Email\">") + "</pre>"
                            + "")
                            + cp.Html5.Li("Manual Select. Select the template and/or layout on this form and the html will be saved to a record with the same name as the uploaded file.")
                        )
                        , "ml-4"
                    );
                    //
                    form.Footer += cp.Html5.H5("Mustache Templates");
                    {
                        string indent = "";
                        indent += cp.Html5.P("Mustache is a popular templating scheme. You may choose to include mustache tags in your html directly in which case the html may not render well in a browser. You can alternativly choose to set special styles outlined here and the import tool will add the Mustache tags you indicate. Reference any of the many <a href=\"https://mustache.github.io/mustache.5.html\">Mustache references online</a>.");
                        form.Footer += cp.Html5.Div(indent, "ml-4");
                    }
                    form.Footer += cp.Html5.H5("Mustache Variable");
                    {
                        string sample = "";
                        sample += "<p>My name is <span data-mustache-variable=\"firstName\">Sample Name</span>.</p>";
                        sample += "\n<p>My name is <span>{{firstName}}</span>.</p>";
                        string indent = "";
                        indent += cp.Html5.P("Replace the content of the html tag with a Mustache Basic tag.");
                        indent += "<pre>" + cp.Utils.EncodeHTML(sample) + "</pre>";
                        form.Footer += cp.Html5.Div(indent, "ml-4");
                    }
                    ////
                    //form.Footer += cp.Html5.H5("Delete");
                    //{
                    //    string sample = "";
                    //    sample += "<p>This is in the layout.<span data-delete>This is not.</span></p>";
                    //    sample += "\n<p>This is in the layout.</p>";
                    //    string indent = "";
                    //    indent += cp.Html5.P("Delete the tag that contains this class, and all child tags.");
                    //    indent += "<pre>" + cp.Utils.EncodeHTML(sample) + "</pre>";
                    //    form.Footer += cp.Html5.Div(indent, "ml-4");
                    //}
                    //
                    form.Footer += cp.Html5.H5("Mustache Section");
                    {
                        string sample = "";
                        sample += "<ul data-mustache-section=\"staff\">"
                            + "\n\t<li data-mustache-variable=\"name\">Sample Name</li>"
                            + "\n\t<li data-delete>Name To Skip</li>"
                            + "\n</ul>"
                            + "\n<ul>"
                            + "\n\t{{#staff}}"
                            + "\n\t<li class=\"\">{{name}}</li>"
                            + "\n\t{{/staff}}"
                            + "\n</ul>";
                        string indent = "";
                        indent += cp.Html5.P("Add a Mustache Section around content to be removed or repeated. If the object property is false, null, or an empty list, the section is removed. If the value is true the section is included. If the value is a list the section is repeated for each item in the list.");
                        indent += "<pre>" + cp.Utils.EncodeHTML(sample) + "</pre>";
                        form.Footer += cp.Html5.Div(indent, "ml-4");
                    }
                    ////
                    //form.Footer += cp.Html5.H5("Truthy Tag");
                    //{
                    //    string indent = "";
                    //    indent += cp.Html5.P("Insert Mustache Truthy around the content of a tag that includes this class. The Mustache Tag is set to class that follows mustache-truthy.");
                    //    indent += "<pre>"
                    //        + cp.Utils.EncodeHTML(""
                    //            + "\n<div class=\"mustache-truthy headline\">"
                    //            + "\n\t<h2 class=\"mustache-basic headline\">This is the Sample Headline</h2>"
                    //            + "\n</div>"
                    //            + "\n<div>"
                    //            + "\n\t{{#headline}}"
                    //            + "\n\t<h2>{{headline}}</h2>"
                    //            + "\n\t{{/headline}}"
                    //            + "\n</div>"
                    //            + "")
                    //        + "</pre>";
                    //    form.Footer += cp.Html5.Div(indent, "ml-4");
                    //}
                    //
                    form.Footer += cp.Html5.H5("Mustache Inverted Section");
                    {
                        string indent = "";
                        indent += cp.Html5.P("Add a Mustache Inverted Section data attrbiute around content to be included if the object property value is false.");
                        indent += "<pre>"
                            + cp.Utils.EncodeHTML(""
                                + "\n<div data-mustache-section=\"emptyList\">"
                                + "\n\t<p>No items were found.</p>"
                                + "\n</div>"
                                + "\n<div>"
                                + "\n\t{{^emptyList}}"
                                + "\n\t<p>No items were found.</p>"
                                + "\n\t{{/itemList}}"
                                + "\n</div>"
                                + "")
                            + "</pre>";
                        form.Footer += cp.Html5.Div(indent, "ml-4");
                    }

                    form.Footer += cp.Html5.H5("Mustache Attribute Values");
                    {
                        string sample = "";
                        sample += "<p>My example is <span value=\"0\" data-mustache-value=\"id\">content</span>.</p>";
                        sample += "\n<p>My example is <span value=\"{{id}}\">content</span>.</p>";
                        string indent = "";
                        indent += cp.Html5.P("Replace the value of the html tag with a Mustache Value tag.");
                        indent += "<pre>" + cp.Utils.EncodeHTML(sample) + "</pre>";
                        form.Footer += cp.Html5.Div(indent, "ml-4");
                    }

                    form.Footer += cp.Html5.H5("Mustache Addon");
                    {
                        string sample = "";
                        sample += "<span data-mustache-addon=\"content_box\">content</span>";
                        sample += "\n<span>{% \"content box\" %}</span>";
                        string indent = "";
                        indent += cp.Html5.P("NOTE: If the addon name contains spaces, replace each space with a _ instead. Ex. content box would be content_box. Replace the inner content of the html tag with the addon after the Mustache Addon tag.");
                        indent += "<pre>" + cp.Utils.EncodeHTML(sample) + "</pre>";
                        form.Footer += cp.Html5.Div(indent, "ml-4");
                    }

                    form.AddFormButton("Upload");
                    form.AddFormButton("Cancel");
                    return form.GetHtml(cp);
                }
                catch (Exception ex) {
                    //
                    // -- the execute method should typically not throw an error into the consuming method. Log and return.
                    cp.Site.ErrorReport(ex);
                    return string.Empty;
                }
            }
        }
    }
}