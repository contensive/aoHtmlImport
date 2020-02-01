
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
        public class ProcessIgnoreController {
            //
            public static HtmlDocument process(CPBaseClass cp, HtmlDocument htmlDoc) {
                string xPath = "//*[contains(@class,'mustache-ignore')]";
                HtmlNodeCollection nodeList = htmlDoc.DocumentNode.SelectNodes(xPath);
                if(nodeList!=null) {
                    foreach (HtmlNode node in nodeList) {
                        node.ParentNode.RemoveChild(node);
                        //node.RemoveAll();
                    }
                }
                return htmlDoc;
            }
        }
    }
}
