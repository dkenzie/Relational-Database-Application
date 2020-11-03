using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace CPSC304_Project
{
    class DatabaseHandler
    {
        private static MySqlConnection mySqlConnection;
        private static string connectionString;
        private static DatabaseHandler dbHandlerInstance = null;
        private static string databaseName = null;
        private static bool initialized = false;

        public static DatabaseHandler getInstance()
        {
            if ( databaseName == null )
            {
                throw new Exception ( "Database name is null. Use setDatabase(string dbName) before calling getInstance()." );
            }

            if ( dbHandlerInstance == null )
            {
                try
                {
                    dbHandlerInstance = new DatabaseHandler ();
                    connectionString = String.Format ( "server=localhost;userid=root;password=lolipop26;database={0};", databaseName );
                    mySqlConnection = new MySqlConnection ( connectionString );

                    mySqlConnection.Open ();
                    MySqlCommand cmd = mySqlConnection.CreateCommand ();
                    cmd.CommandText = "SELECT COUNT(*) FROM 'Users'";
                    cmd.ExecuteNonQuery ();
                    mySqlConnection.Close ();
                    initialized = true;

                }
                catch (Exception e )
                {
                    // Create new database
                    connectionString = "server=localhost;userid=root;password=lolipop26;";
                    mySqlConnection = new MySqlConnection ( connectionString );

                    mySqlConnection.Open ();
                    MySqlCommand cmd = mySqlConnection.CreateCommand ();
                    cmd.CommandText = String.Format ( "CREATE DATABASE IF NOT EXISTS `{0}`;", MainWindow.DATABASE_NAME );
                    cmd.ExecuteNonQuery ();
                    mySqlConnection.Close ();

                    connectionString = String.Format ( "server=localhost;userid=root;password=lolipop26;database={0};", databaseName );
                    mySqlConnection = new MySqlConnection ( connectionString );

                    initialized = false;
                }
            }
            return dbHandlerInstance;
        }

        internal List<Task> getAllTasksForUser( User user )
        {
            // Return a list of all tasks for given user
            mySqlConnection.Open ();
            List<Task> usersTasks = new List<Task> ();
            MySqlCommand cmd = mySqlConnection.CreateCommand ();
            cmd.CommandText =
                String.Format (
                "SELECT id, listId, projectId, taskName, taskDescription, taskDueDate, assignedTo " +
                "FROM Tasks " +
                "WHERE assignedTo = {0} ",
                user.id
                );
            MySqlDataReader reader = cmd.ExecuteReader ();
            while ( reader.Read () )
            {
                int taskId = reader.GetInt32 ( 0 );
                int listId = reader.GetInt32 ( 1 );
                int projectId = reader.GetInt32 ( 2 );
                string taskName = reader.GetString ( 3 );
                string taskDescription = reader.GetString ( 4 );
                DateTime taskDueDate = reader.GetDateTime ( 5 );
                int userId = reader.GetInt32 ( 6 );
                Task task = new Task ( taskId, taskName, taskDescription, listId, projectId, taskDueDate, userId );
                usersTasks.Add ( task );
            }
            mySqlConnection.Close ();
            return usersTasks;
        }

        public void addUserToProject( User user, Project project )
        {
            // Makes the connection between user and project by adding to the WorksOn database table
            mySqlConnection.Open ();
            MySqlCommand cmd = mySqlConnection.CreateCommand ();
            cmd.CommandText =
                String.Format (
                    "INSERT INTO WorksOn(userId,projectId)" +
                    "VALUES ({0},{1});",
                    user.getUserId (), project.getProjectId () );
            cmd.ExecuteNonQuery ();

            mySqlConnection.Close ();
        }

        internal void removeTask( Task activeTask )
        {
            mySqlConnection.Open ();
            MySqlCommand cmd = mySqlConnection.CreateCommand ();
            cmd.CommandText =
                String.Format (
                    "DELETE FROM Tasks " +
                    "WHERE id = {0};",
                    activeTask.Id );
            cmd.ExecuteNonQuery ();

            mySqlConnection.Close ();
        }

        internal void removeList( ProjectList list )
        {
            mySqlConnection.Open ();
            MySqlCommand cmd = mySqlConnection.CreateCommand ();
            cmd.CommandText =
                String.Format (
                    "DELETE FROM Lists " +
                    "WHERE id = {0};",
                    list.Id );
            cmd.ExecuteNonQuery ();

            mySqlConnection.Close ();
        }

        public void addNewProject( Project newProject )
        {
            // Inserts the new project into the database
            mySqlConnection.Open ();
            MySqlCommand cmd = mySqlConnection.CreateCommand ();
            cmd.CommandText =
                String.Format (
                    "INSERT INTO Projects(id,title) " +
                    "VALUES ({0},'{1}');",
                    newProject.getProjectId (), newProject.getName() );
            cmd.ExecuteNonQuery ();

            mySqlConnection.Close ();
            
        }

        public void assignUserToTask( User selectedUser, Task selectedTask )
        {
            // Changes the assigned user of selectedTask to be selectedUser
            mySqlConnection.Open ();
            MySqlCommand cmd = mySqlConnection.CreateCommand ();
            cmd.CommandText =
                String.Format (
                    "UPDATE Tasks " +
                    "SET assignedTo = {0} " +
                    "WHERE id = {1}; ",
                    selectedUser.id, selectedTask.Id );
            cmd.ExecuteNonQuery ();

            mySqlConnection.Close ();
        }

        public static int generateNextProjectId()
        {
            int newProjectId = 0;
            mySqlConnection.Open ();
            MySqlCommand cmd = mySqlConnection.CreateCommand ();
            cmd.CommandText =
                "SELECT id " +
                "FROM Projects ";
            MySqlDataReader reader = cmd.ExecuteReader ();
            while ( reader.Read () )
            {
                int currentProjectId = reader.GetInt32 ( 0 );
                if ( newProjectId <= currentProjectId )
                {
                    newProjectId = currentProjectId + 1;
                }
            }
            mySqlConnection.Close ();
            return newProjectId;
        }

        public static int generateNextProjectListId()
        {
            int newProjectListId = 0;
            mySqlConnection.Open ();
            MySqlCommand cmd = mySqlConnection.CreateCommand ();
            cmd.CommandText =
                "SELECT id " +
                "FROM Lists ";
            MySqlDataReader reader = cmd.ExecuteReader ();
            while ( reader.Read () )
            {
                int currentProjectListId = reader.GetInt32 ( 0 );
                if ( newProjectListId <= currentProjectListId )
                {
                    newProjectListId = currentProjectListId + 1;
                }
            }
            mySqlConnection.Close ();
            return newProjectListId;
        }

        public static int generateNextTaskId()
        {
            int newTaskId = 0;
            mySqlConnection.Open ();
            MySqlCommand cmd = mySqlConnection.CreateCommand ();
            cmd.CommandText =
                "SELECT id " +
                "FROM Tasks ";
            MySqlDataReader reader = cmd.ExecuteReader ();
            while ( reader.Read () )
            {
                int currentTaskId = reader.GetInt32 ( 0 );
                if ( newTaskId <= currentTaskId )
                {
                    newTaskId = currentTaskId + 1;
                }
            }
            mySqlConnection.Close ();
            return newTaskId;
        }

        public List<Project> getUsersProjects( int userId )
        {
            // Return a list of all projects the current user works on
            List<Project> usersProjects = new List<Project> ();

            mySqlConnection.Open ();
            MySqlCommand cmd = mySqlConnection.CreateCommand ();
            cmd.CommandText =
                String.Format (
                "SELECT userId, projectId " +
                "FROM WorksOn " +
                "WHERE userId = {0} ",
                userId );
            MySqlDataReader reader = cmd.ExecuteReader ();
            List<int> projectIds = new List<int> ();
            while ( reader.Read () )
            {
                // Adds all projectIds for the given userId
                projectIds.Add ( reader.GetInt32 ( 1 ) );
            }
            mySqlConnection.Close ();
            foreach ( int projectId in projectIds )
            {
                usersProjects.Add ( getProjectFromId ( projectId ) );
            }
            return usersProjects;

        }

        public Project getProjectFromId( int projectId )
        {
            // Creates and returns a Project object for the given projectId
            string title = null;

            mySqlConnection.Open ();
            MySqlCommand cmd = mySqlConnection.CreateCommand ();
            cmd.CommandText =
                String.Format (
                "SELECT title " +
                "FROM Projects " +
                "WHERE id = {0} ",
                projectId );
            using ( MySqlDataReader reader = cmd.ExecuteReader () )
            {
                while ( reader.Read () )
                {
                    // Get the project title
                    title = reader.GetString ( 0 );
                }
            }
            mySqlConnection.Close ();

            List<ProjectList> projectLists = getListsOnProject ( projectId );
            List<User> users = getUsersOnProject ( projectId );

            Project project = new Project ( title, projectLists, users, projectId );
            return project;
        }

        public List<ProjectList> getListsOnProject( int projectId )
        {
            List<ProjectList> projectLists = new List<ProjectList> ();
            List<int> projectListIds = new List<int> ();
            mySqlConnection.Open ();
            MySqlCommand cmd = mySqlConnection.CreateCommand ();
            cmd.CommandText =
                String.Format (
                "SELECT id " +
                "FROM Lists " +
                "WHERE projectId = {0} ",
                projectId );
            using ( MySqlDataReader reader = cmd.ExecuteReader () )
            {
                while ( reader.Read () )
                {
                    // add all project lists
                    int listId = reader.GetInt32 ( 0 );
                    projectListIds.Add ( listId );
                }
            }
            mySqlConnection.Close ();
            foreach ( int projectListId in projectListIds )
            {
                ProjectList projectList = getListFromId ( projectListId );
                projectLists.Add ( projectList );
            }
            return projectLists;
        }

        public List<Task> getTasksOnList( ProjectList projectList )
        {
            List<Task> tasks = new List<Task> ();

            mySqlConnection.Open ();
            MySqlCommand cmd = mySqlConnection.CreateCommand ();
            cmd.CommandText =
                String.Format (
                "SELECT id, listId, projectId, taskName, taskDescription, taskDueDate, assignedTo " +
                "FROM Tasks " +
                "WHERE listId = {0} ",
                projectList.Id );
            MySqlDataReader reader = cmd.ExecuteReader ();
            List<int> userIds = new List<int> ();
            while ( reader.Read () )
            {
                int taskId = reader.GetInt32 ( 0 );
                int listId = reader.GetInt32 ( 1 );
                int projectId = reader.GetInt32 ( 2 );
                string taskName = reader.GetString ( 3 );
                string taskDescription = reader.GetString ( 4 );
                DateTime taskDueDate = reader.GetDateTime ( 5 );
                int assignedToUserId = reader.GetInt32 ( 6 );
                Task task = new Task ( taskId, taskName, taskDescription, listId, projectId, taskDueDate, assignedToUserId );
                tasks.Add ( task );
            }
            mySqlConnection.Close ();
            return tasks;
            
        }

        public List<User> getUsersOnProject( int projectId )
        {
            // Return a list of all users working on Project with given projectId 
            List<User> users = new List<User> ();

            mySqlConnection.Open ();
            MySqlCommand cmd = mySqlConnection.CreateCommand ();
            cmd.CommandText =
                String.Format (
                "SELECT userId, projectId " +
                "FROM WorksOn " +
                "WHERE projectId = {0} ",
                projectId );
            MySqlDataReader reader = cmd.ExecuteReader ();
            List<int> userIds = new List<int> ();
            while ( reader.Read () )
            {
                userIds.Add ( reader.GetInt32 ( 0 ) );
            }
            mySqlConnection.Close ();
            foreach ( int userId in userIds )
            {
                User user = getUserFromId ( userId );
                users.Add ( user );
            }
            return users;
        }

        public Task getTaskFromId( int id )
        {
            Task task = null;
            try
            {
                mySqlConnection.Open ();
            }
            catch ( Exception e )
            {
                // was already open
            }
            MySqlCommand cmd = mySqlConnection.CreateCommand ();
            cmd.CommandText =
                String.Format (
                "SELECT id, projectId, taskName, taskDescription, taskDueDate, assignedTo, listId " +
                "FROM Tasks " +
                "WHERE id = {0} ",
                id );
            MySqlDataReader reader = cmd.ExecuteReader ();
            List<int> userIds = new List<int> ();
            while ( reader.Read () )
            {
                int taskId = reader.GetInt32 ( 0 );
                int projectId = reader.GetInt32 ( 1 );
                string taskName = reader.GetString ( 2 );
                string description = reader.GetString ( 3 );
                DateTime taskDueDate = reader.GetDateTime ( 4 );
                int assignedToUserId = reader.GetInt32 ( 5 );
                int listId = reader.GetInt32 ( 6 );
                task = new Task ( taskId, taskName, description, listId, projectId, taskDueDate, assignedToUserId );
            }
            mySqlConnection.Close ();
            return task;
        }

        public User getUserFromId( int userId )
        {
            User user = null;
            mySqlConnection.Open ();
            MySqlCommand cmd = mySqlConnection.CreateCommand ();
            cmd.CommandText =
                String.Format (
                "SELECT id, isManager, username, password " +
                "FROM Users " +
                "WHERE id = {0} ",
                userId );
            MySqlDataReader reader = cmd.ExecuteReader ();
            List<int> userIds = new List<int> ();
            while ( reader.Read () )
            {
                int id          = reader.GetInt32 ( 0 );
                int isManagerInt = reader.GetInt32 ( 1 );
                string username = reader.GetString ( 2 );
                string password = reader.GetString ( 3 );
                user = new User ( username, password, id, ( isManagerInt == 1 ) );
            }
            mySqlConnection.Close ();
            return user;
        }

        public ProjectList getListFromId( int listId )
        {
            ProjectList projectList = null;
            List<Task> tasks = getTasksForListId ( listId );

            try
            {
                mySqlConnection.Open ();
            }
            catch ( Exception e )
            {
                // was already open
            }
            MySqlCommand cmd = mySqlConnection.CreateCommand ();
            cmd.CommandText =
                String.Format (
                "SELECT id, title, priority, projectId " +
                "FROM Lists " +
                "WHERE id = {0} ",
                listId );
            using ( MySqlDataReader reader = cmd.ExecuteReader () )
            {
                List<int> userIds = new List<int> ();
                while ( reader.Read () )
                {
                    int id = reader.GetInt32 ( 0 );
                    string listName = reader.GetString ( 1 );
                    string priority = reader.GetString ( 2 );
                    int projectId = reader.GetInt32 ( 3 );
                    projectList = new ProjectList ( id, projectId, listName, priority, tasks );
                }
            }
            mySqlConnection.Close ();
            return projectList;
        }

        public List<Task> getTasksForListId( int listId )
        {
            List<Task> tasks = new List<Task> ();

            try
            {
                mySqlConnection.Open ();
            }
            catch ( Exception e )
            {
                // was already open
            }
            MySqlCommand cmd = mySqlConnection.CreateCommand ();
            cmd.CommandText =
                String.Format (
                "SELECT id, projectId, taskName, taskDescription, taskDueDate, assignedTo " +
                "FROM Tasks " +
                "WHERE listId = {0} ",
                listId );
            MySqlDataReader reader = cmd.ExecuteReader ();
            List<int> userIds = new List<int> ();
            while ( reader.Read () )
            {
                int taskId = reader.GetInt32 ( 0 );
                int projectId = reader.GetInt32 ( 1 );
                string taskName = reader.GetString ( 2 );
                string description = reader.GetString ( 3 );
                DateTime taskDueDate = reader.GetDateTime ( 4 );
                int assignedToUserId = reader.GetInt32 ( 5 );
                Task task = new Task ( taskId, taskName, description, listId, projectId, taskDueDate, assignedToUserId );
                tasks.Add ( task );
            }
            mySqlConnection.Close ();
            return tasks;
        }

        public string getUsername( int userId )
        {
            // Return the username corresponding to the given userId
            mySqlConnection.Open ();
            MySqlCommand cmd = mySqlConnection.CreateCommand ();
            cmd.CommandText =
                String.Format (
                "SELECT username " +
                "FROM Users " +
                "WHERE id = '{0}' ",
                userId.ToString () );
            MySqlDataReader reader = cmd.ExecuteReader ();
            string username = null;
            while ( reader.Read() )
            {
                username = reader.GetString ( 0 );
            }
            if ( username == null )
            {
                throw new Exception ( "ERROR: UserId not found in the database." );
            }
            mySqlConnection.Close ();
            return username;
        }

        public static void initDatabase()
        {
            // If database has not already been initialized, create a new database with the appropriate tables
            if ( !initialized )
            {
                mySqlConnection.Open ();

                MySqlCommand createUsersTableCmd = mySqlConnection.CreateCommand ();
                createUsersTableCmd.CommandText =
                    "CREATE TABLE IF NOT EXISTS Users (" +
                    "id INT PRIMARY KEY, " +
                    "isManager INT," +
                    "username CHAR(200), " +
                    "password CHAR(200) " +
                    ");";
                createUsersTableCmd.ExecuteNonQuery ();

                MySqlCommand createProjectsTableCmd = mySqlConnection.CreateCommand ();
                createProjectsTableCmd.CommandText =
                    "CREATE TABLE IF NOT EXISTS Projects (" +
                    "id INT PRIMARY KEY, " +
                    "title CHAR(200) " +
                    ");";
                createProjectsTableCmd.ExecuteNonQuery ();

                MySqlCommand createListsTableCmd = mySqlConnection.CreateCommand ();
                createListsTableCmd.CommandText =
                    "CREATE TABLE IF NOT EXISTS Lists (" +
                    "id INT, " +
                    "projectId INT, " +
                    "title CHAR(200), " +
                    "priority CHAR(200), " +
                    "PRIMARY KEY (id, projectId), " +
                    "FOREIGN KEY (projectId) REFERENCES Projects(id) " +
                    ");";
                createListsTableCmd.ExecuteNonQuery ();

                MySqlCommand createTasksTableCmd = mySqlConnection.CreateCommand ();
                createTasksTableCmd.CommandText =
                    "CREATE TABLE IF NOT EXISTS Tasks (" +
                    "id INT, " +
                    "projectId INT, " +
                    "listId INT, " +
                    "taskName CHAR(200), " +
                    "taskDescription CHAR(200)," +
                    "taskDueDate DATETIME, " +
                    "assignedTo INT, " +
                    "PRIMARY KEY (id, projectId, listId), " +
                    "FOREIGN KEY (projectId) REFERENCES Projects(id), " +
                    "FOREIGN KEY (listId) REFERENCES Lists(id) ON DELETE CASCADE, " +
                    "FOREIGN KEY (assignedTo) REFERENCES Users(id) " +
                    ");";
                createTasksTableCmd.ExecuteNonQuery ();

                MySqlCommand createWorksOnTableCmd = mySqlConnection.CreateCommand ();
                createWorksOnTableCmd.CommandText =
                    "CREATE TABLE IF NOT EXISTS WorksOn (" +
                    "projectId INT, " +
                    "userId INT, " +
                    "PRIMARY KEY (projectId, userId), " +
                    "FOREIGN KEY (projectId) REFERENCES Projects(id), " +
                    "FOREIGN KEY (userId) REFERENCES Users(id) " +
                    ");";
                createWorksOnTableCmd.ExecuteNonQuery ();
                mySqlConnection.Close ();

            }
            initialized = true;

        }

        public static void setDatabase( string dbName )
        {
            databaseName = dbName;
        }

        public List<User> getAllUsers()
        {
            // Return a list of all users
            mySqlConnection.Open ();
            List<User> allUsers = new List<User> ();
            MySqlCommand cmd = mySqlConnection.CreateCommand ();
            cmd.CommandText =
                "SELECT username, password, id, isManager " +
                "FROM Users ";
            MySqlDataReader reader = cmd.ExecuteReader ();
            while ( reader.Read () )
            {
                string username  = reader.GetString ( 0 );
                string password  = reader.GetString ( 1 );
                int    id        = reader.GetInt32  ( 2 );
                int    isManager = reader.GetInt32  ( 3 );
                User user = new User ( username, password, id, (isManager == 1) );
                allUsers.Add ( user );
            }
            mySqlConnection.Close ();
            return allUsers;
        }

        public List<Project> getAllProjects()
        {
            // Return a list of all projects
            mySqlConnection.Open ();
            List<Project> allProjects = new List<Project> ();
            List<int> allProjectIds = new List<int> ();
            MySqlCommand cmd = mySqlConnection.CreateCommand ();
            cmd.CommandText =
                "SELECT id " +
                "FROM Projects ";
            MySqlDataReader reader = cmd.ExecuteReader ();
            while ( reader.Read () )
            {
                int projectId = reader.GetInt32 ( 0 );
                allProjectIds.Add ( projectId );
            }
            mySqlConnection.Close ();
            foreach ( int id in allProjectIds )
            {
                allProjects.Add ( getProjectFromId ( id ) );
            }
            return allProjects;
        }

        public User addNewUser( string userName, string password, bool isManager )
        {
            List<User> allUsers = getAllUsers ();
            int isManagerInt = isManager ? 1 : 0;
            int newUserId = 0;
            foreach ( User user in allUsers )
            {
                if ( user.id >= newUserId )
                {
                    newUserId = user.id + 1;
                }
            }

            mySqlConnection.Open ();
            MySqlCommand cmd = mySqlConnection.CreateCommand ();
            cmd.CommandText =
                String.Format (
                    "INSERT INTO Users(id,username,password,isManager) " +
                    "VALUES ({0},'{1}','{2}','{3}');",
                    newUserId, userName, password, isManagerInt );
            cmd.ExecuteNonQuery ();
              
            mySqlConnection.Close ();

            User newUser = new User ( userName, password, newUserId, isManager );
            return newUser;
        }

        public void addNewList( ProjectList newProjectList )
        {
            mySqlConnection.Open ();
            MySqlCommand cmd = mySqlConnection.CreateCommand ();
            cmd.CommandText =
                String.Format (
                    "INSERT INTO Lists(id,projectId,title,priority) " +
                    "VALUES ({0},'{1}','{2}','{3}');",
                    newProjectList.Id, newProjectList.ProjectId, newProjectList.getName (), newProjectList.getListPriority () );
            cmd.ExecuteNonQuery ();

            mySqlConnection.Close ();
        }

        public void addNewTask( Task task )
        {
            mySqlConnection.Open ();
            MySqlCommand cmd = mySqlConnection.CreateCommand ();
            string sqlFormattedDateTime = task.GetDueDate ().ToString ( "yyyy-MM-dd HH:mm:ss.fff" );
            cmd.CommandText =
                String.Format (
                    "INSERT INTO Tasks(id,projectId,listId,taskName,taskDescription,taskDueDate,assignedTo) " +
                    "VALUES ({0},{1},{2},'{3}','{4}','{5}',{6});",
                    task.Id, task.ProjectId, task.ListId, task.GetName (), task.GetDescription (), sqlFormattedDateTime, task.AssignedToUserId );
            cmd.ExecuteNonQuery ();

            mySqlConnection.Close ();
        }


        // Complex queries section

        internal int getMaxNumTasksAssignedToSingleUser()
        {
            int maxNumTasksAssignedToUser = 0;
            mySqlConnection.Open ();
            MySqlCommand cmd = mySqlConnection.CreateCommand ();
            cmd.CommandText =
                    "SELECT MAX(subQuery.count) " +
                    "FROM (SELECT COUNT(*) as count " +
                          "FROM Tasks " +
                          "GROUP BY assignedTo) as subQuery ";
            MySqlDataReader reader = cmd.ExecuteReader ();
            while ( reader.Read() )
            {
                maxNumTasksAssignedToUser = reader.GetInt32 ( 0 );
            }
            mySqlConnection.Close ();
            return maxNumTasksAssignedToUser;
        }

        internal float getMinAverageTasksPerListOutOfAllProjects()
        {
            float minAverageTasksPerListOutOfAllProjects = 0;
            mySqlConnection.Open ();
            MySqlCommand cmd = mySqlConnection.CreateCommand ();
            cmd.CommandText =
                    "SELECT MIN(subQuery.averageTasksOnList) " +
                    "FROM (SELECT AVG(subSubQuery.numTasksOnList) as averageTasksOnList " +
                          "FROM (SELECT COUNT(*) as numTasksOnList, projectId " +
                                "FROM Tasks GROUP BY listId, projectId) as subSubQuery " +
                          "GROUP BY projectId) as subQuery";
            MySqlDataReader reader = cmd.ExecuteReader ();
            while ( reader.Read () )
            {
                minAverageTasksPerListOutOfAllProjects = reader.GetFloat ( 0 );
            }
            mySqlConnection.Close ();
            return minAverageTasksPerListOutOfAllProjects;
        }

        internal float getMaxAverageTasksPerListOutOfAllProjects()
        {
            float maxAverageTasksPerListOutOfAllProjects = 0;
            mySqlConnection.Open ();
            MySqlCommand cmd = mySqlConnection.CreateCommand ();
            cmd.CommandText =
                    "SELECT MAX(subQuery.averageTasksOnList) " +
                    "FROM (SELECT AVG(subSubQuery.numTasksOnList) as averageTasksOnList " +
                          "FROM (SELECT COUNT(*) as numTasksOnList, projectId " +
                                "FROM Tasks GROUP BY listId, projectId) as subSubQuery " +
                          "GROUP BY projectId) as subQuery";
            MySqlDataReader reader = cmd.ExecuteReader ();
            while ( reader.Read () )
            {
                maxAverageTasksPerListOutOfAllProjects = reader.GetFloat ( 0 );
            }
            mySqlConnection.Close ();
            return maxAverageTasksPerListOutOfAllProjects;
        }

        public List<string> getListOfAllTaskPriorities()
        {
            List<string> allTaskPriorities = new List<string> ();
            mySqlConnection.Open ();
            MySqlCommand cmd = mySqlConnection.CreateCommand ();
            cmd.CommandText =
                    "SELECT * " +
                    "FROM Tasks, Lists " +
                    "WHERE Tasks.listId = Lists.id ";
            MySqlDataReader reader = cmd.ExecuteReader ();
            while ( reader.Read () )
            {
                string taskPriority = reader.GetString ( 11 );
                allTaskPriorities.Add ( taskPriority );
            }
            mySqlConnection.Close ();
            return allTaskPriorities;

        }
        
        public void closeConnection()
        {
            try
            {
                mySqlConnection.Close ();
            }
            catch(Exception e )
            {
                // do nothing
            }
            
        }
    }
}
