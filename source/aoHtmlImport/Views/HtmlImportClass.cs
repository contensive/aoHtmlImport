
using System;
using Contensive.Addons.HtmlImport.Controllers;
using Contensive.Addons.HtmlImport.Models;
using Contensive.BaseClasses;

namespace Contensive.Addons.HtmlImport {
    namespace Views {
        //
        public class HtmlImportClass : AddonBaseClass {
            //
            public override object Execute(CPBaseClass cp) {
                try {
                    string button = cp.Doc.GetText("button");
                    if ( string.IsNullOrEmpty(button)) {
                        //
                        // -- process button
                    }
                    //
                    // -- create output form
                    //string result = cp.Html5.H2("Html Import");
                    //result += cp.Html5.P("Upload an html file.");
                    //result += cp.Html5.InputFile("uploadFilename", "custom-file-input", "customFile");
                    string layout = cp.Content.getLayout("Html Import");
                    layout = cp.Html5.Form(layout);
                    var htmlImportViewModel = new HtmlImportViewModel();
                    return Nustache.Core.Render.StringToString(layout, htmlImportViewModel);
                } catch (Exception ex) {
                    //
                    // -- the execute method should typically not throw an error into the consuming method. Log and return.
                    cp.Site.ErrorReport(ex);
                    return string.Empty;
                }
            }
        }
    }
}
