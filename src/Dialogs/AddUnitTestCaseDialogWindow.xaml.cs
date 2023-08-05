
using System.Windows;

namespace TcUnit_VsExtension.Dialogs
{
    /// <summary>
    /// Interaction logic for AddUnitTestCaseDialogWindow.xaml.
    /// </summary>
    public partial class AddUnitTestCaseDialogWindow : BaseDialogWindow
    {
        public string textboxName = "UnitTest";

        public AddUnitTestCaseDialogWindow()
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
