using Microsoft.VisualStudio.Shell;
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;


namespace TcUnit.Options
{
    [Guid("91bb19c8-9375-4b26-94cc-cee621b0bde7")]
    [ComVisible(true)]
    public class GeneralOptionsPage : UIElementDialogPage
	{
		public const string Category = "TwinCAT\\TcUnit";
		public const string Name = "General";
		protected override UIElement Child => new GeneralOptionsControl();
	}
}
