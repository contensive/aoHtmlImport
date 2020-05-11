
using System;
using System.Text;
using Contensive.BaseClasses;
using Contensive.Models.Db;
using HtmlAgilityPack;

namespace Contensive.Addons.HtmlImport {
    namespace Controllers {
        public static class ImportController {
            //
            //====================================================================================================
            /// <summary>
            /// Import file as layout or template
            /// body is removed as the source
            /// <meta name="layout", content="NameOfLayout">
            /// meta name=template, content=NameOfTemplate
            /// </summary>
            /// <param name="cp"></param>
            /// <param name="htmlSourceTempPathFilename"></param>
            /// <returns></returns>
            public static bool importFile(CPBaseClass cp, string htmlSourceTempPathFilename, int importTypeId , int layoutId, int pageTemplateId, int emailTemplateId, int emailId, ref string returnStatusMessage) {
                try {
                    returnStatusMessage = "";
                    HtmlDocument htmlDoc = new HtmlDocument();
                    if (System.IO.Path.GetExtension(htmlSourceTempPathFilename).Equals(".zip")) {
                        //
                        // -- upload file is a zip and unzip to temp and copy assets to wwwroot
                        cp.TempFiles.UnzipFile(htmlSourceTempPathFilename);
                        cp.TempFiles.DeleteFile(htmlSourceTempPathFilename);
                        //
                        // -- copy non-html files to wwwroot
                        copyNonHtmlFilesToWWW(cp, cp.TempFiles.GetPath(htmlSourceTempPathFilename), "");
                    }
                    //
                    // -- import all html files in the root folder
                    string tempPath = cp.TempFiles.GetPath(htmlSourceTempPathFilename);
                    bool htmlFileFound = false;
                    foreach (var file in cp.TempFiles.FileList(tempPath)) {
                        if (file.Extension.ToLowerInvariant().Equals(".html")) {
                            htmlFileFound = true;
                            htmlDoc.Load(cp.TempFiles.PhysicalFilePath + tempPath + file.Name, Encoding.UTF8);
                            if (htmlDoc == null) {
                                //
                                // -- body tag not found, import the whole document
                                returnStatusMessage += cp.Html.p("The uploaded file is empty.");
                                return false;
                            }
                            if (!importHtmlDoc(cp, htmlDoc, importTypeId, layoutId, pageTemplateId, emailTemplateId, emailId, ref returnStatusMessage)) {
                                return false;
                            }
                        }
                    }
                    if (!htmlFileFound) {
                        returnStatusMessage += cp.Html.p("No files were found with .HTML extension. Only files with .HTML extensions are imported.");
                        return false;
                    }
                    return true;
                } catch (Exception ex) {
                    cp.Site.ErrorReport(ex);
                    throw;
                }
            }
            //
            //
            //====================================================================================================
            //
            public static bool importHtmlDoc(CPBaseClass cp, HtmlDocument htmlDoc, int importTypeId, int layoutId, int pageTemplateId, int emailTemplateId, int emailId, ref string returnStatusMessage) {
                //
                // -- search for meta name=template|layout content=recordaname
                string layoutRecordName = string.Empty;
                string pageTemplateRecordName = string.Empty;
                string emailTemplateRecordName = string.Empty;
                string emailRecordName = string.Empty;
                var metadataList = htmlDoc.DocumentNode.SelectNodes("//meta");
                if (metadataList != null) {
                    foreach (var metadataNode in metadataList) {
                        switch (metadataNode.GetAttributeValue("name", String.Empty).ToLowerInvariant()) {
                            case "layout": {
                                    layoutRecordName = metadataNode.GetAttributeValue("content", String.Empty);
                                    break;
                                }
                            case "template":
                            case "pagetemplate": {
                                    pageTemplateRecordName = metadataNode.GetAttributeValue("content", String.Empty);
                                    break;
                                }
                            case "emailtemplate": {
                                    emailTemplateRecordName = metadataNode.GetAttributeValue("content", String.Empty);
                                    break;
                                }
                            case "email": {
                                    emailRecordName = metadataNode.GetAttributeValue("content", String.Empty);
                                    break;
                                }
                            default: {
                                    break;
                                }
                        }
                    }
                }
                //
                // -- get body
                if (importTypeId!=4) {
                    HtmlNode bodyNode = htmlDoc.DocumentNode.SelectSingleNode("//body");
                    if (bodyNode == null) {
                        //
                        // -- no body found, use entire document
                        returnStatusMessage += cp.Html.p("The content does not include a <body> tag so the entire document will be imported.");
                    } else {
                        //
                        // -- use body
                        string body = bodyNode.InnerHtml;
                        if (string.IsNullOrWhiteSpace(body)) {
                            //
                            // -- body tag not found, import the whole document
                            returnStatusMessage += cp.Html.p("The content includes a <body> tag, but the body tag is empty.");
                            return false;
                        }
                        //
                        // -- body found, set the htmlDoc to the body
                        htmlDoc.LoadHtml(body);
                    }
                }
                //
                // -- process mustache nodes
                MustacheDeleteController.process(htmlDoc);
                MustacheBasicController.process(htmlDoc);
                MustacheLoopController.process(htmlDoc);
                MustacheTruthyController.process(htmlDoc);
                MustacheFalseyController.process(htmlDoc);
                MustacheValueController.process(htmlDoc);
                MustacheAddonController.process(htmlDoc);
                //
                // -- save manual layout
                LayoutModel layout = null;
                {
                    if (!layoutId.Equals(0)) {
                        layout = DbBaseModel.create<LayoutModel>(cp, layoutId);
                        if (layout == null) {
                            returnStatusMessage += cp.Html.p("The layout selected could not be found.");
                            return false;
                        }
                        layout.layout.content = htmlDoc.DocumentNode.OuterHtml;
                        layout.save(cp);
                    }
                    //
                    // -- save meta layout
                    if ((layout == null) && !string.IsNullOrWhiteSpace(layoutRecordName)) {
                        layout = DbBaseModel.createByUniqueName<LayoutModel>(cp, layoutRecordName);
                        if (layout == null) {
                            layout = DbBaseModel.addDefault<LayoutModel>(cp);
                            layout.name = layoutRecordName;
                        }
                        layout.layout.content = htmlDoc.DocumentNode.OuterHtml;
                        layout.save(cp);
                    }
                }
                //
                // -- save page template
                PageTemplateModel pageTemplate = null;
                {
                    if (!pageTemplateId.Equals(0)) {
                        pageTemplate = DbBaseModel.create<PageTemplateModel>(cp, pageTemplateId);
                        if (pageTemplate == null) {
                            returnStatusMessage += cp.Html.p("The template selected could not be found.");
                            return false;
                        }
                        pageTemplate.bodyHTML = htmlDoc.DocumentNode.OuterHtml;
                        pageTemplate.save(cp);
                    }
                    //
                    // -- save meta template
                    if ((pageTemplate == null) && !string.IsNullOrWhiteSpace(pageTemplateRecordName)) {
                        pageTemplate = DbBaseModel.createByUniqueName<PageTemplateModel>(cp, pageTemplateRecordName);
                        if (pageTemplate == null) {
                            pageTemplate = DbBaseModel.addDefault<PageTemplateModel>(cp);
                            pageTemplate.name = pageTemplateRecordName;
                        }
                        //
                        // -- try to resolve the various relative urls possible into the primary url, then to a reoot relative url
                        string urlProtocolDomainSlash = "https://" + cp.Site.DomainPrimary + "/";
                        string bodyhtml = htmlDoc.DocumentNode.OuterHtml;
                        bodyhtml = genericController.convertLinksToAbsolute(bodyhtml, urlProtocolDomainSlash);
                        bodyhtml = bodyhtml.Replace(urlProtocolDomainSlash, "/");
                        //
                        pageTemplate.bodyHTML = bodyhtml;
                        pageTemplate.save(cp);
                    }
                }
                //
                // -- save email template
                EmailTemplateModel emailTemplate = null;
                {
                    if (!emailTemplateId.Equals(0)) {
                        emailTemplate = DbBaseModel.create<EmailTemplateModel>(cp, emailTemplateId);
                        if (emailTemplate == null) {
                            returnStatusMessage += cp.Html.p("The template selected could not be found.");
                            return false;
                        }
                        emailTemplate.bodyHTML = htmlDoc.DocumentNode.OuterHtml;
                        emailTemplate.save(cp);
                    }
                    //
                    // -- save meta template
                    if ((emailTemplate == null) && !string.IsNullOrWhiteSpace(emailTemplateRecordName)) {
                        emailTemplate = DbBaseModel.createByUniqueName<EmailTemplateModel>(cp, emailTemplateRecordName);
                        if (emailTemplate == null) {
                            emailTemplate = DbBaseModel.addDefault<EmailTemplateModel>(cp);
                            emailTemplate.name = emailTemplateRecordName;
                        }
                        emailTemplate.bodyHTML = htmlDoc.DocumentNode.OuterHtml;
                        emailTemplate.save(cp);
                    }
                }
                //
                // -- save email
                EmailModel email = null;
                {
                    if (!emailId.Equals(0)) {
                        email = DbBaseModel.create<EmailModel>(cp, emailId);
                        if (email == null) {
                            returnStatusMessage += cp.Html.p("The email selected could not be found.");
                            return false;
                        }
                        email.copyFilename.content = htmlDoc.DocumentNode.OuterHtml;
                        email.save(cp);
                    }
                    //
                    // -- save meta template
                    if ((email == null) && !string.IsNullOrWhiteSpace(emailRecordName)) {
                        email = DbBaseModel.createByUniqueName<EmailModel>(cp, emailRecordName);
                        if (email == null) {
                            email = DbBaseModel.addDefault<EmailModel>(cp);
                            email.name = emailRecordName;
                        }
                        email.copyFilename.content = htmlDoc.DocumentNode.OuterHtml;
                        email.save(cp);
                    }
                }
                //
                // -- I had to do this quickly, refactor later
                if ((layout == null) && (pageTemplate == null) && (emailTemplate == null) && (email == null)) {
                    returnStatusMessage += cp.Html.p("No template or layout name could be determined. If not selected, a target must be included as a meta tag in the html document (&lt;meta name=\"layout\" content=\"LayoutName\"&gt; or &lt;meta name=\"template\" content=\"TemplateName\"&gt;)");
                    return false;
                }
                //
                return true;

            }
            //
            // ====================================================================================================
            /// <summary>
            /// copy all files from tempPath to wwwPath, except html files in the root.
            /// </summary>
            /// <param name="cp"></param>
            /// <param name="tempPath"></param>
            /// <param name="wwwPath"></param>
            public static void copyNonHtmlFilesToWWW(CPBaseClass cp, string tempPath, string wwwPath) {
                try {
                    foreach (var folder in cp.TempFiles.FolderList(tempPath)) {
                        string folderPath = folder.Name + "\\";
                        cp.WwwFiles.CreateFolder(wwwPath + folderPath);
                        copyNonHtmlFilesToWWW(cp, tempPath + folderPath, wwwPath + folderPath);
                    }
                    foreach (var file in cp.TempFiles.FileList(tempPath)) {
                        if (!string.IsNullOrEmpty(wwwPath) || !file.Extension.ToLowerInvariant().Equals(".html")) {
                            cp.TempFiles.Copy(tempPath + file.Name, wwwPath + file.Name, cp.WwwFiles);
                        }
                    }
                } catch (Exception ex) {
                    cp.Site.ErrorReport(ex);
                    throw;
                }
            }
        }
    }
}
