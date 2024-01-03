using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Windows.Input;

namespace TcUnit.Options
{
    public class GeneralOptionsViewModel : ObservableObject
    {
		public GeneralOptionsViewModel() 
		{
			_loadDefaultsCommand = new RelayCommand(LoadSettingsDefaults);

			General.Instance.Load();
			testSuiteNamingRegex = General.Instance.TestSuiteNamingRegex;
			testCaseNamingRegex = General.Instance.TestCaseNamingRegex;
			testCaseTemplate = General.Instance.TestCaseTemplate;
		}

		private string testSuiteNamingRegex;

		public string TestSuiteNamingRegex {
			get => testSuiteNamingRegex;
			set {
				General.Instance.TestSuiteNamingRegex = value;
				General.Instance.Save();
				SetProperty(ref testSuiteNamingRegex, value);
			}
		}

		private string testCaseNamingRegex;

		public string TestCaseNamingRegex {
			get => testCaseNamingRegex;
			set {
				General.Instance.TestCaseNamingRegex = value;
				General.Instance.Save();
				SetProperty(ref testCaseNamingRegex, value);
			}
		}

		private string testCaseTemplate;

		public string TestCaseTemplate {
			get => testCaseTemplate;
			set {
				General.Instance.TestCaseTemplate = value;
				General.Instance.Save();
				SetProperty(ref testCaseTemplate, value);
			}
		}

		private void LoadSettingsDefaults()
		{
			General.Instance.LoadDefaults();
			TestCaseTemplate = General.Instance.TestCaseTemplate;
			TestCaseNamingRegex = General.Instance.TestCaseNamingRegex;
			TestSuiteNamingRegex = General.Instance.TestSuiteNamingRegex;
		}

		private RelayCommand _loadDefaultsCommand;
		public ICommand LoadDefaults => _loadDefaultsCommand;
	}
}
