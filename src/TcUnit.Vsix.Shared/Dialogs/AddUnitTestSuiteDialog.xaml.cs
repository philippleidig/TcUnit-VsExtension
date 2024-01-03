using System.Windows;

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
