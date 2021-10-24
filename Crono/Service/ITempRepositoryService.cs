using System.Collections.Generic;
using Crono.Model;

namespace Crono.Service
{
    public interface ITempRepositoryService
    {
        void AddDeletedTask(CronoTask task);
        void AddModifiedTask(CronoTask task);
        void AddNewTask(CronoTask task);
        void ClearAll();
        CronoTask GetAdded(CronoTask task);
        List<CronoTask> GetAllAdded();
        List<CronoTask> GetAllAdded(string codiceCommessa);
        List<CronoTask> GetAllDeleted();
        List<CronoTask> GetAllDeleted(string codiceCommessa);
        List<CronoTask> GetAllModifed();
        List<CronoTask> GetAllModifed(string codiceCommessa);
        CronoTask GetDeleted(CronoTask task);
        CronoTask GetModifed(CronoTask task);
        int GetNewMaxId();
        bool IsAdded(CronoTask task);
        bool IsDeleted(CronoTask task);
        bool IsModified(CronoTask task);
        void UpdadeId(int oldId, int newId);
    }
}