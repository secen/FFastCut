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
    /// Interaction logic for OptionsMenu.xaml
    /// </summary>
    public partial class OptionsMenu : Window
    {
        public OptionsMenu()
        {
            InitializeComponent();
            Loaded += OptionsMenu_Loaded;
        }

        private void OptionsMenu_Loaded(object sender, RoutedEventArgs e)
        {
            frame.NavigationService.Navigate(new AboutPage());
        }
    }
}
