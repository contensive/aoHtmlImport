using Microsoft.VisualStudio.TestTools.UnitTesting;
using Contensive.Addons.HtmlImport.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace Contensive.Addons.HtmlImport.Controllers.Tests {
    [TestClass()]
    public class MustacheBasicControllerTests {
        [TestMethod()]
        public void processTest() {
            string test1Src = "<p><span class=\"mustache-basic abc\">ERROR</span>.</p>";
            string test1Expect = "<p><span>{{{abc}}}</span>.</p>";
            //
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(test1Src);
            Controllers.MustacheBasicController.process(htmlDoc);
            string test1Result = htmlDoc.DocumentNode.OuterHtml;
            //
            Assert.AreEqual(test1Expect, test1Result);

        }
    }
}