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
    public class MustacheFalseyControllerTests {
        [TestMethod()]
        public void processTest() {
            string test1Src = "<div class=\"mustache-falsey showIfFalse\">Sample Name</div>";
            string test1Expect = "<div>{{{^showIfFalse}}}Sample Name{{{/showIfFalse}}}</div>";
            //
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(test1Src);
            Controllers.MustacheFalseyController.process(htmlDoc);
            string test1Result = htmlDoc.DocumentNode.OuterHtml;
            //
            Assert.AreEqual(test1Expect, test1Result);
        }
    }
}