using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPSC304_Project
{
    public class ProjectList
    {
        private int id;
        private int projectId;
        private string projectListName;
        private string priorityString;
        private List<Task> tasks;

        public ProjectList( int id, int projectId, string name, string priority, List<Task> projectListTasks )
        {
            this.id = id;
            this.projectId = projectId;
            projectListName = name;
            priorityString = priority;
            tasks = projectListTasks;
        }

        public int Id
        {
            get => id;
            set => id = value;
        }
        public int ProjectId
        {
            get => projectId;
            set => projectId = value;
        }

        internal string getName()
        {
            return projectListName;
        }

        internal string getListPriority()
        {
            return priorityString;
        }

        internal List<Task> getTasks()
        {
            return tasks;
        }
    }
}
