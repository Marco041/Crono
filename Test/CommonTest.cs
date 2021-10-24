using Crono.Configuration;
using Crono.Repository;
using Crono.Service;
using Crono.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace Test
{
    public class CommonTest
    {
        private ICronoConfig _config;
        protected ITempRepositoryService _tempService;
        protected IDaysService _dayService;
        protected IAnimationService _aniService;


        public CommonTest()
        {
            _config = new CronoConfig();
            
            _tempService = new TempRepositoryService();
            _dayService = new DaysService(_config);
            _aniService = new AnimationService();
        }

        protected void DragRight(int taskId, GantChartViewModel viewModel, int count = 1)
        {
            for (int i = 0; i < count; i++)
                viewModel.Drag(ShiftArgs(11, taskId, viewModel) as DragDeltaEventArgs);
        }

        protected void DragLeft(int taskId, GantChartViewModel viewModel, int count = 1)
        {
            for (int i = 0; i < count; i++)
                viewModel.Drag(ShiftArgs(-11, taskId, viewModel) as DragDeltaEventArgs);
        }

        protected RoutedEventArgs ShiftArgs(int horizontalChange, int taskId, GantChartViewModel viewModel)
        {
            RoutedEventArgs args = new DragDeltaEventArgs(horizontalChange, 0);
            args.Source = new Thumb() { DataContext = viewModel.TaskBlocks.First(f => f.TaskModel.Id == taskId) };
            return args;
        }

        protected FasiViewModel GetMainVM() => new FasiViewModel(_config, _dayService, _tempService, _aniService, null);
        protected GantChartViewModel GetGanttVM() => new GantChartViewModel(_config, _dayService, _tempService, _aniService);
        protected DetailsTaskViewModel GetDetailVM() => new DetailsTaskViewModel();
        
        
    }
}
