using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPSC304_Project
{
    public class Project
    {
        private string projectName;
        private List<ProjectList> projectLists = new List<ProjectList> ();
        private List<User> users = new List<User> ();
        private int id;

        public Project(string name, List<ProjectList> lists, List<User> users, int id)
        {
            projectName = name;
            projectLists = lists;
            this.users = users;
            this.id = id;
        }

        public string getName()
        {
            return projectName;
        }

        public List<ProjectList> getProjectLists()
        {
            return projectLists;
        }

        public List<User> getUserIds()
        {
            return users;
        }

        internal void AddProjectList( ProjectList newProjectList )
        {
            projectLists.Add ( newProjectList );
        }

        internal int getProjectId()
        {
            return id;
        }
    }
}
