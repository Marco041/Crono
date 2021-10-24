/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:Crono"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using CommonServiceLocator;
using Crono.Configuration;
using Crono.Service;
using Crono.Utility;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using System;

namespace Crono.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<CommesseViewModel>();
            SimpleIoc.Default.Register<FasiViewModel>();
            SimpleIoc.Default.Register<GantChartViewModel>();
            SimpleIoc.Default.Register<DetailsTaskViewModel>();
            SimpleIoc.Default.Register<ICronoConfig>(() => new CronoConfig());
            SimpleIoc.Default.Register<IAnimationService, AnimationService>();
            SimpleIoc.Default.Register<IDaysService, DaysService>();
            SimpleIoc.Default.Register<ITempRepositoryService, TempRepositoryService>();
            SetupNavigation();
            
        }

        private static void SetupNavigation()
        {
            var navigationService = new FrameNavigationService();
            navigationService.Configure("Commesse", new Uri("Crono.Views.CommessePage", UriKind.Relative));
            navigationService.Configure("Fasi", new Uri("Crono.Views.FasiPage", UriKind.Relative));
            SimpleIoc.Default.Register<IFrameNavigationService>(() => navigationService);
        }

        public MainViewModel Main
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainViewModel>();
            }
        }
        
        public CommesseViewModel CommesseViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<CommesseViewModel>();
            }
        }
        public FasiViewModel FasiViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<FasiViewModel>();
            }
        }

        public DayBlockViewModel DayBlockViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<DayBlockViewModel>();
            }
        }

        public GantChartViewModel GantChartViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<GantChartViewModel>();
            }
        }

        public RowBlockViewModel RowBlockViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<RowBlockViewModel>();
            }
        }

        public TaskBlockViewModel TaskBlockViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<TaskBlockViewModel>();
            }
        }

        public DetailsTaskViewModel DetailsTaskViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<DetailsTaskViewModel>();
            }
        }

        public ConstraintViewModel ConstraintViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ConstraintViewModel>();
            }
        }

        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}