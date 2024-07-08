
using Contensive.BaseClasses;
using System;
using System.Collections.Generic;

namespace Contensive.Addons.HtmlImport {
    //
    public class HtmlImportAddon : AddonBaseClass {
        //
        private readonly string uploadFormInputName = "uploadFile";
        private readonly string buttonCancel = "Cancel";
        //
        public override object Execute(CPBaseClass cp) {
            try {
                //
                // -- create output form

                var tool = new PortalFramework.LayoutBuilderSimple();
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
                        tool.failMessage = "Upload failed";
                    } else {
                        var userMessageList = new List<string>();
                        var importTypeId = (CPLayoutBaseClass.ImporttypeEnum)cp.Doc.GetInteger("importTypeId");
                        //
                        // -- tmp hack. save platform version, set platform 4/5 based on layout requirement, then restore
                        // -- long-term, use cp.layout.process with platform argument
                        int layoutFrameworkSiteSetting = cp.Site.GetInteger("HTML PLATFORM VERSION");
                        var layoutFrameworkSelected = cp.Doc.GetInteger("layoutFrameworkSelectionId");
                        if (layoutFrameworkSiteSetting != layoutFrameworkSelected) { cp.Site.SetProperty("HTML PLATFORM VERSION", layoutFrameworkSelected == 2 ? 5 : 4); }
                        //
                        bool success = cp.Layout.processImportFile(uploadFolderPath + uploadFile, importTypeId, cp.Doc.GetInteger("layoutId"), cp.Doc.GetInteger("pageTemplateId"), cp.Doc.GetInteger("emailTemplateId"), cp.Doc.GetInteger("emailId"), ref userMessageList);
                        if (layoutFrameworkSiteSetting != layoutFrameworkSelected) { cp.Site.SetProperty("HTML PLATFORM VERSION", layoutFrameworkSiteSetting); }
                        if (!success) {
                            tool.failMessage = "Error<br><br>" + string.Join("<br>", userMessageList);
                        } else {
                            tool.successMessage = "Success<br><br>" + string.Join("<br>", userMessageList);
                            cp.Cache.InvalidateAll();
                            string refreshUrl = cp.Utils.ModifyLinkQueryString(cp.Request.Link, cp.Utils.GetRandomString(10), cp.Utils.GetRandomString(10));
                            refreshUrl = cp.Utils.ModifyLinkQueryString(refreshUrl, "importTypeId", cp.Doc.GetInteger("importTypeId"));
                            refreshUrl = cp.Utils.ModifyLinkQueryString(refreshUrl, "layoutFrameworkSelectionId", cp.Doc.GetInteger("layoutFrameworkSelectionId"));
                            refreshUrl = cp.Utils.ModifyLinkQueryString(refreshUrl, "layoutId", cp.Doc.GetInteger("layoutId"));
                            cp.Response.Redirect(refreshUrl);
                            return "html import success, redirect to refresh page";
                        }
                    }
                    cp.TempFiles.DeleteFolder(uploadFolderPath);
                }
                //
                // -- populate output form
                tool.title = "Html Importer";
                tool.description = cp.Html5.P("This tool uploads and imports html files and converts the html to Mustache-format compatible layouts, templates and addons. See reference at the end of this document.");
                {
                    //
                    // -- destination
                    var importTypeList = new List<string>() { "Import Destination set in Html File", "Layout For Addon", "Page Template", "Email Template", "Email" };
                    string editRow = cp.AdminUI.GetEditRow("Import Type", cp.AdminUI.GetLookupListEditor("importTypeId", importTypeList, cp.Doc.GetInteger("importTypeId"), "hiImportTypeid"),
                        "Select the type of html you are importing.", "");
                    tool.body += cp.Html5.Div(editRow, "", "hiImportType");
                }
                {
                    //
                    // -- select layout
                    string editRow = cp.AdminUI.GetEditRow("Layout to Update", cp.AdminUI.GetLookupContentEditor("layoutId", "Layouts", cp.Doc.GetInteger("layoutId")),
                        "Select the Layout you want to populate with this html document.", "hiSelectLayout");
                    tool.body += cp.Html5.Div(editRow, "", "hiSelectLayoutId");
                }
                {
                    //
                    // -- select layout bootstrap 4 or 5
                    var lookupList = new List<string>() { "default (bootstrap-4)", "bootstrap-5" };
                    int layoutFrameworkDefault = cp.Site.GetInteger("html platform version", 4);
                    int selectionIndex = cp.Doc.GetInteger("layoutFrameworkSelectionId", layoutFrameworkDefault== 5 ? 2 : 1);
                    //int selectionIndex  = layoutFrameworkSelected==5 ? 2 : 1;
                    string editRow = cp.AdminUI.GetEditRow("Layout for Bootstrap 4 or 5", cp.AdminUI.GetLookupListEditor("layoutFrameworkSelectionId", lookupList, selectionIndex, ""),
                        "Layout records can include html for default (bootswtrap-4) or bootstrap-5. The default layout is used if the site is set to bootstrap-4 or if the bootstrap-5 is blank.", "hiSelectLayout");
                    tool.body += cp.Html5.Div(editRow, "", "hiSelectLayoutFrameworkId");
                }
                {
                    //
                    // -- select page template
                    string editRow = cp.AdminUI.GetEditRow("Page Template to Update", cp.AdminUI.GetLookupContentEditor("pageTemplateId", "Page Templates", cp.Doc.GetInteger("pagetemplateId")),
                        "Select the Page Template you want to populate with this html document. " +
                        "Page templates import just the html body. Head metadata and associated resources have to be manually configured.", "hiSelectPageTemplate");
                    tool.body += cp.Html5.Div(editRow, "", "hiSelectPageTemplateId");
                }
                {
                    //
                    // -- select email template
                    string editRow = cp.AdminUI.GetEditRow("Email Template to Update", cp.AdminUI.GetLookupContentEditor("emailTemplateId", "Email Templates", cp.Doc.GetInteger("emailtemplateId")),
                        "Select the Email Template you want to populate with this html document. " +
                        "Email templates import the entire html document including the head tag elements. If they are missing the system will create them.", "hiSelectPageTemplate");
                    tool.body += cp.Html5.Div(editRow, "", "hiSelectEmailTemplateId");
                }
                {
                    //
                    // -- select email
                    string editRow = cp.AdminUI.GetEditRow("Email to Update", cp.AdminUI.GetLookupContentEditor("emailId", "Email", cp.Doc.GetInteger("emailId")),
                        "Select the Group, System or Conditional Email you want to populate with this html document. ", "hiSelectEmail");
                    tool.body += cp.Html5.Div(editRow, "", "hiSelectEmailId");
                }
                {
                    //
                    // -- file upload
                    string editRow = cp.AdminUI.GetEditRow("File Upload", cp.AdminUI.GetFileEditor(uploadFormInputName, ""), "Select the file you want to import. The file may include directives the determine the save location and Mustache replacements.");
                    tool.body += cp.Html5.Div(editRow, "", "hiUploadInput");
                }

                //
                tool.htmlAfterTable += cp.Html5.H4("Instructions");
                tool.htmlAfterTable += cp.Html5.Div(
                    cp.Html5.P("Use this tool to upload website files like HTML, CSS, and Javascript and to create/update template and layout records. Page templates are records with html content used as the base structure for web pages. Layouts are records with html content used by programs executed within add-ons.")
                    + cp.Html5.P("<b>Upload a single layout</b> -- Upload an html file, select the layout or template record where it should be saved, and select bootstrap 4 or 5. Only the html &lt;body&gt; content will be saved. If not body tag, the entire files will be used. ")
                    + cp.Html5.P("<b>Upload layout(s) with asset files</b> -- Create a zip file with html files and asset files like images, Javascript, and CSS. HTML files should include a meta tag that describes the layout or page template (see details below). Asset files and folders will be reproduced in the www folder as they are in the zip file.")
                    , "ml-4"
                );
                //
                tool.htmlAfterTable += cp.Html5.H5("Import Type");
                tool.htmlAfterTable += cp.Html5.Div(
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
                tool.htmlAfterTable += cp.Html5.H5("Mustache Properties and Data Properties");
                {
                    string indent = "";
                    indent += cp.Html5.P("There are two types of replacement properties supported. Data properties modify the html as described. Mustache properties create html that supports Mustache templating. Mustache is a popular templating scheme. You may choose to include mustache tags in your html directly in which case the html may not render well in a browser. You can alternativly choose to set special styles outlined here and the import tool will add the Mustache tags you indicate. Reference any of the many <a href=\"https://mustache.github.io/mustache.5.html\">Mustache references online</a>.");
                    tool.htmlAfterTable += cp.Html5.Div(indent, "ml-4");
                }
                //
                tool.htmlAfterTable += cp.Html5.H5("data-mustache-section");
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
                    tool.htmlAfterTable += cp.Html5.Div(indent, "ml-4");
                }
                //
                tool.htmlAfterTable += cp.Html5.H5("data-mustache-inverted-section");
                {
                    string indent = "";
                    indent += cp.Html5.P("Add a Mustache Inverted Section data attrbiute around content to be included if the object property value is false.");
                    indent += "<pre>"
                        + cp.Utils.EncodeHTML(""
                            + "\n<div data-mustache-inverted-section=\"emptyList\">"
                            + "\n\t<p>No items were found.</p>"
                            + "\n</div>"
                            + "\n<div>"
                            + "\n\t{{^emptyList}}"
                            + "\n\t<p>No items were found.</p>"
                            + "\n\t{{/itemList}}"
                            + "\n</div>"
                            + "")
                        + "</pre>";
                    tool.htmlAfterTable += cp.Html5.Div(indent, "ml-4");
                }
                //
                tool.htmlAfterTable += cp.Html5.H5("data-body");
                {
                    string sample = "";
                    sample += "<body><span data-body>This content will be included without the span tag</span> and this copy will not be imported</body>";
                    sample += "\nThis content will be included without the span tag";
                    string indent = "";
                    indent += cp.Html5.P("The data-body attribute is used to locate the html to be processed. Anything outside of this region will not be processed. If a data-body attribute is found, only the html within that element will be included. If no data-body is used, the content of the entire html body tag is imported.");
                    indent += "<pre>" + cp.Utils.EncodeHTML(sample) + "</pre>";
                    tool.htmlAfterTable += cp.Html5.Div(indent, "ml-4");
                }
                //
                tool.htmlAfterTable += cp.Html5.H5("data-layout");
                {
                    string sample = "";
                    sample += "<body><span data-layout=\"New-Site-Header\">This content will be saved to the layout named 'New-Site-Header' without the span tag</span> and this copy will not be imported. If a tag includes both a data-delete and a data-layout, the innter content will be saved to a layout and deleted from the html.</body>";
                    sample += "\nThis content will be included without the span tag";
                    string indent = "";
                    indent += cp.Html5.P("If a data-layout attribute is found, the html within that element will be saved to the named layout record.");
                    indent += "<pre>" + cp.Utils.EncodeHTML(sample) + "</pre>";
                    tool.htmlAfterTable += cp.Html5.Div(indent, "ml-4");
                }
                //
                tool.htmlAfterTable += cp.Html5.H5("data-cdn");
                {
                    string sample = "";
                    sample += "<body><div><img data-cdn=\"src\" src=\"/img/sample.png\"></div></body>";
                    sample += "\nWhen imported, the image in src will be copied to the cdn data source, and the html will be updated to target the cdn version.";
                    string indent = "";
                    indent += cp.Html5.P("Set data-cdn to an attribute in the html tag, like src, and the file in the url will be copied to the cdn and the html will be updated.");
                    indent += "<pre>" + cp.Utils.EncodeHTML(sample) + "</pre>";
                    tool.htmlAfterTable += cp.Html5.Div(indent, "ml-4");
                }
                //
                tool.htmlAfterTable += cp.Html5.H5("data-href");
                {
                    string sample = "";
                    sample += "<body><p><a href=\"MainMenu.html\" data-href=\"{{/mainmenu}}\">Click here to see the main menu.</a></p></body>";
                    sample += "\nThe html will click to MainMenu.html during design. When imported, it will click to /menumenu.";
                    string indent = "";
                    indent += cp.Html5.P("Adds an href to the current element, replacing what is there if it has one.");
                    indent += "<pre>" + cp.Utils.EncodeHTML(sample) + "</pre>";
                    tool.htmlAfterTable += cp.Html5.Div(indent, "ml-4");
                }
                //
                tool.htmlAfterTable += cp.Html5.H5("data-value");
                {
                    string sample = "";
                    sample += "<p>My example is <span value=\"0\" data-value=\"{{id}}\">content</span>.</p>";
                    sample += "\n<p>My example is <span value=\"{{id}}\">content</span>.</p>";
                    string indent = "";
                    indent += cp.Html5.P("Replace the value of the html tag with the provided value.");
                    indent += "<pre>" + cp.Utils.EncodeHTML(sample) + "</pre>";
                    tool.htmlAfterTable += cp.Html5.Div(indent, "ml-4");
                }
                //
                tool.htmlAfterTable += cp.Html5.H5("data-src");
                {
                    string sample = "";
                    sample += "<body><image src=\"placeholder-image.jpg\" data-src=\"{{user-photo}}\"></body>";
                    sample += "\nThe html will show placeholder-image.jpg. When imported, the src will be the mustache tag {{user-photo}}.";
                    string indent = "";
                    indent += cp.Html5.P("Adds an src to the current element, replacing what is there if it has one.");
                    indent += "<pre>" + cp.Utils.EncodeHTML(sample) + "</pre>";
                    tool.htmlAfterTable += cp.Html5.Div(indent, "ml-4");
                }
                tool.htmlAfterTable += cp.Html5.H5("data-alt");
                {
                    string sample = "";
                    sample += "<body><image src=\"image.jpg\" data-alt=\"{{photo-alt}}\"></body>";
                    sample += "\nThe html will have no alt tag. When imported, the alt tag will be {{photo-alt}}.";
                    string indent = "";
                    indent += cp.Html5.P("Adds an alt to the current element, replacing what is there if it has one.");
                    indent += "<pre>" + cp.Utils.EncodeHTML(sample) + "</pre>";
                    tool.htmlAfterTable += cp.Html5.Div(indent, "ml-4");
                }
                //
                tool.htmlAfterTable += cp.Html5.H5("data-addon");
                {
                    string sample = "";
                    sample += "<span data-addon=\"content_box\">content</span>";
                    sample += "\n<span>{% \"content box\" %}</span>";
                    string indent = "";
                    indent += cp.Html5.P("NOTE: If the addon name contains spaces, replace each space with a _ instead. Ex. content box would be content_box. Replace the inner content of the html tag with the addon after the Mustache Addon tag.");
                    indent += "<pre>" + cp.Utils.EncodeHTML(sample) + "</pre>";
                    tool.htmlAfterTable += cp.Html5.Div(indent, "ml-4");
                }
                //
                tool.htmlAfterTable += cp.Html5.H5("data-innertext");
                {
                    string sample = "";
                    sample += "<div><span data-innertext=\"{{myMustacheProperty}}\">content</span></div>";
                    sample += "\n<div><span>{{myMustacheProperty}}</span></div>";
                    string indent = "";
                    indent += "<pre>" + cp.Utils.EncodeHTML(sample) + "</pre>";
                    tool.htmlAfterTable += cp.Html5.Div(indent, "ml-4");
                }
                ////
                //tool.htmlAfterTable += cp.Html5.H5("data-outertext");
                //{
                //    string sample = "";
                //    sample += "<div><span data-outertext=\"{{myMustacheProperty}}\">content</span></div>";
                //    sample += "\n<div>{{myMustacheProperty}}</div>";
                //    string indent = "";
                //    indent += "<pre>" + cp.Utils.EncodeHTML(sample) + "</pre>";
                //    tool.htmlAfterTable += cp.Html5.Div(indent, "ml-4");
                //}
                //
                tool.htmlAfterTable += cp.Html5.H5("data-delete");
                {
                    string sample = "";
                    sample += "<p>This is in the layout.<span data-delete>This is not.</span></p>";
                    sample += "\n<p>This is in the layout.</p>";
                    string indent = "";
                    indent += cp.Html5.P("Delete the tag that contains this class, and all child tags.");
                    indent += "<pre>" + cp.Utils.EncodeHTML(sample) + "</pre>";
                    tool.htmlAfterTable += cp.Html5.Div(indent, "ml-4");
                }
                //
                // legacy, use data-value instead
                // 
                tool.htmlAfterTable += cp.Html5.H5("data-mustache-value");
                {
                    //string sample = "";
                    //sample += "<p>My example is <span value=\"0\" data-mustache-value=\"id\">content</span>.</p>";
                    //sample += "\n<p>My example is <span value=\"{{id}}\">content</span>.</p>";
                    string indent = "";
                    indent += cp.Html5.P("Legacy. Use data-value instead, adding your own mustache braces.");
                    //indent += "<pre>" + cp.Utils.EncodeHTML(sample) + "</pre>";
                    tool.htmlAfterTable += cp.Html5.Div(indent, "ml-4");
                }
                //
                tool.htmlAfterTable += cp.Html5.H5("data-mustache-variable");
                {
                    //string sample = "";
                    //sample += "<p>My name is <span data-mustache-variable=\"firstName\">Sample Name</span>.</p>";
                    //sample += "\n<p>My name is <span>{{{firstName}}}</span>.</p>";
                    string indent = "";
                    indent += cp.Html5.P("Legacy, use data-innertext instead.");
                    //indent += "<pre>" + cp.Utils.EncodeHTML(sample) + "</pre>";
                    tool.htmlAfterTable += cp.Html5.Div(indent, "ml-4");
                }

                tool.addFormButton("Upload");
                tool.addFormButton("Cancel");
                return tool.getHtml(cp);
            } catch (Exception ex) {
                //
                // -- the execute method should typically not throw an error into the consuming method. Log and return.
                cp.Site.ErrorReport(ex);
                throw;
            }
        }
    }
}