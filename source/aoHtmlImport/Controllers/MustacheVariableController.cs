﻿
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
        /// process delete (see tool form for details)
        /// </summary>
        public class MustacheVariableController {
            //
            public static void process(HtmlDocument htmlDoc) {
                {
                    //
                    // -- class mustache-basic
                    string xPath = "//*[contains(@class,'mustache-basic')]";
                    HtmlNodeCollection nodeList = htmlDoc.DocumentNode.SelectNodes(xPath);
                    if (nodeList != null) {
                        foreach (HtmlNode node in nodeList) {
                            IEnumerable<string> classList = node.GetClasses();
                            if (classList != null) {
                                string lastClass = "";
                                foreach (string className in classList) {
                                    if (lastClass.Equals("mustache-basic")) {
                                        node.InnerHtml = "{{{" + className + "}}}";
                                        node.RemoveClass(className);
                                        node.RemoveClass("mustache-basic");
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
                    // -- data-mustach-section
                    string xPath = "//*[@data-mustache-variable]";
                    HtmlNodeCollection nodeList = htmlDoc.DocumentNode.SelectNodes(xPath);
                    if (nodeList != null) {
                        foreach (HtmlNode node in nodeList) {
                            string listPropertyName = node.Attributes["data-mustache-variable"]?.Value;
                            node.Attributes.Remove("data-mustache-variable");
                            node.InnerHtml = "{{{" + listPropertyName + "}}}";
                        }
                    }
                }
            }
        }
    }
}
