using Crono.Configuration;
using Crono.Model;
using Crono.ViewModel;
using Crono.Views;
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

namespace Crono.Views
{
    /// <summary>
    /// Interaction logic for FasiPage.xaml
    /// </summary>
    public partial class FasiPage : Page
    {
        public FasiPage(CommessaDto commessa)
        {
            InitializeComponent();
            ((FasiViewModel)DataContext).CommessaChanged(commessa);
        }

        /// <summary>
        /// Metodo usato per fare il binding della dimensione in pixel della finestra, non si riesce a fare direttamente dallo xaml
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
          
        }

        /// <summary>
        /// Metodo per gestire il drag&drop dei task per creare vincoli. 
        /// Setta l'oggetto di cui si è fatto il drag nel viewModel e poi richiama funzioni del framework per gestire gaficamente il drag
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void thumbConstraints_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ((FasiViewModel)this.DataContext).ConstraintSource = ((FrameworkElement)sender).DataContext as TaskBlockViewModel;
            DataObject dragData = new DataObject("");
        }

        /// <summary>
        /// Setta l'animazione per fare il drop di un task quando si entra in un altro
        /// </summary>
        private void bb_DragEnter(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Link;
        }
    }
}
