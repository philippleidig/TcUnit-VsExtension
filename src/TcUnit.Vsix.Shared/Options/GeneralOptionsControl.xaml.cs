using System.Drawing;
using System.Text.RegularExpressions;
using System;
using System.Windows.Controls;
using System.Windows.Documents;
using Microsoft.VisualStudio.PlatformUI;
using System.Windows.Media;
using System.Windows;

namespace TcUnit.Options
{
	/// <summary>
	/// Interaktionslogik für GeneralOptionsControl.xaml
	/// </summary>
	public partial class GeneralOptionsControl : UserControl
	{
		public GeneralOptionsControl()
		{
			InitializeComponent();
			DataContext = new GeneralOptionsViewModel();
		}
	}
}
