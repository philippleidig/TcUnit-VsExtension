using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCatSysManagerLib;

namespace TcUnit_VsExtension
{
    public class TestCaseFactory
    {
        public static string TestCaseTemplate = @"
TEST('{{TEST_NAME}}');

// @TEST-FIXTURE

// @TEST-RUN

// @TEST-ASSSERT

TEST_FINISHED();";

        public void Create (string name, ITcSmTreeItem parent)
        {
            if(parent == null)
            {
                throw new ArgumentNullException();
            }

            if(name == string.Empty)
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
