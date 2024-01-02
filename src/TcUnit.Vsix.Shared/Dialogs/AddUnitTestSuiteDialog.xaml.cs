using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace TcUnit
{
	/// <summary>
	/// Interaktionslogik für AddUnitTestSuiteDialog.xaml
	/// </summary>
	public partial class AddUnitTestSuiteDialog : Window
	{
		public string textboxName = "UnitTestSuite";
		public AddUnitTestSuiteDialog()
		{
			InitializeComponent();
		}

		private void BtnOpen_Click(object sender, RoutedEventArgs e)
		{
			if (txtName.Text.Length > 1)
			{
				textboxName = txtName.Text;
				DialogResult = true;
				Close();
			}
		}

		private void BtnCancel_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
		}
	}
}
