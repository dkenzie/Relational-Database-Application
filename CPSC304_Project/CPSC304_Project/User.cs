using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPSC304_Project
{
    public class User
    {
        public string username;
        public string password;
        public int id;
        public bool isManager;

        public User( string name, string password, int id, bool isManager )
        {
            this.username = name;
            this.password = password;
            this.id = id;
            this.isManager = isManager;
        }

        public int getUserId()
        {
            return id;
        }
    }
}
