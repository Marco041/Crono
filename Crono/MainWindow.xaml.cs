using Crono.ViewModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Crono
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // ((MainViewModel)this.DataContext).StartLoadingCommesse();
            // this.SetBinding(WidthProperty, new Binding("ResWidth") { Source = (MainViewModel)this.DataContext, Mode = BindingMode.TwoWay });
            // this.SetBinding(HeightProperty, new Binding("ResHeight") { Source = (MainViewModel)this.DataContext, Mode = BindingMode.TwoWay });
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ((MainViewModel)this.DataContext).ResHeight = (int)MWindow.ActualHeight;
            ((MainViewModel)this.DataContext).ResWidth = (int)MWindow.ActualWidth;
        }
    }
}
