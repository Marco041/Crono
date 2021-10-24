using Crono.Utility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Crono.Model
{
    [DataContract]
    public class CronoTask : IComparable, INotifyPropertyChanged
    {
        private DateTime _startDate;
        private DateTime _endDate;
        private int? _duration;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "") =>
          PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        public event PropertyChangedEventHandler PropertyChanged;
        [DataMember]
        public ObservableCollection<CronoTask> Constraints { get; set; }
        [DataMember]
        public string CodiceCommessa { get; set; }
        [DataMember]
        public string Intervento { get; set; }
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public int IdFase { get; set; }
        [DataMember]
        public int? Duration {
            get
            {
                return _duration;
            }
            set
            {
                _duration = value;
            }
        }
        [DataMember]
        public DateTime StartDate
        {
            get
            {
                return _startDate;
            }
            set
            {
                _startDate = value;
                NotifyPropertyChanged("StartDate");
            }
        }
        [DataMember]
        public DateTime EndDate
        {
            get
            {
                return _endDate;
            }
            set
            {
                _endDate = value;
                
                NotifyPropertyChanged("EndDate");
            }
        }
        
        public void SetDuration() => Duration = ((int)(_endDate - _startDate).TotalDays+1) - Util.HolidaysNumber(_startDate, _endDate);


        public CronoTask()
        {
            Constraints = new ObservableCollection<CronoTask>();            
        }

        /// <summary>
        /// Metodo per modificare la data di inizio
        /// </summary>
        /// <param name="days"></param>
        public void ChangeStartDate(int days)
        {
            StartDate = StartDate.AddDays(days);
            SetDuration();
            if (Duration != null && !Util.IsHoliday(StartDate))
                Duration -= days;   //se cade su un festivo viene spostato al primo giorno lavorativo utile opposto al verso dello spostamento
        }

        /// <summary>
        /// Modifica della data di fine
        /// </summary>
        /// <param name="days">Verso dello spostamento</param>
        public void ChangeEndDate(int days)
        {
            DateTime originalEndDate = _endDate;
            EndDate = EndDate.AddDays(days);
            SetDuration();
            foreach (var constraint in Constraints)
                if ((Util.IsHoliday(EndDate) && !Util.IsHoliday(originalEndDate) && days<0) || !Util.IsHoliday(EndDate))
                {
                    AdjustConstraint(constraint, originalEndDate, days);    //i vincoli vengono spostati solo se la modifica è stata fatta di un giorno lavorativo
                    constraint.MoveIfHoliday();
                }
        }

        /// <summary>
        /// Riposiziona la dat di inizio al primo giorno lavorativo utile
        /// </summary>
        public void MoveIfHoliday()
        {
            while (Util.IsHoliday(_startDate))
                _startDate = _startDate.AddDays(1);
            StartDate = _startDate;
        }

        /// <summary>
        /// Modifica di entrambe le date
        /// </summary>
        /// <param name="days">Verso dello spostamento</param>
        public void ChangeBothDate(int days)
        {
            DateTime originalEndDate = _endDate;
            StartDate = _startDate.AddDays(days);
            if (Duration==null)
                _endDate = _endDate.AddDays(days);
            else            
                _endDate = CalcEndDate(StartDate, Duration.Value - 1);
            if (_endDate < _startDate) _endDate = _startDate;   //se le date non sono valide di default la durata è un giorno
            EndDate = _endDate;
            if (EndDate != originalEndDate)
                foreach (var constraint in Constraints)
                    AdjustConstraint(constraint, originalEndDate, days);
                
        }

        /// <summary>
        /// Riposizionamento dei vincoli in seguito alla modifica della data finale
        /// </summary>
        /// <param name="constraint">Fase vincolata</param>
        /// <param name="originalEndDate">Data di fine prima della modifica della fase che vincola</param>
        /// <param name="days">Verso dello spostamento</param>
        private void AdjustConstraint(CronoTask constraint, DateTime originalEndDate, int days)
        {
            DateTime endDateCopy = EndDate;
            while (Util.IsHoliday(endDateCopy)) endDateCopy = endDateCopy.AddDays(-1);
            DateTime newStart = endDateCopy;
            while (Util.IsHoliday(originalEndDate)) originalEndDate = originalEndDate.AddDays(-1);  
            constraint.MoveIfHoliday(); //la fine della fase che vincola e del vincolo vengono riposizionate al primo giorno lavorativo utile se sono su un festivo
            int constraintDay = Math.Abs(Math.Abs((int)(constraint.StartDate - originalEndDate).TotalDays) - Util.HolidaysNumber(constraint.StartDate, originalEndDate));   //calcolo dei giorni lavorativi del vincolo che vanno preservati durante lo spostamento
            while (Math.Abs((int)(newStart - endDateCopy).TotalDays) - Util.HolidaysNumber(newStart, endDateCopy) < constraintDay)
                newStart = newStart.AddDays(Math.Sign((constraint.StartDate - endDateCopy).TotalDays) == 0 ? 1 : Math.Sign((constraint.StartDate - endDateCopy).TotalDays));
            while (Util.IsHoliday(constraint.StartDate.AddDays(Math.Sign(days) * Math.Abs((int)(constraint.StartDate - newStart).TotalDays))))
                newStart = newStart.AddDays(-1);    //calcolo della nuova data di partenza del vincolo partendo dalla data di fine della fase che vincola e aggiungendo il numero di giorni lavorativi necessari
            constraint.ChangeBothDate(Math.Sign(days) * Math.Abs((int)(constraint.StartDate - newStart).TotalDays));
        }

        /// <summary>
        /// Modifica della data di fine delle fase
        /// </summary>
        public DateTime CalcEndDate(DateTime start, int duration)
        {
            var endDate = start.AddDays(duration);
            while (Util.IsHoliday(endDate) || (endDate - start).TotalDays - Util.HolidaysNumber(start, endDate) < Duration - 1)
                endDate = endDate.AddDays(1);
            return endDate;
        }

        public void AddConstraint(CronoTask constraint)
        {
            Constraints.Add(constraint);
        }

        public void RemoveConstraint(CronoTask constraint)
        {
            Constraints.Remove(constraint);
        }

        public int CompareTo(object obj)
        {
            if (((CronoTask)obj).StartDate < this.StartDate)
                return 1;
            if (((CronoTask)obj).StartDate > this.StartDate)
                return -1;
            return 0;
        }

        public override bool Equals(object obj)
        {
            if(obj.GetType().Equals(typeof(CronoTask)))
                return ((CronoTask)obj).Id == this.Id ? true : false;
            return false;
        }

        public override int GetHashCode()
        {
            return 2108858624 + Id.GetHashCode();
        }

        public bool IsSamePhase(CronoTask task)
        {
            return task.IdFase == this.IdFase;
        }
    }
}
