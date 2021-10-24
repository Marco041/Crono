using Crono.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crono.Repository
{
    /// <summary>
    /// In memory repository
    /// </summary>
    public class FakeRepository : IRepository
    {
        //Test configuration
        private List<CronoTask> Tasks = new List<CronoTask>()
        {
            new CronoTask(){Id=1, IdFase=1, Intervento="T1", CodiceCommessa="2015C028", StartDate= new DateTime(2018,01,30), EndDate=new DateTime(2018,12,18)},
            new CronoTask(){Id=2, IdFase=2, Intervento="T2", CodiceCommessa="2015C027", StartDate= new DateTime(2018,02,25), EndDate=new DateTime(2018,02,27) },
            new CronoTask(){Id=3, IdFase=3, Intervento="T3", CodiceCommessa="2015C027", StartDate= new DateTime(2018,04,20), EndDate=new DateTime(2018,04,27) },
            new CronoTask(){Id=4, IdFase=4, Intervento="T4", CodiceCommessa="2015C024", StartDate= new DateTime(2018,04,01), EndDate=new DateTime(2018,04,02) },
            new CronoTask(){Id=5, IdFase=5, Intervento="T5", CodiceCommessa="2015C024", StartDate= new DateTime(2018,03,05), EndDate=new DateTime(2018,03,27) },
            new CronoTask(){Id=6, IdFase=6, Intervento="T6", CodiceCommessa="2015C028", StartDate= new DateTime(2018,05,25), EndDate=new DateTime(2018,06,07) },
            new CronoTask(){Id=7, IdFase=7, Intervento="T7", CodiceCommessa="2015C028", StartDate= new DateTime(2018,05,15), EndDate=new DateTime(2018,05,17) },
            new CronoTask(){Id=8, IdFase=8, Intervento="T8", CodiceCommessa="2015C028", StartDate= new DateTime(2018,05,05), EndDate=new DateTime(2018,06,17) },
            new CronoTask(){Id=9, IdFase=9, Intervento="T9", CodiceCommessa="2015C026", StartDate= new DateTime(2018,05,12), EndDate=new DateTime(2018,05,27) },
            new CronoTask(){Id=10, IdFase=10, Intervento="T10", CodiceCommessa="2015C023", StartDate= new DateTime(2018,03,25), EndDate=new DateTime(2018,05,07) },
            new CronoTask(){Id=11, IdFase=11, Intervento="T11", CodiceCommessa="2015C028", StartDate= new DateTime(2018,05,4), EndDate=new DateTime(2018,05,08) },
            new CronoTask(){Id=12, IdFase=12, Intervento="T12", CodiceCommessa="2015C023", StartDate= new DateTime(2018,05,10), EndDate=new DateTime(2018,05,17) },
            new CronoTask(){Id=13, IdFase=13, Intervento="T13", CodiceCommessa="2015C028", StartDate= new DateTime(2018,06,07), EndDate=new DateTime(2018,06,16) },
            new CronoTask(){Id=14, IdFase=14, Intervento="T14", CodiceCommessa="2015C023", StartDate= new DateTime(2018,05,11), EndDate=new DateTime(2018,05,24) },
            new CronoTask(){Id=15, IdFase=15, Intervento="T15", CodiceCommessa="2015C023", StartDate= new DateTime(2018,05,05), EndDate=new DateTime(2018,05,07) },
            new CronoTask(){Id=16, IdFase=16, Intervento="T16", CodiceCommessa="2015C023", StartDate= new DateTime(2018,05,23), EndDate=new DateTime(2018,05,31) },
            new CronoTask(){Id=17, IdFase=17, Intervento="T17", CodiceCommessa="2015C023", StartDate= new DateTime(2018,05,17), EndDate=new DateTime(2018,05,18) },
            new CronoTask(){Id=18, IdFase=18, Intervento="T18", CodiceCommessa="2015C023", StartDate= new DateTime(2018,05,09), EndDate=new DateTime(2018,05,15) },
            new CronoTask(){Id=19, IdFase=19, Intervento="T19", CodiceCommessa="2015C024", StartDate= new DateTime(2018,05,01), EndDate=new DateTime(2018,05,05) },
            new CronoTask(){Id=20, IdFase=20, Intervento="T20", CodiceCommessa="2015C028", StartDate= new DateTime(2018,05,03), EndDate=new DateTime(2018,05,08) },
            new CronoTask(){Id=21, IdFase=21, Intervento="T21", CodiceCommessa="2015C024", StartDate= new DateTime(2018,05,04), EndDate=new DateTime(2018,05,09) },
            new CronoTask(){Id=22, IdFase=22, Intervento="T22", CodiceCommessa="2015C025", StartDate= new DateTime(2018,04,01), EndDate=new DateTime(2018,04,10) },
            new CronoTask(){Id=23, IdFase=23, Intervento="T23", CodiceCommessa="2015C025", StartDate= new DateTime(2018,04,01), EndDate=new DateTime(2018,04,15) },
            new CronoTask(){Id=24, IdFase=24, Intervento="T24", CodiceCommessa="2015C024", StartDate= new DateTime(2018,01,02), EndDate=new DateTime(2018,01,11) },
            new CronoTask(){Id=25, IdFase=25, Intervento="T25", CodiceCommessa="2015C023", StartDate= new DateTime(2018,06,08), EndDate=new DateTime(2018,06,21) },
            new CronoTask(){Id=26, IdFase=26, Intervento="T26", CodiceCommessa="2015C025", StartDate= new DateTime(2018,05,10), EndDate=new DateTime(2018,05,20) },
            new CronoTask(){Id=27, IdFase=27, Intervento="T27", CodiceCommessa="2015C025", StartDate= new DateTime(2018,05,13), EndDate=new DateTime(2018,05,21) },
            new CronoTask(){Id=28, IdFase=28, Intervento="T28", CodiceCommessa="2015C025", StartDate= new DateTime(2018,05,02), EndDate=new DateTime(2018,05,04) },
            new CronoTask(){Id=29, IdFase=29, Intervento="T29", CodiceCommessa="2015C025", StartDate= new DateTime(2018,05,03), EndDate=new DateTime(2018,05,04) },
            new CronoTask(){Id=30, IdFase=30, Intervento="T30", CodiceCommessa="2015C024", StartDate= new DateTime(2018,05,8), EndDate=new DateTime(2018,05,14) },
            new CronoTask(){Id=31, IdFase=7, Intervento="T7", CodiceCommessa="2015C028", StartDate= new DateTime(2018,05,20), EndDate=new DateTime(2018,05,24) },
        };

        private List<Commessa> ListaCommesse = new List<Commessa>(){
            new Commessa(){ Codice="2015C023", DataRegistrazione=new DateTime(2018,01,01)},
            new Commessa(){ Codice="2015C024", DataRegistrazione=new DateTime(2018,02,08)},
            new Commessa(){ Codice="2015C025", DataRegistrazione=new DateTime(2018,01,04)},
            new Commessa(){ Codice="2015C026", DataRegistrazione=new DateTime(2018,01,11)},
            new Commessa(){ Codice="2015C027", DataRegistrazione=new DateTime(2018,01,14)},
            new Commessa(){ Codice="2015C028", DataRegistrazione=new DateTime(2018,01,21)}
        };

        public FakeRepository()
        {
            Tasks.First(f=>f.Id==7).Constraints.Add(Tasks.First(f => f.Id == 8));
            Tasks.First(f => f.Id == 7).Constraints.Add(Tasks.First(f => f.Id == 13));
            Tasks.First(f => f.Id == 8).Constraints.Add(Tasks.First(f => f.Id == 11));
            Tasks.First(f => f.Id == 13).Constraints.Add(Tasks.First(f => f.Id == 20));
            Tasks.First(f => f.Id == 18).Constraints.Add(Tasks.First(f => f.Id == 25));
            Tasks.First(f => f.Id == 19).Constraints.Add(Tasks.First(f => f.Id == 24));
            Tasks.First(f => f.Id == 24).Constraints.Add(Tasks.First(f => f.Id == 30));
            Tasks.First(f => f.Id == 4).Constraints.Add(Tasks.First(f => f.Id == 5));
            Tasks.First(f => f.Id == 22).Constraints.Add(Tasks.First(f => f.Id == 23));
            Tasks.First(f => f.Id == 29).Constraints.Add(Tasks.First(f => f.Id == 23));
        }

        public async Task<List<CronoTask>> GetTaskBetween(DateTime start, DateTime end)
        {
            var res = Tasks.Where(w => (w.StartDate >= start && w.StartDate <= end) || (w.EndDate>=start && w.EndDate<=end) || (w.EndDate>end && w.StartDate<start) ).ToList();

            for(int j= res.Count - 1; j >= 0; j--)
            {
                res.AddRange(AddMaster(res[j]));
                res.AddRange(AddConstraint(res[j]));
            }                                  
            return await Task.FromResult(res);
        }

        public async Task<int> InsertTask(CronoTask task)
        {
            try
            {

                Tasks.Add(task);
                return await Task.FromResult(Tasks.Max(m => m.Id) + 1);
            }
            catch (Exception)
            {
                return -1;
            }
        }

        public async Task<bool> DeleteTask(CronoTask task)
        {
            try
            {

                return await Task.FromResult(Tasks.Remove(task));
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<int> UpdateTask(CronoTask task)
        {
            try
            {

                var t = Tasks.First(f => f.Equals(task));
                t.EndDate = task.EndDate;
                t.StartDate = task.StartDate;
                t.Intervento = task.Intervento;
                return await Task.FromResult(1);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public async Task<List<Commessa>> GetCommesse()
        {
            return await Task.FromResult(ListaCommesse);
        }

        public async Task<List<CronoTask>> GetAllTask(string codiceCommessa)
        {
            return await Task.FromResult(Tasks.Where(w=>w.CodiceCommessa.Equals(codiceCommessa)).ToList());
        }

        private List<CronoTask> AddConstraint(CronoTask task)
        {
            List<CronoTask> result = new List<CronoTask>();
            foreach (var item in task.Constraints)
            {
                result.AddRange(Tasks.Where(w => w.Equals(item)));
                result.AddRange(AddConstraint(item));
            }

            return result;
            
        }

        private List<CronoTask> AddMaster(CronoTask task)
        {
            List<CronoTask> result = new List<CronoTask>();
            var masters = Tasks.Where(w => w.Constraints.Contains(task));
            result.AddRange(masters);
            foreach (var item in masters)
            {
                result.AddRange(AddMaster(item));
            }

            return result;
        }

        public Task InsertConstraints(CronoTask task)
        {
            return Task.FromResult(true);
        }

        public Task<List<CronoTask>> GetAllCommesseBetween(DateTime d1, DateTime d2)
        {
            return Task.FromResult(Tasks.Where(w => (w.StartDate >= d1 && w.StartDate <= d1) || (w.EndDate >= d1 && w.EndDate <= d2)).ToList());
        }
    }
}
