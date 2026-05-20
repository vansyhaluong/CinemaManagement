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
    /// Interaction logic for CustomMessageBox.xaml
    /// </summary>
    public partial class CustomMessageBox : Window
    {
        public bool Result { get; private set; }
        public enum MessageBoxType
        {
            Ok,
            YesNo
        }
        public CustomMessageBox(string title, string message, MessageBoxType type = MessageBoxType.Ok)
        {
            InitializeComponent();
            txtTitle.Text = title;
            txtMessage.Text = message;
            ConfigureButtons(type);
        }
        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            Result = true;
            this.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Result = false;
           this.Close();
        }
        private void ConfigureButtons(MessageBoxType type)
        {
            if (type == MessageBoxType.Ok)
            {
                btnOk.Visibility = Visibility.Visible;
                btnCancel.Visibility = Visibility.Collapsed;
            }
            else if (type == MessageBoxType.YesNo)
            {
                btnOk.Content = "Yes";
                btnCancel.Content = "No";

                btnOk.Visibility = Visibility.Visible;
                btnCancel.Visibility = Visibility.Visible;
            }
        }
    }
}
