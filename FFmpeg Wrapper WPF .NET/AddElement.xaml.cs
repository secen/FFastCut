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

namespace FFmpeg_Wrapper_WPF.NET
{
    /// <summary>
    /// Interaction logic for AddElement.xaml
    /// </summary>
    public partial class AddElement : Window
    {
        public AddElement()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            if(dlg.ShowDialog() == true)
            {
                string fileName = dlg.FileName;
                sourceTextBox.Text = fileName;
            }
        }

    }
}
