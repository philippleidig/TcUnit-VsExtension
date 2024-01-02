using System;
using System.Text.RegularExpressions;
using TCatSysManagerLib;

namespace TcUnit.VisualStudio.Factories
{
    public class TestCaseFactory
    {
        public void Create (string name, ITcSmTreeItem parent, string template)
        {
            if(parent == null)
				throw new ArgumentNullException(nameof(parent));

			if (name == string.Empty)
				throw new ArgumentOutOfRangeException(nameof(name));

			ITcSmTreeItem testCase = parent.CreateChild(
                 name,
                 (int)TREEITEMTYPES.TREEITEMTYPE_PLCMETHOD,
                 null,
                 new string[] { ((int)IECLANGUAGETYPES.IECLANGUAGE_ST).ToString(), "", "PRIVATE" }
             );

            ITcPlcImplementation testCaseImpl = (ITcPlcImplementation)testCase;
            testCaseImpl.ImplementationText = template.Replace("{{TEST_NAME}}", name);

            ITcPlcImplementation testSuiteImpl = parent as ITcPlcImplementation;
            string impl = testSuiteImpl.ImplementationText;
            impl = string.Concat(impl, name + "();\r\n");
            testSuiteImpl.ImplementationText = impl;

        }
    }
}
