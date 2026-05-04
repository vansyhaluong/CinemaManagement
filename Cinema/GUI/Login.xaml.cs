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
using System.Windows.Shapes;

namespace Cinema.GUI
{
	/// <summary>
	/// Interaction logic for Login.xaml
	/// </summary>
	public partial class Login : Window
	{
		public Login()
		{
			InitializeComponent();
		}
		private void btnExit_Click(object sender, RoutedEventArgs e)
		{
			MessageBoxResult result = MessageBox.Show(
	 "Are you sure you want to exit?",
	 "Exit Confirmation",
	 MessageBoxButton.YesNo,
	 MessageBoxImage.Question);
		}
		private void btnLogin_Click(object sender, RoutedEventArgs e)
		{

		}
	}
}
