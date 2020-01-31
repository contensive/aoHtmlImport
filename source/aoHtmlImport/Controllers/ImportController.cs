
using System;
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
            /// <param name="htmlSourcePathFilename"></param>
            /// <returns></returns>
            public static bool importHtmlFile(CPBaseClass cp, string htmlSourcePathFilename, int layoutId, int templateId, ref string returnStatusMessage) {
                try {
                    returnStatusMessage = "";
                    HtmlDocument htmlDoc = new HtmlDocument();
                    htmlDoc.Load(htmlSourcePathFilename);
                    if (htmlDoc == null) {
                        //
                        // -- body tag not found, import the whole document
                        returnStatusMessage += cp.Html.p("The uploaded file is empty.");
                        return false;
                    }
                    //
                    // -- search for meta name=template|layout content=recordaname
                    string layoutRecordName = string.Empty;
                    string templateRecordName = string.Empty;
                    var metadataList = htmlDoc.DocumentNode.SelectNodes("//meta");
                    if (metadataList != null) {
                        foreach (var metadataNode in metadataList) {
                            switch (metadataNode.GetAttributeValue("name", String.Empty).ToLowerInvariant()) {
                                case "layout": {
                                        layoutRecordName = metadataNode.GetAttributeValue("content", String.Empty);
                                        break;
                                    }
                                case "template": {
                                        templateRecordName = metadataNode.GetAttributeValue("content", String.Empty);
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
                    HtmlAgilityPack.HtmlNode bodyNode = htmlDoc.DocumentNode.SelectSingleNode("//body");
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
                    LayoutModel layout = null;
                    if ((layout == null) && !layoutId.Equals(0)) {
                        layout = DbBaseModel.create<LayoutModel>(cp, layoutId);
                        if (layout == null) {
                            returnStatusMessage += cp.Html.p("The layout selected could not be found.");
                            return false;
                        }
                        layout.layout.content = htmlDoc.DocumentNode.OuterHtml;
                        layout.save(cp);
                    }
                    if ((layout == null) && !string.IsNullOrWhiteSpace(layoutRecordName)) {
                        layout = DbBaseModel.createByUniqueName<LayoutModel>(cp, layoutRecordName);
                        if (layout == null) {
                            layout = DbBaseModel.addDefault<LayoutModel>(cp);
                            layout.name = layoutRecordName;
                        }
                        layout.layout.content = htmlDoc.DocumentNode.OuterHtml;
                        layout.save(cp);
                    }



                    PageTemplateModel template  = null;
                    if ((template == null) && !templateId.Equals(0)) {
                        template = DbBaseModel.create<PageTemplateModel>(cp, templateId);
                        if (template == null) {
                            returnStatusMessage += cp.Html.p("The template selected could not be found.");
                            return false;
                        }
                        template.bodyHTML = htmlDoc.DocumentNode.OuterHtml;
                        layout.save(cp);
                    }
                    if ((template == null) && !string.IsNullOrWhiteSpace(templateRecordName)) {
                        template = DbBaseModel.createByUniqueName<PageTemplateModel>(cp, templateRecordName);
                        if (template == null) {
                            template = DbBaseModel.addDefault<PageTemplateModel>(cp);
                            template.name = templateRecordName;
                        }
                        template.bodyHTML = htmlDoc.DocumentNode.OuterHtml;
                        template.save(cp);
                    }




                    if ((layout==null) && (template == null)) {
                        returnStatusMessage += cp.Html.p("No template or layout name could be determined. If not selected, a target must be included as a meta tag in the html document (&lt;meta name=\"layout\" content=\"LayoutName\"&gt; or &lt;meta name=\"template\" content=\"TemplateName\"&gt;)");
                        return false;
                    }
                    //
                    return true;
                } catch (Exception) {
                    throw;
                }
            }
        }
    }
}
