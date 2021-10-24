using Crono.Configuration;
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

namespace Crono.Views
{
    /// <summary>
    /// Interaction logic for TaskDetails.xaml
    /// </summary>
    public partial class TaskDetails : UserControl
    {
        public TaskDetails()
        {
            try
            {
                InitializeComponent();
            }catch(Exception e) { }
        }
    }
}
