
using System;
using System.Collections.Generic;
using Contensive.BaseClasses;
using HtmlAgilityPack;
using static Contensive.Addons.HtmlImport.Constants;
using static Newtonsoft.Json.JsonConvert;

namespace Contensive.Addons.HtmlImport {
    namespace Controllers {
        // 
        // ====================================================================================================
        /// <summary>
        /// process truthy (see tool form for details)
        /// </summary>
        public class MustacheTruthyController {
            //
            public static void process(HtmlDocument htmlDoc) {
                string xPath = "//*[contains(@class,'mustache-truthy')]";
                HtmlNodeCollection nodeList = htmlDoc.DocumentNode.SelectNodes(xPath);
                if (nodeList != null) {
                    foreach (HtmlNode node in nodeList) {
                        IEnumerable<string> classList = node.GetClasses();
                        if (classList != null) {
                            string lastClass = "";
                            foreach (string className in classList) {
                                if (lastClass.Equals("mustache-truthy")) {
                                    node.RemoveClass(lastClass);
                                    node.RemoveClass(className);
                                    var listClone = node.Clone();
                                    node.ChildNodes.Clear();
                                    node.AppendChild(HtmlNode.CreateNode("{{{#" + className + "}}}"));
                                    foreach (HtmlNode listChild in listClone.ChildNodes) {
                                        node.AppendChild(listChild);
                                    }
                                    node.AppendChild(HtmlNode.CreateNode("{{{/" + className + "}}}"));
                                    break;
                                }
                                lastClass = className;
                            }
                        }
                    }
                }
            }
        }
    }
}
