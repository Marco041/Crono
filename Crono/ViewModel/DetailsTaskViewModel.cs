using Crono.Configuration;
using Crono.Model;
using Crono.Repository;
using Crono.Service;
using Crono.Utility;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crono.ViewModel
{
    /// <summary>
    /// Classe che gestisce la modale per creare o modificare una fase
    /// </summary>
    public class DetailsTaskViewModel : ViewModelBase
    {
        private bool _openModalNewTask;   //indicates if open the new task modal
        private Commessa _commessaCorrente; 
        private CronoTask[] _backupCopy;    //copy of the task. Edit are saved only after Save button click.
        private CronoTask _newTask;     
        private bool _endDateModality;     //Mode where user specify end date of a new task 
        private bool _durationModality;    //Mode where user spficy day duration of a new task  
        private CronoTask[] _taskList;  //Phase show inside the edit modal (for constraint creation)
        private List<Tuple<DateTime, DateTime>> _periodList;    //Period of the phase (if more than one phase is on the same line)
        private Tuple<DateTime, DateTime> _selectedPeriod;     //Period selected of the phase to edit (if the phase is not duplicated on the same line the period is only one)
        private bool _showDetails; 
        private bool _showCombo;    //show periodo combo selection (if more than one phase is on the same line)
        private List<CronoTask> _allTask;
        private CronoTask _selectedConstraint;
        private List<CronoTask> _allConstraint;
        private bool _isConstraintVisible;

        public bool IsConstraintVisible
        {
            get { return _isConstraintVisible; }
            set { _isConstraintVisible = value; RaisePropertyChanged("IsConstraintVisible"); }
        }

        public CronoTask SelectedConstraint
        {
            get { return _selectedConstraint; }
            set { _selectedConstraint = value; RaisePropertyChanged("SelectedConstraint"); }
        }

        public List<CronoTask>  Alltask
        {
            get { return _allTask; }
            set { _allTask = value; RaisePropertyChanged("AllTask"); }
        }

        public bool IsOpenModalNewTask
        {
            get { return _openModalNewTask; }
            set { _openModalNewTask = value; RaisePropertyChanged("IsOpenModalNewTask"); }
        }
        public Commessa CommessaCorrente
        {
            get { return _commessaCorrente; }
            set { _commessaCorrente = value; }
        }
        public CronoTask NewTask {
            get
            {
                return _newTask;
            }
            set
            {
                _newTask = value;
                RaisePropertyChanged("NewTask");
            }
        }
        public bool EndDateModality
        {
            get
            {
                return _endDateModality;
            }
            set
            {
                _endDateModality = value;
                _durationModality = !value;
                RaisePropertyChanged("EndDateModality");
                RaisePropertyChanged("DurationModality");
            }
        }

        public bool DurationModality
        {
            get
            {
                return _durationModality;
            }
            set
            {
                _durationModality = value;
                _endDateModality = !value;
                RaisePropertyChanged("DurationModality");
                RaisePropertyChanged("EndDateModality");
            }
        }



        public CronoTask[] BackupCopy
        {
            get { return _backupCopy; }
            set { _backupCopy = value; RaisePropertyChanged("BackupCopy"); }
        }

        public List<Tuple<DateTime, DateTime>> PeriodList
        {
            get
            {
                return _periodList;
            }
            set
            {
                _periodList = value;
                RaisePropertyChanged("PeriodList");
            }
        }
        public Tuple<DateTime, DateTime> SelectedPeriod
        {
            get
            {
                return _selectedPeriod;
            }
            set
            {
                _selectedPeriod = value;
                RaisePropertyChanged("SelectedPeriod");
                if(value!=null)
                    SetTask(_backupCopy.FirstOrDefault(w => w.StartDate.Equals(value.Item1) && w.EndDate.Equals(value.Item2)));

            }
        }
        public bool ShowDetails        
        {
            get { return _showDetails; }
            set { _showDetails = value; RaisePropertyChanged("ShowDetails"); }
        }
        public bool ShowCombo
        {
            get { return _showCombo; }
            set { _showCombo = value; RaisePropertyChanged("ShowCombo"); }
        }
        private bool _enableNewTaskCreation;
        public bool EnableNewTaskCreation
        {
            get { return _enableNewTaskCreation; }
            set { _enableNewTaskCreation = value; RaisePropertyChanged("EnableNewTaskCreation"); }
        }
        private bool _editMode;
        public bool EditMode
        {
            get { return _editMode; }
            set
            {
                _editMode = value;
                RaisePropertyChanged("EditMode");
            }
        }
        public RelayCommand CloseModalCommand { get; set; }
        public RelayCommand SaveTaskCommand { get; set; }
        public RelayCommand<CronoTask> DeleteConstraintCommand { get; set; }
        public RelayCommand AddConstraintCommand { get; set; }

        public DetailsTaskViewModel(){
            EndDateModality = false;
            DurationModality = false;
            IsOpenModalNewTask = false;
            CloseModalCommand = new RelayCommand(CloseModal);
            SaveTaskCommand = new RelayCommand(Save);
            DeleteConstraintCommand = new RelayCommand<CronoTask>(DeleteConstraint);
            AddConstraintCommand = new RelayCommand(AddConstraint);
            ServiceBus.SubscribeToOpenModal(this, SetModalTask);
            IsConstraintVisible = false;
            EnableNewTaskCreation = true;
        }

        private void AddConstraint()
        {
            if (SelectedConstraint != null)
            {
                NewTask.Constraints.Add(SelectedConstraint);
                RaisePropertyChanged("Newtask");
                Alltask = _allConstraint.Where(w => !NewTask.Constraints.Contains(w) && !w.Equals(NewTask)).ToList();
            }
        }

        private void DeleteConstraint(CronoTask constraintTask)
        {
            NewTask.Constraints.Remove(constraintTask);
            Alltask = _allConstraint.Where(w => !NewTask.Constraints.Contains(w) && !w.Equals(NewTask)).ToList();
        }

        private void CloseModal()
        {
            IsOpenModalNewTask = false;

            ServiceBus.RaiseCloseModal(null);
        }

        /// <summary>
        /// Open the modal
        /// </summary>
        public void SetModalTask(TaskDetailsDto task)
        {
            EditMode = !task.IsReadOnly;
            SelectedPeriod = null;
            _backupCopy = JsonConvert.DeserializeObject<CronoTask[]>(JsonConvert.SerializeObject(task.CurrentTask));    //deep copy of the phase. Modification are flushed only after save click
            _taskList = task.CurrentTask.ToArray();
            IsOpenModalNewTask = true;
            EnableNewTaskCreation = !task.IsNewtask;
            _allConstraint = new List<CronoTask>(task.AllTask);
            if (_allConstraint.Count > 0) IsConstraintVisible = true;
            if (_taskList.Length == 1)   //Only 1 phase on the line
            {
                ShowCombo = false;
                PeriodList = new List<Tuple<DateTime, DateTime>>();
                SetTask(_backupCopy[0]);
            }
            else
            {   //More phase on the line, a period must be specified
                ShowCombo = true;   
                ShowDetails = false;
                PeriodList = _taskList.Select(s => new Tuple<DateTime, DateTime>(s.StartDate, s.EndDate)).ToList();
                NewTask = _backupCopy[0];
            }
        }

        /// <summary>
        /// Set current phase to show in the modal
        /// </summary>
        private void SetTask(CronoTask t)
        {
            NewTask = t;
            Alltask = _allConstraint.Where(w => !t.Constraints.Contains(w) && !w.Equals(NewTask)).ToList();
            if (Alltask.Count > 0) IsConstraintVisible = true;
            ShowDetails = true;
        }

        /// <summary>
        /// Save phase details
        /// </summary>
        public void Save()
        {
            if (((NewTask.Duration != null && DurationModality) || (!DurationModality && NewTask.EndDate!=null)) && NewTask.StartDate != null)
            {
                IsOpenModalNewTask = false;
                foreach (var item in _backupCopy)
                {
                    CronoTask original = _taskList.FirstOrDefault(f => f.Equals(item)); 
                    original.Constraints.RemoveAll(r => !item.Constraints.Contains(r));
                    foreach (var constraint in item.Constraints)
                    {
                        if (!original.Constraints.Contains(constraint))
                            original.AddConstraint(constraint);
                    }
                    original.Intervento = item.Intervento;
                    original.StartDate = item.StartDate;
                    if (EndDateModality)
                    {
                        if (item.EndDate < item.StartDate)
                            item.EndDate = item.StartDate;
                        original.EndDate = item.EndDate;
                    }
                    else
                    {
                        if (item.Duration <= 0)
                            item.Duration = 1;
                        original.Duration = item.Duration;
                        original.EndDate = item.CalcEndDate(item.StartDate, item.Duration.Value - 1);

                    }

                }
                ServiceBus.RaiseCloseModal(_taskList);  //Raise close modal event
            }
        }
    }
}
