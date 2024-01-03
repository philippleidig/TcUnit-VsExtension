using System;
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

			string[] settings = new string[] { "1", "Extends", "TcUnit.FB_TestSuite" };
            ITcSmTreeItem testSuite = parent.CreateChild(name, 604, "", settings);
        }
    }
}
