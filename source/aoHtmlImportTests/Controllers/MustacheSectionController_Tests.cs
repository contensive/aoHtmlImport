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
    public class MustacheSectionController_Tests {
        //
        // -- class test
        [TestMethod()]
        public void LoopClassTest() {
            string test1Src = "<ul class=\"mustache-loop staff\"><li>Sample Name</li></ul>";
            string test1Expect = "<ul>{{#staff}}<li>Sample Name</li>{{/staff}}</ul>";
            //
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(test1Src);
            Controllers.MustacheSectionController.process(htmlDoc);
            string test1Result = htmlDoc.DocumentNode.OuterHtml;
            //
            Assert.AreEqual(test1Expect, test1Result);

        }
        //
        // -- data test
        [TestMethod()]
        public void LoopDataTest() {
            string test1Src = "<ul data-mustache-section=\"staff\"><li>Sample Name</li></ul>";
            string test1Expect = "<ul>{{#staff}}<li>Sample Name</li>{{/staff}}</ul>";
            //
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(test1Src);
            Controllers.MustacheSectionController.process(htmlDoc);
            string test1Result = htmlDoc.DocumentNode.OuterHtml;
            //
            Assert.AreEqual(test1Expect, test1Result);

        }
    }
}