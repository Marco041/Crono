using Crono.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crono.Repository
{
    public interface IRepository
    {
        Task<List<CronoTask>> GetTaskBetween(DateTime start, DateTime end);
        Task<int> InsertTask(CronoTask task);
        Task<int> UpdateTask(CronoTask task);
        Task<bool> DeleteTask(CronoTask task);
        Task<List<Commessa>> GetCommesse();
        Task<List<CronoTask>> GetAllTask(string codiceCommessa);
        Task InsertConstraints(CronoTask task);
        Task<List<CronoTask>> GetAllCommesseBetween(DateTime d1, DateTime d2);
    }
}
