using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPSC304_Project
{
    public class Task
    {
        private int id;
        private string taskName;
        private string taskDescription;
        private int listId;
        private int projectId;
        private DateTime dueDate;
        private int assignedToUserId;

        public Task( int id, string name, string description, int listId, int projectId, DateTime dueDate, int assignedTo )
        {
            this.id = id;
            taskName = name;
            taskDescription = description;
            this.listId = listId;
            this.projectId = projectId;
            this.dueDate = dueDate;
            assignedToUserId = assignedTo;
        }

        public int ListId
        {
            get => listId;
            set => listId = value;
        }
        public int ProjectId
        {
            get => projectId;
            set => projectId = value;
        }
        public int Id
        {
            get => id;
            set => id = value;
        }
        public int AssignedToUserId
        {
            get => assignedToUserId;
            set => assignedToUserId = value;
        }

        public string GetName()
        {
            return taskName;
        }

        public string GetDescription()
        {
            return taskDescription;
        }

        internal DateTime GetDueDate()
        {
            return dueDate;
        }
    }
}
