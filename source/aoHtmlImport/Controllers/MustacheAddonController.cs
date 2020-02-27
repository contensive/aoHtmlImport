using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contensive.Addons.HtmlImport.Controllers {
    public class MustacheAddonController {
        //
        public static void process(HtmlDocument htmlDoc) {
            string xPath = "//*[contains(@class,'mustache-addon')]";
            HtmlNodeCollection nodeList = htmlDoc.DocumentNode.SelectNodes(xPath);
            if (nodeList != null) {
                foreach (HtmlNode node in nodeList) {
                    IEnumerable<string> classList = node.GetClasses();
                    if (classList != null) {
                        string lastClass = "";
                        foreach (string className in classList) {
                            if (lastClass.Equals("mustache-addon")) {
                                string addon = className.Replace("_", " ");
                                node.InnerHtml = "{% \"" + addon + "\" %}";
                                node.RemoveClass(className);
                                node.RemoveClass("mustache-addon");
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