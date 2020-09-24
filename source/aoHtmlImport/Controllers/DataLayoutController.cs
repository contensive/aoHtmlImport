﻿
using System;
using System.Collections.Generic;
using Contensive.BaseClasses;
using Contensive.Models.Db;
using HtmlAgilityPack;
using static Contensive.Addons.HtmlImport.Constants;
using static Newtonsoft.Json.JsonConvert;

namespace Contensive.Addons.HtmlImport {
    namespace Controllers {
        // 
        // ====================================================================================================
        /// <summary>
        /// locate the data-layout="layoutname" attributes and save the inner html to the layout record
        /// </summary>
        public static class DataLayoutController {
            //
            public static void process(CPBaseClass cp, HtmlDocument htmlDoc, ref List<string> userMessageList) {
                //
                // -- data attribute
                {
                    string xPath = "//*[@data-layout]";
                    HtmlNodeCollection nodeList = htmlDoc.DocumentNode.SelectNodes(xPath);
                    if (nodeList != null) {
                        foreach (HtmlNode node in nodeList) {
                            string layoutRecordName = node.Attributes["data-layout"]?.Value;
                            node.Attributes.Remove("data-layout");
                            //
                            // -- body found, set the htmlDoc to the body
                            var layoutDoc = new HtmlDocument();
                            layoutDoc.LoadHtml(node.InnerHtml);
                            //
                            // -- process the layout 
                            DataDeleteController.process(layoutDoc);
                            MustacheVariableController.process(layoutDoc);
                            MustacheSectionController.process(layoutDoc);
                            MustacheTruthyController.process(layoutDoc);
                            MustacheInvertedSectionController.process(layoutDoc);
                            MustacheValueController.process(layoutDoc);
                            DataAddonController.process(cp, layoutDoc);
                            //
                            // -- save the alyout
                            LayoutModel layout = null;
                            if ((layout == null) && !string.IsNullOrWhiteSpace(layoutRecordName)) {
                                layout = DbBaseModel.createByUniqueName<LayoutModel>(cp, layoutRecordName);
                                if (layout == null) {
                                    layout = DbBaseModel.addDefault<LayoutModel>(cp);
                                    layout.name = layoutRecordName;
                                }
                                layout.layout.content = layoutDoc.DocumentNode.OuterHtml;
                                layout.save(cp);
                                userMessageList.Add("Saved Layout '" + layoutRecordName + "' from data-layout attribute.");
                            }
                        }
                    }
                }
            }
        }
    }
}
