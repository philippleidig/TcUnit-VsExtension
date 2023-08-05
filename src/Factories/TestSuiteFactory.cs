using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCatSysManagerLib;

namespace TcUnit_VsExtension
{
    public class TestSuiteFactory
    {
        public void Create (string name, ITcSmTreeItem parent)
        {
            if(parent == null)
            {
                throw new ArgumentNullException();
            }

            if (name == string.Empty)
            {
                throw new ArgumentOutOfRangeException();
            }

            string[] array = new string[] { "1", "Extends", "TcUnit.FB_TestSuite" };
            ITcSmTreeItem tcSmTreeItem5 = parent.CreateChild(name, 604, "", array);

            // instantiate in UNIT_TEST program
        }
    }
}
