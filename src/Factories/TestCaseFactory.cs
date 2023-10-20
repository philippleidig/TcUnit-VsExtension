using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TCatSysManagerLib;
using TcUnit.Options;

namespace TcUnit.VisualStudio.Factories
{
    public class TestCaseFactory
    {
        private string TestCaseTemplate => TcUnitPackage.GetTestCaseTemplate().TestCaseTemplate;
        private Regex TestCaseNamingRegex => new Regex(TcUnitPackage.GetTestCaseTemplate().TestCaseNamingRegex);

        public void Create (string name, ITcSmTreeItem parent)
        {
            if(parent == null)
            {
                throw new ArgumentNullException(nameof(parent));
            }

            if(name == string.Empty)
            {
                throw new ArgumentOutOfRangeException(nameof(name));
            }

            if(!TestCaseNamingRegex.IsMatch(name))
            {
                throw new ArgumentOutOfRangeException();
            }

            ITcSmTreeItem testCase = parent.CreateChild(
                 name,
                 (int)TREEITEMTYPES.TREEITEMTYPE_PLCMETHOD,
                 null,
                 new string[] { ((int)IECLANGUAGETYPES.IECLANGUAGE_ST).ToString(), "", "PRIVATE" }
             );

            ITcPlcImplementation testCaseImpl = (ITcPlcImplementation)testCase;
            testCaseImpl.ImplementationText = TestCaseTemplate.Replace("{{TEST_NAME}}", name);

            ITcPlcImplementation testSuiteImpl = parent as ITcPlcImplementation;
            string impl = testSuiteImpl.ImplementationText;
            impl = string.Concat(impl, name + "();\r\n");
            testSuiteImpl.ImplementationText = impl;

        }
    }
}
