using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contensive.Addons.HtmlImport.Controllers
{
    public class MustacheValueController
    {
        //
        public static void process(HtmlDocument htmlDoc)
        {
            string xPath = "//*[contains(@class,'mustache-value')]";
            HtmlNodeCollection nodeList = htmlDoc.DocumentNode.SelectNodes(xPath);
            if (nodeList != null)
            {
                foreach (HtmlNode node in nodeList)
                {
                    IEnumerable<string> classList = node.GetClasses();
                    if (classList != null)
                    {
                        string lastClass = "";
                        foreach (string className in classList)
                        {
                            if (lastClass.Equals("mustache-value"))
                            {
                                node.SetAttributeValue("value", "{{" + className + "}}");
                                node.RemoveClass(className);
                                node.RemoveClass("mustache-value");
                                break;
                            }
                            lastClass = className;
                        }
                    }
                }
            }
            //return htmlDoc;
        }
    }
}
