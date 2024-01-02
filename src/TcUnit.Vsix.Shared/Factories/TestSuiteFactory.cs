using System;
using System.Text.RegularExpressions;
using TCatSysManagerLib;

namespace TcUnit.VisualStudio.Factories
{
    public class TestSuiteFactory
    {
        public void Create (string name, ITcSmTreeItem parent)
        {
            if(parent == null)
				throw new ArgumentNullException(nameof(parent));

			if (name == string.Empty)
				throw new ArgumentOutOfRangeException(nameof(name));

			string[] array = new string[] { "1", "Extends", "TcUnit.FB_TestSuite" };
            ITcSmTreeItem tcSmTreeItem5 = parent.CreateChild(name, 604, "", array);
        }
    }
}
