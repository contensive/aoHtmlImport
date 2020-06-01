
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
        /// process loop (see tool form for details)
        /// </summary>
        public class MustacheSectionController {
            public static void process(HtmlDocument htmlDoc) {
                {
                    //
                    // -- legacy class - mustache-loop
                    string xPath = "//*[contains(@class,'mustache-loop')]";
                    HtmlNodeCollection nodeList = htmlDoc.DocumentNode.SelectNodes(xPath);
                    if (nodeList != null) {
                        foreach (HtmlNode node in nodeList) {
                            IEnumerable<string> classList = node.GetClasses();
                            if (classList != null) {
                                string lastClass = "";
                                foreach (string className in classList) {
                                    if (lastClass.Equals("mustache-loop")) {
                                        node.RemoveClass(lastClass);
                                        node.RemoveClass(className);
                                        var listClone = node.Clone();
                                        //HtmlNode.CreateNode(node.InnerHtml);
                                        node.ChildNodes.Clear();
                                        node.AppendChild(HtmlNode.CreateNode("{{#" + className + "}}"));
                                        foreach (HtmlNode listChild in listClone.ChildNodes) {
                                            node.AppendChild(listChild);
                                        }
                                        node.AppendChild(HtmlNode.CreateNode("{{/" + className + "}}"));
                                        break;
                                    }
                                    lastClass = className;
                                }
                            }
                        }
                    }
                }
                {
                    //
                    // -- data-mustache-section
                    string xPath = "//*[@data-mustache-section]";
                    HtmlNodeCollection nodeList = htmlDoc.DocumentNode.SelectNodes(xPath);
                    if (nodeList != null) {
                        foreach (HtmlNode node in nodeList) {
                            var listClone = node.Clone();
                            string sectionName = node.Attributes["data-mustache-section"].Value;
                            node.Attributes.Remove("data-mustache-section");
                            node.ChildNodes.Clear();
                            node.AppendChild(HtmlNode.CreateNode("{{#" + sectionName + "}}"));
                            foreach (HtmlNode listChild in listClone.ChildNodes) {
                                node.AppendChild(listChild);
                            }
                            node.AppendChild(HtmlNode.CreateNode("{{/" + sectionName + "}}"));
                        }
                    }
                }
            }
        }
    }
}
