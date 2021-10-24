using Crono.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crono.Service
{
    /// <summary>
    /// Temp Repository. Is Flushed on save
    /// </summary>
    public class TempRepositoryService : ITempRepositoryService
    {
        private HashSet<CronoTask> _deletedTask;    //list of deleted phase (not used at the moment)
        private HashSet<CronoTask> _modifiedTask;   //edited phase
        private HashSet<CronoTask> _addedTask;  //added phase

        public TempRepositoryService()
        {
            _deletedTask = new HashSet<CronoTask>();
            _modifiedTask = new HashSet<CronoTask>();
            _addedTask = new HashSet<CronoTask>();
        }        

        public void AddNewTask(CronoTask task)
        {
            _addedTask.Add(task);
        }

        public void AddModifiedTask(CronoTask task)
        {
            _modifiedTask.Add(task);
        }

        public void AddDeletedTask(CronoTask task)
        {
            _deletedTask.Add(task);
        }

        public bool IsDeleted(CronoTask task) => _deletedTask.Contains(task);
        public bool IsModified(CronoTask task) => _modifiedTask.Contains(task);
        public bool IsAdded(CronoTask task) => _addedTask.Contains(task);

        public List<CronoTask> GetAllAdded(string codiceCommessa) => _addedTask.Where(w => w.CodiceCommessa.Equals(codiceCommessa)).ToList();
        public List<CronoTask> GetAllDeleted(string codiceCommessa) => _deletedTask.Where(w => w.CodiceCommessa.Equals(codiceCommessa)).ToList();
        public List<CronoTask> GetAllModifed(string codiceCommessa) => _modifiedTask.Where(w => w.CodiceCommessa.Equals(codiceCommessa)).ToList();

        public List<CronoTask> GetAllAdded() => _addedTask.ToList();
        public List<CronoTask> GetAllDeleted() => _deletedTask.ToList();
        public List<CronoTask> GetAllModifed() => _modifiedTask.ToList();

        public CronoTask GetAdded(CronoTask task) => _addedTask.FirstOrDefault(w => w.Equals(task));
        public CronoTask GetDeleted(CronoTask task) => _deletedTask.FirstOrDefault(w => w.Equals(task));
        public CronoTask GetModifed(CronoTask task) => _modifiedTask.FirstOrDefault(w => w.Equals(task));

        /// <summary>
        /// Aggiorna l'id di una fase aggiunta in modo che se ci sono modifche da salvare viene usato il nuovo id
        /// </summary>
        /// <param name="oldId"></param>
        /// <param name="newId"></param>
        public void UpdadeId(int oldId, int newId)
        {
            var t1 = _addedTask.FirstOrDefault(f => f.Id == oldId);
                if (t1 != null) t1.Id = newId;
            var t2 = _modifiedTask.FirstOrDefault(f => f.Id == oldId);
                if (t2 != null) t1.Id = newId;
            var t3 = _deletedTask.FirstOrDefault(f => f.Id == oldId);
                if (t3 != null) t1.Id = newId;
        }

        public int GetNewMaxId() => _addedTask.Count>0 ? _addedTask.Min(m => m.Id) - 1 : -1;

        public void ClearAll()
        {
            _addedTask.Clear();
            _modifiedTask.Clear();
            _deletedTask.Clear();
        }
    }
}
