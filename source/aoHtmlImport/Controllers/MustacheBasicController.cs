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
        /// process ignore (see tool form for details)
        /// </summary>
        public class MustacheBasicController {
            //
            public static HtmlDocument process(CPBaseClass cp, HtmlDocument htmlDoc) {
                string xPath = "//*[contains(@class,'mustache-basic')]";
                HtmlNodeCollection nodeList = htmlDoc.DocumentNode.SelectNodes(xPath);
                if (nodeList != null) {
                    foreach (HtmlNode node in nodeList) {
                        IEnumerable<string> classList = node.GetClasses();
                        if (classList != null) {
                            string lastClass = "";
                            foreach (string className in classList) {
                                if (lastClass.Equals("mustache-basic")) {
                                    node.InnerHtml = "{{" + className + "}}";
                                    break;
                                }
                                lastClass = className;
                            }
                        }
                    }
                }
                return htmlDoc;
            }
        }
    }
}
