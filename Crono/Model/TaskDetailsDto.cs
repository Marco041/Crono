using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crono.Model
{
    public class TaskDetailsDto
    {
        public List<CronoTask> CurrentTask { get; set; }
        public List<CronoTask> AllTask { get; set; }
        public bool IsNewtask { get; set; }
        public bool IsReadOnly { get; set; }

        public TaskDetailsDto(List<CronoTask> currentTask, List<CronoTask> allTask, bool isnewtask, bool isReadOnly)
        {
            CurrentTask = currentTask;
            AllTask = allTask;
            IsNewtask = isnewtask;
            IsReadOnly = isReadOnly;
        }
    }
}
