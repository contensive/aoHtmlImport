
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
            public static bool importHtmlFile(CPBaseClass cp, string htmlSourcePathFilename, string layoutName, string templateName, ref string returnErrorMessage) {
                try {
                    HtmlDocument htmlDoc = new HtmlDocument();
                    htmlDoc.Load(htmlSourcePathFilename);
                    returnErrorMessage = default;
                    //
                    // -- get body
                    HtmlAgilityPack.HtmlNode bodyNode = htmlDoc.DocumentNode.SelectSingleNode("//body");
                    string body = bodyNode.InnerText;
                    //
                    // -- search for meta name=template|layout content=recordaname
                    var metadataList = htmlDoc.DocumentNode.SelectNodes("//meta");
                    string layoutRecordName = layoutName;
                    string templateRecordName = templateName;
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
                    if (string.IsNullOrWhiteSpace(layoutRecordName) && string.IsNullOrWhiteSpace(templateRecordName)) {
                        returnErrorMessage = "No template or layout name could be determined. If not selected, a target must be included as a meta tag in the html document (<meta name=\"layout\" content=\"LayoutName\"> or <meta name=\"template\" content=\"TemplateName\">)";
                        return false;
                    }
                    if (!string.IsNullOrWhiteSpace(layoutRecordName)) {
                        LayoutModel layout = DbBaseModel.createByUniqueName<LayoutModel>(cp, layoutRecordName);
                        if (layout == null) {
                            layout = DbBaseModel.addDefault<LayoutModel>(cp);
                            layout.name = layoutRecordName;
                        }
                        layout.layout.content = body;
                        layout.save(cp);
                    }
                    if (!string.IsNullOrWhiteSpace(templateRecordName)) {
                        PageTemplateModel template = DbBaseModel.createByUniqueName<PageTemplateModel>(cp, templateRecordName);
                        if (template == null) {
                            template = DbBaseModel.addDefault<PageTemplateModel>(cp);
                            template.name = templateRecordName;
                        }
                        template.bodyHTML = body;
                        template.save(cp);
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
