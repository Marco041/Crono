using Crono.Configuration;
using Crono.Configuration.Log;
using Crono.Model;
using Crono.Repository;
using Crono.Service;
using Crono.Utility;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Crono.ViewModel
{
    public class CommesseViewModel : ViewModelBase
    {
        public RelayCommand<object> OpenFasiCommand { get; set; }
        public RelayCommand<object> OpenReportCommand { get; set; }
        public RelayCommand StartLoadingCommand { get; set; }
        private ICronoConfig _config;
        private LogSplitter _log;   //logger
        private Commessa _commessaCorrente; //current commission 
        private IRepository _repository;
        private string _filtroCodice;
        private Timer timer = new Timer();
        private bool _commessaArgs;
        private List<Commessa> _listaCommesseCopy;
        private DateTime _from;
        public DateTime From
        {
            get
            {
                return _from;
            }
            set
            {
                _from = value;
                RaisePropertyChanged("From");
            }
        }
        private DateTime _to;
        public DateTime To
        {
            get
            {
                return _to;
            }
            set
            {
                _to = value;
                RaisePropertyChanged("To");
            }
        }

        private List<Commessa> _listaCommesse;
        public List<Commessa> ListaCommesse
        {
            get
            {
                return _listaCommesse;
            }
            set
            {
                _listaCommesse = value;
                RaisePropertyChanged("ListaCommesse");
            }
        }
        public Commessa CommessaCorrente
        {
            get
            {
                return _commessaCorrente;
            }
            set
            {
                _commessaCorrente = value;
                RaisePropertyChanged("CommessaCorrente");
                ServiceBus.RaiseCommessaChange(new CommessaDto(value, false));
            }
        }
        public string FiltroCodice
        {
            get
            {
                return _filtroCodice;
            }
            set
            {
                _filtroCodice = value;
                if (string.IsNullOrEmpty(value))
                    FiltroCommesse();
                else
                    timer.Enabled = true;
            }
        }

        private string _filtroTecnico;
        public string FiltroTecnico
        {
            get
            {
                return _filtroTecnico;
            }
            set
            {
                _filtroTecnico = value;
                if (string.IsNullOrEmpty(value))
                    FiltroCommesse();
                else
                    timer.Enabled = true;
            }
        }
        private bool _filtroManutenzione;
        public bool FiltroManutenzione
        {
            get
            {
                return _filtroManutenzione;
            }
            set
            {
                _filtroManutenzione = value;
                FiltroCommesse();
            }
        }

        private bool _filtroIntervento;
        public bool FiltroIntervento
        {
            get
            {
                return _filtroIntervento;
            }
            set
            {
                _filtroIntervento = value;
                FiltroCommesse();
            }
        }


        private bool _filtroChiusura;
        public bool FiltroChiusura
        {
            get
            {
                return _filtroChiusura;
            }
            set
            {
                _filtroChiusura = value;
                FiltroCommesse();
            }
        }

        private RelayCommand _loadedCommand;
        public RelayCommand LoadedCommand
        {
            get
            {
                return _loadedCommand
                    ?? (_loadedCommand = new RelayCommand(
                    () =>
                    {
                        StartLoadingCommesse();
                    }));
            }
        }

        private IFrameNavigationService _navigationService;

        public CommesseViewModel(ICronoConfig config, IFrameNavigationService navService)
        {
            _navigationService = navService;
            timer.Elapsed += Timer_Elapsed;
            timer.Interval = 1550;
            timer.Enabled = false;
            _commessaArgs = !string.IsNullOrEmpty(config.CodiceCommessaArgs);
            _log = config.Logger;
            _config = config;
            _repository = config.Repository;
            OpenFasiCommand = new RelayCommand<object>(OpenFasi);
            OpenReportCommand = new RelayCommand<object>(OpenReport);
            StartLoadingCommand = new RelayCommand(()=>StartLoadingCommesse());
            From = DateTime.Now;
            To = From.AddDays(7);
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            timer.Enabled = false;
            FiltroCommesse();
        }

        private void FiltroCommesse()
        {
            ListaCommesse = (from w in _listaCommesseCopy
                where
                w.Manutenzione == _filtroManutenzione && w.Intervento == _filtroIntervento && w.Chiusa == _filtroChiusura &&
                ((!string.IsNullOrEmpty(_filtroCodice) && !string.IsNullOrEmpty(w.Codice)) ? w.Codice.Contains(_filtroCodice.ToUpper()) : true) &&
                    ((!string.IsNullOrEmpty(_filtroTecnico) && !string.IsNullOrEmpty(w.Tecnico)) ? w.Tecnico.ToUpper().Contains(_filtroTecnico.ToUpper()) : true)
                    select w
                    
                ).ToList();
        }

        public void OpenFasi(object c)
        {
            try
            {
                if (c.GetType().Equals(typeof(Commessa)))
                    _navigationService.NavigateTo("Fasi", new CommessaDto(c as Commessa, false));
                ServiceBus.RaiseCommessaChange(new CommessaDto(c as Commessa, false));
            }catch(Exception e)
            {

            }
        }

        public void OpenReport(object c)
        {
            ServiceBus.RaiseCommessaChange(new CommessaDto(null, true, From, To));
        }

        private void CheckCommessaArgs()
        {
            if (_commessaArgs)
                ServiceBus.RaiseCommessaChange(new CommessaDto(new Commessa() { Codice = _config.CodiceCommessaArgs }, false));
            _commessaArgs = false;
        }

        public async void StartLoadingCommesse()
        {
            try
            {
                if (_commessaArgs) CheckCommessaArgs();
                else
                {
                    if (string.IsNullOrEmpty(FiltroCodice))
                    {
                        ListaCommesse = await _repository.GetCommesse();
                        _listaCommesseCopy = new List<Commessa>(ListaCommesse.ToList());
                        FiltroCommesse();
                    }
                }
            }
            catch (Exception e)
            {
                //ShowMessage("Errore durante il recupero delle commesse");
                _log.Error("Errore durante il recupero delle commesse", e);
            }
        }
    }
}
