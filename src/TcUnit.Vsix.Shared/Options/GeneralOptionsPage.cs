using Microsoft.VisualStudio.Shell;
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;


namespace TcUnit.Options
{
    [Guid("91bb19c8-9375-4b26-94cc-cee621b0bde7")]
    [ComVisible(true)]
    public class GeneralOptionsPage : DialogPage
    {
        private string testCaseTemplate = 
@"TEST('{{TEST_NAME}}');

// @TEST-FIXTURE

// @TEST-RUN

// @TEST-ASSSERT

TEST_FINISHED();";

        public string TestCaseTemplate
        {
            get { return testCaseTemplate; }
            set { testCaseTemplate = value; }
        }

        private string testCaseNamingRegex = "";
        public string TestCaseNamingRegex
        {
            get { return testCaseNamingRegex; }
            set { testCaseNamingRegex = value; }
        }

        private string testSuiteNamingRegex = "^[A-Z][a-zA-Z0-9]*_Tests$";
        public string TestSuiteNamingRegex
        {
            get { return testSuiteNamingRegex; }
            set { testSuiteNamingRegex = value; }
        }

        protected override IWin32Window Window => new GeneralOptionsControl(this);
    }
}
