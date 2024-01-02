using System.Windows;

namespace TcUnit.VisualStudio.Dialogs
{
	/// <summary>
	/// Interaction logic for AddUnitTestCaseDialogWindow.xaml.
	/// </summary>
	public partial class AddUnitTestCaseDialog : Window
    {
        public string textboxName = "UnitTest";

        public AddUnitTestCaseDialog()
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
