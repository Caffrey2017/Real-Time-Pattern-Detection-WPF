using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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



namespace WpfApp1
{
    public partial class MainWindow : Window
    {

        private void Page_One_Click(object sender, RoutedEventArgs e)
        {
            Main.Content = new Page1();
            
        }

        private void Page_Two_Click(object sender, RoutedEventArgs e)
        {
            Main.Content = new Page2();
        }

    }

    
}

