using Crono.Configuration;
using Crono.Utility;
using Crono.ViewModel;
using Crono.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crono
{
    static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            var container = args.Length>0?args[0]:null;
            var app = new App();
            app.InitializeComponent();
            var mainWindow = new MainWindow();
            mainWindow.Closed += (object sender, EventArgs e) => app.Shutdown();
            app.Run(mainWindow);
        }
       
    }
}
