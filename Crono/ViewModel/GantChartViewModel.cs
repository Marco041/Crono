using Crono.Configuration;
using Crono.Configuration.Log;
using Crono.Model;
using Crono.Repository;
using Crono.Service;
using Crono.Utility;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;

namespace Crono.ViewModel
{
    /// <summary>
    /// ViewModel che gestisce la visualizzazione delle fasi su un canvas
    /// </summary>
    public class GantChartViewModel : ViewModelBase
    {    
        #region Properies
        private SolidColorBrush _backgroundColorTaskLight = (SolidColorBrush)(new BrushConverter().ConvertFrom("#c5e2c6")); //Highlight color of a phase when a constraint can be crated (droppen on that phase)
        private SolidColorBrush _backgrondColorTask = (SolidColorBrush)(new BrushConverter().ConvertFrom("#a5d6a7"));   //Phase background color
        private ObservableCollection<TaskBlockViewModel> _taskBlocks;   //List of the task
        private ObservableCollection<RowBlockViewModel> _rowCollection; //Row list (one row can contains more phases)
        private ICollectionView _taskList;  //Task lisk grouped by Id
        private ITempRepositoryService _tempService; 
        private IAnimationService _aniSevice;    //loading animation service
        private Commessa _commessaCorrente;    //Selected commision
        private IRepository _repository;    
        private IDaysService _dayService;    //Day list (column)
        private int _attractionRange;   //Initial drag value to allow phase shift
        private double _dayWidth;   //Column width
        private int _rowHeight;     //Phase height
        private int _rowMargin;     //Phase vertical margin
        private int _startRow;  //Y coordinate of the first phase
        private int _dayShift;  //Number of day to shift timespan
        private int _height;    //Canvas height
        private LogSplitter _log;   
        private bool _readOnlyMode;
        public int LeftListWidth
        {
            get;set;
        }
        public ObservableCollection<TaskBlockViewModel> TaskBlocks
        {
            get { return _taskBlocks; }
            set { _taskBlocks = value;
                RaisePropertyChanged("TaskBlocks");
            }
        }        
        public ObservableCollection<RowBlockViewModel> RowCollection
        {
            get
            {
                return _rowCollection;
            }
            set
            {
                _rowCollection = value;
                RaisePropertyChanged("RowCollection");
            }
        }
        public TaskBlockViewModel ConstraintSource { get; set; }
        public int Height
        {
            get
            {
                return _height;
            }
            set
            {
                _height = value;
                RaisePropertyChanged("Height");
            }
        }

        public ICollectionView TaskList {
            get
            {
                return _taskList;
            }
            set
            {
                _taskList = value;
                RaisePropertyChanged("TaskList");
            }
        }

        public bool ReadOnlyMode
        {
            get { return _readOnlyMode; }
            set { _readOnlyMode = value;
                RaisePropertyChanged("ReadOnlyMode");
            }
        }
        #endregion

        #region Command
        public RelayCommand<TaskBlockViewModel> OpenDetails { get; set; }   //Phase details modal
        public RelayCommand<DragDeltaEventArgs> DragLeftCommand { get; set; }   //Change start date of a phase
        public RelayCommand<DragDeltaEventArgs> DragRightCommand { get; set; }  //Change end date of a phase
        public RelayCommand<DragDeltaEventArgs> DragCommand { get; set; }   //Task shift
        public RelayCommand<TaskBlockViewModel> DragOverCommand { get; set; }   //Dropover on a phase during contraint line creation
        public RelayCommand<TaskBlockViewModel> DropCommand { get; set; }   //Drop of contratint line (contraint creation)
        public RelayCommand<TaskBlockViewModel> DragLeaveCommand { get; set; }  //Drop phace cancelled
        public RelayCommand<TaskBlockViewModel> DeleteCommand { get; set; }   //Delete task
        public RelayCommand<ConstraintViewModel> DeleteConstraintCommand { get; set; }  //Delete contratin   
        public RelayCommand<TaskBlockViewModel> OpenNewTaskModal { get; set; }  //New phase
        public RelayCommand<DragCompletedEventArgs> DragCompletedCommand { get; set; }  //End shift
        #endregion

        private DateTime StartDate => _dayService.StartDate;
        private DateTime EndDate => _dayService.EndDate;

        public GantChartViewModel(ICronoConfig config, IDaysService dayService, ITempRepositoryService tempService, IAnimationService aniService)
        {
            _aniSevice = aniService;
            RowCollection = new ObservableCollection<RowBlockViewModel>();
            _dayService = dayService;
            _repository = config.Repository;
            _attractionRange = config.AttractionRange;
            _rowHeight = config.RowHeight;
            _dayWidth = config.DayWidth;
            _rowMargin = config.RowMargin;
            _startRow = config.RowStart;
            _dayShift = config.DayShift;
            _tempService = tempService;
            LeftListWidth = config.CanvasReduceWidth;
            _log = config.Logger;
            TaskBlocks = new ObservableCollection<TaskBlockViewModel>();
            DragLeftCommand = new RelayCommand<DragDeltaEventArgs>(DragLeft);
            DragRightCommand = new RelayCommand<DragDeltaEventArgs>(DragRight);
            DragCommand = new RelayCommand<DragDeltaEventArgs>(Drag);
            DropCommand = new RelayCommand<TaskBlockViewModel>(TaskDrop);
            DragLeaveCommand = new RelayCommand<TaskBlockViewModel>(DragLeave);
            DeleteConstraintCommand = new RelayCommand<ConstraintViewModel>(DeleteConstraint);
            OpenNewTaskModal = new RelayCommand<TaskBlockViewModel>(NewTaskInPhase);
            DragCompletedCommand = new RelayCommand<DragCompletedEventArgs>(DragCompleted);
            DragOverCommand = new RelayCommand<TaskBlockViewModel>(DragOver);
            ServiceBus.SubscribeToCommessaChange(this, (CommessaDto c) => { if (c.ReadOnlyMode) GetAllFasi(c); else GetTasks(c); });
            ServiceBus.SubscribeToDateShiftChange(this, ShiftTime);
            ServiceBus.SubscribeToDateStartChange(this, Redraw);
            ServiceBus.SubscribeToCloseModal(this, SaveNewTask);
            ServiceBus.SubscribeToDateEndChange(this, AdjustConstraintLine);
            OpenDetails = new RelayCommand<TaskBlockViewModel>(OpenModalDetail);
        }

        private void DragCompleted(DragCompletedEventArgs args)
        {
            TaskBlockViewModel currentTask = (TaskBlockViewModel)((Thumb)args.Source).DataContext;
            currentTask.TaskModel.MoveIfHoliday(); 
        }

        /// <summary>
        /// Open modal details
        /// </summary>
        private void OpenModalDetail(TaskBlockViewModel t) => 
            ServiceBus.RaiseOpenModal(new TaskDetailsDto(
                TaskBlocks.Where(w => w.TaskModel.IdFase == t.TaskModel.IdFase).Select(s => s.TaskModel).ToList(), 
                TaskBlocks.Where(w=>w.TaskModel.CodiceCommessa.Equals(_commessaCorrente?.Codice)).Select(s => s.TaskModel).ToList(),false, ReadOnlyMode));

        /// <summary>
        /// On widnow reisize redraw contraint line
        /// </summary>
        /// <param name="newEnd"></param>
        private void AdjustConstraintLine(DateTime newEnd) => TaskBlocks.ToList().ForEach(i => i.ConstraintList.ToList().ForEach(j => j.UpdateConstraint()));
        
        /// <summary>
        /// New phase on the same line (duplication of phase)
        /// </summary>
        /// <param name="task"></param>
        private void NewTaskInPhase(TaskBlockViewModel task)
        {
            try
            {
                CronoTask n = new CronoTask()
                {
                    Id = _tempService.GetNewMaxId(),
                    IdFase = task.TaskModel.IdFase,
                    CodiceCommessa = task.TaskModel.CodiceCommessa,
                    Intervento = task.TaskModel.Intervento,
                    StartDate = task.TaskModel.EndDate.AddDays(1),
                    EndDate = task.TaskModel.EndDate.AddDays(1),
                };
                ServiceBus.RaiseOpenModal(new TaskDetailsDto(
                new List<CronoTask>() { n },
                TaskBlocks.Where(w => w.TaskModel.CodiceCommessa.Equals(task.TaskModel.CodiceCommessa)).Select(s => s.TaskModel).ToList(), true, ReadOnlyMode));
            }catch(Exception e)
            {
                _log.Error("Errore creazione nuova fase", e);
            }
        }

        /// <summary>
        /// Get all phase in a period (report)
        /// </summary>
        public async void GetAllFasi(CommessaDto cd)
        {
            try
            {
                Commessa c = cd.Comm;
                ReadOnlyMode = cd.ReadOnlyMode;
                _aniSevice.IsLoading = true;
                TaskBlocks.Clear();
                _commessaCorrente = c;
                DateTime start = cd.From;
                DateTime end = cd.To;
                int i = 0;
                var temp = new List<CronoTask>((await _repository.GetAllCommesseBetween(cd.From, cd.To))); //chiamata al repository che torna una lista piatta di tutti i task
                foreach (CronoTask item in temp)
                {
                    CreateTask(item, i);
                    var last = TaskBlocks.Last();
                        last.TaskModel.Intervento = last.TaskModel.CodiceCommessa + " " + last.TaskModel.Intervento;
                }
                foreach (TaskBlockViewModel taskVM in TaskBlocks)
                    CreateConstraints(taskVM);

                if (TaskBlocks.Count > 0)
                    Height = (int)TaskBlocks.Max(m => m.Y) + _rowMargin;    //l'altezza del canvas è pari alla y dell'ultimo task più il margine                
                UpdateListOfPhase();
                _aniSevice.IsLoading = false;
            }
            catch (Exception e)
            {
                _aniSevice.IsLoading = false;
                _log.Error("Errore nel recupero delle fasi", e);
            }
        }
        /// <summary>
        /// Load task from repository on creare line contratin 
        /// </summary>
        public async void GetTasks(CommessaDto cd)
        {
            try
            {
                Commessa c = cd.Comm;
                ReadOnlyMode = cd.ReadOnlyMode;
                _aniSevice.IsLoading = true;
                TaskBlocks.Clear();
                _commessaCorrente = c;
                DateTime start = _dayService.StartDate;
                DateTime end = _dayService.EndDate;
                int i = 0;
                var temp = new List<CronoTask>((await _repository.GetAllTask(c.Codice))); 
                foreach (CronoTask item in temp.Union(_tempService.GetAllAdded(c.Codice)))
                {
                    item.PropertyChanged += ModelTaskDateChanged;
                    if (!_tempService.IsDeleted(item))
                        if (_tempService.IsModified(item))
                            CreateTask(_tempService.GetModifed(item), i); //id a task has been modified is loaded from tempService otherwise from the repository
                        else
                            CreateTask(item, i);
                }
                foreach (TaskBlockViewModel taskVM in TaskBlocks)
                    CreateConstraints(taskVM);

                if(TaskBlocks.Count>0)
                    Height = (int)TaskBlocks.Max(m => m.Y) + _rowMargin;    //Canvas heigh is equals to Y coordinate of the last phase plus a margin              
                UpdateListOfPhase();
                _aniSevice.IsLoading = false;
            }
            catch (Exception e)
            {
                _aniSevice.IsLoading = false;
                _log.Error("Errore nel recupero delle fasi", e);
            }
        }

        private void CreateConstraints(TaskBlockViewModel taskVM)
        {
                    if (taskVM.TaskModel.Constraints.Count > 0)
                    {
                        foreach (CronoTask constraint in taskVM.TaskModel.Constraints)
                        {
                            //Check if the task has contraint on himself
                            if (!taskVM.ConstraintList.Select(s => s.ConstraintTask).Select(s => s.TaskModel).Contains(constraint))
                            {
                                taskVM.ConstraintLine = true;
                                TaskBlockViewModel taskConstraintVM = TaskBlocks.FirstOrDefault(w => w.TaskModel.Equals(constraint));
                                var cvm = new ConstraintViewModel(_dayService, taskConstraintVM, taskVM);
                                taskVM.ConstraintList.Add(cvm);
                                taskConstraintVM.Masters.Add(cvm);
                                UpdateGroup(taskConstraintVM.Group, taskVM.Group);  //Group is number that identify a group of task with contraint on themself
                                taskConstraintVM.Group = taskVM.Group;
                            }
                        }
                    }
                    else
                        taskVM.ConstraintLine = false;
        }

        /// <summary>
        /// Refresh of phase grouped by id phase
        /// </summary>
        private void UpdateListOfPhase()
        {
            var viewSource = new CollectionViewSource();
            viewSource.Source = TaskBlocks.GroupBy(g => g.TaskModel.IdFase).ToList().Select(s => s.First()).ToList();
            TaskList = viewSource.View;
        }

        /// <summary>
        /// Redraw task on date change
        /// </summary>
        /// <param name="e"></param>
        private void ModelTaskDateChanged(object sender, PropertyChangedEventArgs e)
        {
            try
            {
                switch (e.PropertyName)
                {
                    case nameof(CronoTask.StartDate):
                        ChangeTaskLeft(TaskBlocks.First(f => f.TaskModel.Equals(sender as CronoTask)));
                        break;
                    case nameof(CronoTask.EndDate):
                        ChangeTaskRight(TaskBlocks.First(f => f.TaskModel.Equals(sender as CronoTask)));
                        break;
                }
            }catch(Exception ex)
            {
                _log.Error("Errore aggiornamento posizione fase", ex);
            }
        }

        /// <summary>
        /// Update group of taks
        /// </summary>
        private void UpdateGroup(int groupOld, int newGroup) =>
            TaskBlocks.Where(w => w.Group == groupOld).ToList().ForEach(t => t.Group = newGroup);

        private TaskBlockViewModel CreateTask(CronoTask item, int i)
        {
            try
            {
                int currentHeight = _rowMargin + _startRow;
                if (TaskBlocks.Count > 0)
                    currentHeight = _rowMargin + (int)TaskBlocks.Max(m => m.Y);   //Current task y coordinate is last task y coordinate plus margin
                var row = RowCollection.FirstOrDefault(f => f.Task.FirstOrDefault(f2 => f2.TaskModel.IsSamePhase(item)) != null);
                row = row == null ? new RowBlockViewModel(0, currentHeight - 7, i) : row;
                RowCollection.Add(row);     //Task/row assotiation
                var fase = TaskBlocks.FirstOrDefault(s => s.TaskModel.IsSamePhase(item));
                if (fase != null)
                    currentHeight = (int)fase.Y;
                var startX = DateToPixel(item.StartDate);   //Date to pixel
                item.SetDuration();
                //ViewModel with task state creation
                TaskBlocks.Add(new TaskBlockViewModel(startX, currentHeight, DurationToPixel(item.StartDate, item.EndDate.AddDays(1)), -i, _backgrondColorTask, item, _rowHeight));
                row.Task.Add(TaskBlocks.Last());
                return TaskBlocks.Last();
            }catch(Exception e)
            {
                _log.Error($"Errore creazione nuovo task {item.Id}, {item.Intervento}, {item.StartDate}, {item.EndDate}", e);
                return null;
            }
        }

        /// <summary>
        /// Drag left thumb callback
        /// </summary>
        public void DragLeft(DragDeltaEventArgs args)
        {
            try
            {
                TaskBlockViewModel currentTask = (TaskBlockViewModel)((Thumb)args.Source).DataContext;  
                if (IsMoveLeftPossible(currentTask.TaskModel, Math.Sign(args.HorizontalChange)))   
                {
                    double currentX = args.HorizontalChange + currentTask.X;
                    var day = _dayService.GetStartDayBetween(currentX - _attractionRange, currentX + _attractionRange); 
                    if ((args.HorizontalChange > _attractionRange || args.HorizontalChange < -_attractionRange) && day != null && day.Day != currentTask.TaskModel.StartDate)
                        currentTask.TaskModel.ChangeStartDate(Math.Sign(args.HorizontalChange));
                }
            }catch(Exception e)
            {
                _log.Error("Errore drag left", e);
            }
        }
       
        /// <summary>
        /// Date start change callback
        /// </summary>
        private void ChangeTaskLeft(TaskBlockViewModel currentTask)
        {
            try
            {
                AddModified(currentTask.TaskModel);
                double newX = DateToPixel(currentTask.TaskModel.StartDate);
                currentTask.MoveLeft(newX);
                AddModified(currentTask.TaskModel);
                foreach (var masterTask in currentTask.Masters)
                    masterTask.AdjstConstraintLine(currentTask.TaskModel.StartDate);
            }catch(Exception e)
            {
                _log.Error("Errore modifica data di inizio", e);
            }
        }

        /// <summary>
        /// Drag right thumb callback
        /// </summary>
        public void DragRight(DragDeltaEventArgs args)
        {
            try
            {
                TaskBlockViewModel currentTask = (TaskBlockViewModel)((Thumb)args.Source).DataContext;
                if (IsMoveRightPossible(currentTask.TaskModel, Math.Sign(args.HorizontalChange)))
                {
                    double currentRight = args.HorizontalChange + currentTask.X + currentTask.Width;
                    var day = _dayService.GetEndDayBetween(currentRight - _attractionRange, currentRight + _attractionRange);
                    if ((args.HorizontalChange > _attractionRange || args.HorizontalChange < -_attractionRange) && day != null && (day.Day != currentTask.TaskModel.EndDate || day.Day != currentTask.TaskModel.EndDate.AddDays(1)))
                        currentTask.TaskModel.ChangeEndDate(Math.Sign(args.HorizontalChange));
                }
            }catch(Exception e)
            {
                _log.Error("Errore modifica drg right", e);
            }
        }

        /// <summary>
        /// Date end change callback
        /// </summary>
        private void ChangeTaskRight(TaskBlockViewModel currentTask)
        {
            try
            {
                AddModified(currentTask.TaskModel);
                double newX = DateToPixel(currentTask.TaskModel.EndDate);
                currentTask.MoveRight(newX, _dayWidth);
                AddModified(currentTask.TaskModel);
                foreach (var constraint in currentTask.ConstraintList)  //all contraint that start from this task must be redrawn
                {
                    constraint.ShiftWithoutDate(currentTask);
                }
            }catch(Exception e)
            {
                _log.Error("Errore modifica data di fine", e);
            }
        }

        /// <summary>
        /// Edit task in modal callback
        /// </summary>
        public void EditTask(TaskBlockViewModel task)
        {
            try
            {
                if (task.TempEndDate >= task.TempStartDate)
                {
                    double differenceS = (task.TempStartDate - task.TaskModel.StartDate).TotalDays;
                    double differenceE = (task.TempEndDate - task.TaskModel.EndDate).TotalDays;
                    task.SaveDetails();   
                    task.TaskModel.ChangeStartDate((int)differenceS);
                    task.TaskModel.ChangeEndDate((int)differenceE);
                    task.OpenDetails = false;   //modal close
                }
            }catch(Exception e)
            {
                _log.Error($"Errore durante la modifica manuale della fase {task?.TaskModel?.Id}", e);
            }
        }

        /// <summary>
        /// Move task callback
        /// </summary>
        public void Drag(DragDeltaEventArgs args)
        {
            try
            {
                TaskBlockViewModel currentTask = (TaskBlockViewModel)((Thumb)args.Source).DataContext;
                
                    double currentX = args.HorizontalChange + currentTask.X;
                    var day = _dayService.GetStartDayBetween(currentX - _attractionRange, currentX + _attractionRange);
                    if (day == null || day.Day.Equals(currentTask.TaskModel.StartDate))   //istruzioni per rendere possibile il movimento di un task fino alla sua data di fine pari al primo giorno del timestamp o viceversa data inizio task = data fine timespan
                        day = _dayService.GetEndDayBetween(currentX - _attractionRange + currentTask.Width, currentX + _attractionRange + currentTask.Width);
                    if (day == null && currentTask.TaskModel.StartDate <= StartDate && currentTask.TaskModel.EndDate >= EndDate.AddDays(-1))
                    {
                        if (args.HorizontalChange % _dayWidth < 5)
                            day = new DayBlockViewModel();
                    }
                    if ((args.HorizontalChange > _attractionRange || args.HorizontalChange < -_attractionRange) && day != null)                    
                        currentTask.TaskModel.ChangeBothDate(Math.Sign(args.HorizontalChange));
                    
            }catch(Exception e)
            {
                _log.Error("Errore durante il drag", e);
            }
        }
       
        /// <summary>
        /// Move task after timespan shift callback
        /// </summary>
        private void MoveTasks(TaskBlockViewModel currentTask, int days)
        {
            try
            {
                if (Math.Abs(currentTask.X) - Math.Sign(DateToPixel(currentTask.TaskModel.StartDate)) > 1 ||
                    Math.Abs(currentTask.X) - Math.Sign(DateToPixel(currentTask.TaskModel.StartDate)) < 1)
                {
                    double newX = currentTask.X;
                    newX = DateToPixel(currentTask.TaskModel.StartDate);
                    currentTask.Move(newX); 
                }
                foreach (var constraint in currentTask.ConstraintList)  //Move constraint
                {
                    constraint.ShiftWithoutDate(currentTask);
                }
            }catch(Exception e)
            {
                _log.Error("Errore durante lo spostamento di una fase", e);
            }
        }


        private double DateToPixel(DateTime day) => _dayService.DateToPixel(day);

        /// <summary>
        /// Check if a task can be moved to right
        /// </summary>
        private bool IsMoveRightPossible(CronoTask task, int sign)
        {
            if ((((task.EndDate - task.StartDate).TotalDays == 0 && sign == 1) || (task.EndDate - task.StartDate).TotalDays > 0) &&
                task.EndDate.AddDays(1) <= EndDate)
                return true;
            return false;
        }

        /// <summary>
        /// Check if date start of a task can be moved
        /// </summary>
        private bool IsMoveLeftPossible(CronoTask task, int sign)
        {
            if ((((task.EndDate - task.StartDate).TotalDays == 0 && sign == -1) || (task.EndDate - task.StartDate).TotalDays > 0) &&
                 task.StartDate >= StartDate)
                return true;
            return false;
        }

        private bool IsMovePossible(CronoTask task, int sign)
        {
            if (sign == -1 && task.EndDate.AddDays(-1) >= StartDate)
                return true;
            if (sign == 1 && task.StartDate.AddDays(1) <= EndDate)
                return true;
            return false;
        }

        /// <summary>
        /// Convert a date interval to pixel based on _daywidth
        /// </summary>
        public double DurationToPixel(DateTime start, DateTime end) => (end - start).TotalDays * _dayWidth;             

        private void AddCreatedTask()
        {
            if (_commessaCorrente != null)
            {
                foreach (CronoTask t in _tempService.GetAllAdded(_commessaCorrente.Codice))
                    if (!TaskBlocks.Select(s => s.TaskModel).Contains(t) && t.EndDate >= StartDate && t.StartDate <= EndDate)
                    {
                        if (TaskBlocks.Count > 0)
                            CreateTask(t, TaskBlocks.Last().Zindex);
                        else
                            CreateTask(t, 1);
                    }
            }
                
        }

        /// <summary>
        /// Contraint creation callback
        /// </summary>
        /// <param name="o">Task vincolato</param>
        public void TaskDrop(TaskBlockViewModel o)
        {
            o.Background = _backgrondColorTask;
            if (ConstraintSource != null && ConstraintSource!=o)   //This variable contains task 1 of the contraint
            {
                if (!ConstraintSource.TaskModel.Constraints.Contains(o.TaskModel))
                {
                    ConstraintSource.TaskModel.AddConstraint(o.TaskModel);
                    CreateSingleConstraint(o, ConstraintSource);
                    AddModified(ConstraintSource.TaskModel);
                }
            }
            ConstraintSource = null;
        }

        private void CreateSingleConstraint(TaskBlockViewModel o, TaskBlockViewModel source)
        {
            
            source.ConstraintLine = true;
            TaskBlockViewModel taskConstraintVM = TaskBlocks.FirstOrDefault(w => w.TaskModel.Equals(o.TaskModel));
            if (taskConstraintVM != null)
            {
                var c = TaskBlocks.FirstOrDefault(f => f.TaskModel.Equals(o.TaskModel));
                source.ConstraintList.Add(new ConstraintViewModel(_dayService, o, source));
                o.Masters.Add(source.ConstraintList.Last());
                UpdateGroup(taskConstraintVM.Group, source.Group);
                taskConstraintVM.Group = source.Group;                
            }
        }

        /// <summary>
        /// Date change callback
        /// </summary>
        public void Redraw(int difference)
        {
            try
            {
                foreach (var task in TaskBlocks)
                {
                    task.Deletable = true;
                    MoveTasks(task, difference);   //effettivo spostamento dei task
                }
            }
            catch (Exception e)
            {
                _log.Error("Errore durante lo shft del timespan", e);
            }
        }

        /// <summary>
        /// Timespan shift callback
        /// </summary>
        public void ShiftTime(DateTime oldDate)
        {
            try
            {
                double difference = (StartDate - oldDate).TotalDays;
                foreach (var task in TaskBlocks)
                {
                    task.Deletable = true;
                    MoveTasks(task, _dayShift * Math.Sign(difference));   //effettivo spostamento dei task
                }
                AddCreatedTask();
            }catch(Exception e)
            {
                _log.Error("Errore durante lo shft del timespan", e);
            }
        }

        /// <summary>
        /// Edit/Create phase callback
        /// </summary>
        /// <param name="taskList"></param>
        public void SaveNewTask(CronoTask[] taskList)
        {
            try
            {
                if (taskList != null)
                {
                    foreach (var task in taskList)
                    {
                        if (!TaskBlocks.Select(s=>s.TaskModel).Contains(task))    
                        {
                            //new
                            int zindex = 1;
                            if (TaskBlocks.Count > 0) zindex = TaskBlocks.Last().Zindex;
                            var taskVM = CreateTask(task, zindex);
                            if (task.Constraints.Count > 0) CreateConstraints(taskVM);
                            task.PropertyChanged += ModelTaskDateChanged;
                            Height = (int)TaskBlocks.Max(m => m.Y) + _rowMargin;    //Canvas height
                            AddNew(task);
                        }
                        else
                        {   //edit
                            ConstraintViewModel[] lista = TaskBlocks.First(f => f.TaskModel.Equals(task)).ConstraintList.ToArray();
                            foreach (var constraint in task.Constraints)                            
                                if (!lista.Select(s => s.ConstraintTask.TaskModel).Contains(constraint))                                
                                    CreateSingleConstraint(TaskBlocks.First(f => f.TaskModel.Equals(constraint)), TaskBlocks.First(f => f.TaskModel.Equals(task)));                                                            
                            for (int i = lista.Length - 1; i >= 0; i--)
                            {
                                if (!task.Constraints.Contains(lista[i].ConstraintTask.TaskModel))  //constraint delete
                                {
                                    DeleteConstraint(lista[i]);
                                }
                            }

                            AddModified(task);
                        }
                    }
                    UpdateListOfPhase();    //refresh
                }
            }catch(Exception e)
            {
                _log.Error("Errore durante il salvataggio", e);
            }
        }

        /// <summary>
        /// Move up the task is a row if deleted. 
        /// </summary>
        private void MoveUpTaskOnEmptyRow()
        {
            List<RowBlockViewModel> emptyRows = new List<RowBlockViewModel>();
            foreach (RowBlockViewModel row in RowCollection)
            {
                if (row.Task == null)
                {
                    emptyRows.Add(row); 
                }
                else
                {
                    if (emptyRows.Count > 0)
                    {
                        emptyRows.First().Task = row.Task; 
                        row.Task = null;
                        emptyRows.RemoveAt(0);
                        emptyRows.Add(row);
                    }
                }
            }
        }

        /// <summary>
        /// Delete contraint callback
        /// </summary>
        public void DeleteConstraint(ConstraintViewModel constraint)
        {
            CronoTask constraintTaskModel = constraint.ConstraintTask.TaskModel;
            constraint.MasterTask.TaskModel.RemoveConstraint(constraintTaskModel); 
            constraint.MasterTask.ConstraintList.RemoveAll(r => r.ConstraintTask.Equals(constraint.ConstraintTask));
            constraint.ConstraintTask.Masters .RemoveAll(r => r.MasterTask.Equals(constraint.MasterTask));
            if (constraint.MasterTask.ConstraintList.Count == 0)
                constraint.MasterTask.ConstraintLine = false;
            AddModified(constraint.MasterTask.TaskModel);
        }

        public void DragOver(TaskBlockViewModel o) => o.Background = _backgroundColorTaskLight;       
        public void DragLeave(TaskBlockViewModel o) => o.Background = _backgrondColorTask;        

        public void AddModified(CronoTask task) => _tempService.AddModifiedTask(task);
        public void AddNew(CronoTask task) => _tempService.AddNewTask(task);
    }
}
