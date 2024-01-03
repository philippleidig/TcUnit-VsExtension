using Community.VisualStudio.Toolkit;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;

namespace TcUnit.Options
{
	internal partial class OptionsProvider
	{
		[ComVisible(true)]
		public class GeneralOptions : BaseOptionPage<General> { }
	}

	public class General : BaseOptionModel<General>
	{
		private const string TestCaseTemplateDefault =
@"TEST('{{TEST_NAME}}');

// @TEST-FIXTURE

// @TEST-RUN

// @TEST-ASSSERT

TEST_FINISHED();";
		private const string TestCaseNamingRegexDefault = "";
		private const string TestSuiteNamingRegexDefault = "^[A-Z][a-zA-Z0-9]*_Tests$";

		public string TestCaseTemplate { get; set; } = TestCaseTemplateDefault;
		public string TestCaseNamingRegex { get; set; } = TestCaseNamingRegexDefault;
		public string TestSuiteNamingRegex { get; set; } = TestSuiteNamingRegexDefault;

		public void LoadDefaults()
		{
			Instance.TestCaseTemplate = TestCaseTemplateDefault;
			Instance.TestCaseNamingRegex = TestCaseNamingRegexDefault;
			Instance.TestSuiteNamingRegex = TestSuiteNamingRegexDefault;
			Instance.Save();
		}
	}
}
